using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 儲存章節解鎖資料的結構，用於 Save/Load 序列化
/// </summary>
[System.Serializable]
public class ChapterProgressData
{
    // 儲存已解鎖的章節 ID 清單，例如 "chapter_1", "chapter_2"
    public List<string> unlockedChapterIds = new List<string>();
}

/// <summary>
/// 控制章節解鎖邏輯的單例類別
/// </summary>
public class ChapterProgress : MonoBehaviour
{
    // 靜態單例存取點（確保場上只有一份）
    public static ChapterProgress Instance { get; private set; }

    // 實際記錄玩家目前進度的資料
    private ChapterProgressData data = new ChapterProgressData();

    private void Awake()
    {
        // 設定單例實體（若場上已存在其他實體則銷毀）
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        data = ChapterProgressStorage.Load();
    }

    private void Start()
    {
        //Debug.Log("目前已解鎖章節：" + string.Join(",", data.unlockedChapterIds));
        // 若初次開啟遊戲沒有任何章節 → 預設解鎖第一章
        if (data.unlockedChapterIds.Count == 0)
        {
            UnlockChapter("chapter_1");
        }
    }

    /// <summary>
    /// 判斷指定章節是否已解鎖
    /// </summary>
    /// <param name="chapterId">章節 ID（如 chapter_1）</param>
    /// <returns>是否已解鎖</returns>
    public bool IsChapterUnlocked(string chapterId)
    {
        //Debug.Log(string.Join(", ", data.unlockedChapterIds));
        return data.unlockedChapterIds.Contains(chapterId);
    }

    /// <summary>
    /// 將指定章節標記為已解鎖
    /// </summary>
    /// <param name="chapterId">要解鎖的章節 ID</param>
    public void UnlockChapter(string chapterId)
    {
        if (!data.unlockedChapterIds.Contains(chapterId))
        {
            data.unlockedChapterIds.Add(chapterId);
            ChapterProgressStorage.Save(data); // 新增：自動存檔
        }
    }

    /// <summary>
    /// 取得目前章節進度資料（用於存檔）
    /// </summary>
    /// <returns>目前的章節進度資料物件</returns>
    public ChapterProgressData GetData()
    {
        return data;
    }

    /// <summary>
    /// 載入章節進度資料（用於讀檔）
    /// </summary>
    /// <param name="loadedData">從存檔讀出的資料</param>
    public void LoadData(ChapterProgressData loadedData)
    {
        data = loadedData;
    }
    /// <summary>
    /// 清除所有章節解鎖資料，重設為預設狀態
    /// </summary>
    public void ResetProgress()
    {
        // 清空章節資料
        data.unlockedChapterIds.Clear();

        // 預設重新解鎖第一章（必要時你可移除這行）
        UnlockChapter("chapter_1");

        // 儲存到硬碟
        ChapterProgressStorage.Save(data);

        Debug.Log("已重置章節解鎖進度。");
    }
}
