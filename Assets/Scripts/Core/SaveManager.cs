using UnityEngine;               // 引用 UnityEngine 核心函式庫
using System.IO;                 // 使用 File 類別來操作檔案（儲存與讀取）
using System.Collections.Generic; // 使用 List 與泛型功能

/// <summary>
/// SaveManager 控制遊戲進度的儲存與讀取，採用 Singleton 設計模式
/// 支援多個存檔槽，並且可跨場景使用
/// </summary>
public class SaveManager : MonoBehaviour
{
    // 單例模式（Singleton）靜態實例，方便其他腳本用 SaveManager.Instance 存取
    public static SaveManager Instance { get; private set; }

    // 可支援的最大存檔槽數（可自行調整）
    private const int MaxSlots = 3;

    /// <summary>
    /// 初始化 Singleton（保證場景中只有一個 SaveManager）
    /// </summary>
    private void Awake()
    {
        // 若場上已經存在其他 SaveManager 實例 → 銷毀自己（避免重複）
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 設定此實例為 Singleton
        Instance = this;

        // 讓物件在場景切換時不被銷毀
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 傳回指定存檔槽的儲存路徑（檔案名稱為 slot{編號}.json）
    /// 例如：slot0.json、slot1.json...
    /// </summary>
    private string GetSlotPath(int slot)
    {
        // 使用 Application.persistentDataPath 保證跨平台皆可儲存
        return Path.Combine(Application.persistentDataPath, $"slot{slot}.json");
    }

    /// <summary>
    /// 將遊戲進度儲存至指定的槽位
    /// </summary>
    /// <param name="slot">要儲存的槽位編號</param>
    /// <param name="sequenceName">章節名稱（內部識別碼）</param>
    /// <param name="lineIndex">對話進度（目前位於第幾句）</param>
    /// <param name="displayName">顯示用名稱（如：第2章 - 教室）</param>
    public void SaveToSlot(int slot, string sequenceName, int lineIndex, string displayName)
    {
        // 建立存檔資料的物件
        SaveData data = new SaveData
        {
            sequenceName = sequenceName, // 儲存劇情序列的識別名稱
            lineIndex = lineIndex,       // 儲存目前對話索引
            displayName = displayName,   // 顯示用名稱（在 UI 中顯示）
            savedAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") // 取得目前時間
        };

        // 將 SaveData 物件轉為 JSON 字串（美化格式 true）
        string json = JsonUtility.ToJson(data, true);

        // 將 JSON 字串寫入指定的檔案路徑
        File.WriteAllText(GetSlotPath(slot), json);

        // 顯示儲存成功的提示
        Debug.Log($"進度已儲存至 Slot {slot}: {GetSlotPath(slot)}");
    }

    /// <summary>
    /// 從指定槽位讀取遊戲進度（如果檔案不存在則回傳 null）
    /// </summary>
    /// <param name="slot">要讀取的槽位編號</param>
    public SaveData LoadFromSlot(int slot)
    {
        // 取得該槽位的路徑
        string path = GetSlotPath(slot);

        // 若檔案不存在，則顯示警告並返回 null
        if (!File.Exists(path))
        {
            Debug.LogWarning($"找不到 Slot {slot} 的存檔檔案！");
            return null;
        }

        // 讀取整份 JSON 檔案
        string json = File.ReadAllText(path);

        // 將 JSON 字串轉回 SaveData 物件
        return JsonUtility.FromJson<SaveData>(json);
    }
    /// <summary>
    /// 刪除指定槽位的存檔
    /// </summary>
    public void DeleteSlot(int slot)
    {
        string path = GetSlotPath(slot);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Slot {slot} 存檔已刪除");
        }
    }

    /// <summary>
    /// 檢查指定槽位是否有儲存檔存在
    /// </summary>
    /// <param name="slot">槽位編號</param>
    public bool HasSaveInSlot(int slot)
    {
        // 檢查對應 JSON 檔案是否存在
        return File.Exists(GetSlotPath(slot));
    }

    /// <summary>
    /// 取得所有有存檔的槽位資料（用於 UI 顯示）
    /// 回傳一個清單：每項包含槽位編號與對應的存檔資料
    /// </summary>
    public List<(int slot, SaveData data)> GetAllSaveSlots()
    {
        // 建立儲存結果的 List，元素為 (slot編號, SaveData資料)
        List<(int, SaveData)> result = new List<(int, SaveData)>();

        // 依序掃描每個槽位
        for (int i = 0; i < MaxSlots; i++)
        {
            SaveData data = LoadFromSlot(i);

            // 若該槽位有資料，就加入結果清單
            if (data != null)
            {
                result.Add((i, data));
            }
        }

        // 回傳所有已存在存檔的槽位資訊
        return result;
    }
}
