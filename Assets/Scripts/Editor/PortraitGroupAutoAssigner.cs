#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

/// <summary>
/// 將指定資料夾下的立繪圖自動分類至對應角色名稱的 Addressables Group 中
/// 群組命名格式為 Portrait_角色名，圖的 address 則設為去掉副檔名的檔名
/// </summary>
public class PortraitGroupAutoAssigner
{
    /// <summary>
    /// 在 Unity 編輯器中建立一個工具選單項目
    /// 點選後會執行整個自動分類流程
    /// </summary>
    [MenuItem("Tools/Addressables/Auto Assign Portraits to Groups")]
    public static void AutoAssignPortraits()
    {
        // 定義要掃描的立繪圖片資料夾
        string portraitsFolder = "Assets/Art/Portraits";

        // 掃描該資料夾下所有 .png 圖檔（排除 .meta 檔）
        var allPortraitPaths = Directory.GetFiles(portraitsFolder, "*.png", SearchOption.AllDirectories)
            .Where(path => !path.EndsWith(".meta")).ToList();

        // 若找不到任何立繪圖，則顯示警告並中止
        if (allPortraitPaths.Count == 0)
        {
            Debug.LogWarning("找不到任何立繪圖檔 (*.png) 在 " + portraitsFolder);
            return;
        }

        // 嘗試取得 Addressables 設定（Editor 專用 API）
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("找不到 Addressables 設定，請先初始化 Addressables 系統");
            return;
        }

        // 逐一處理每張圖片
        foreach (var path in allPortraitPaths)
        {
            // 確保路徑格式正確（Windows 為 \，需統一轉換為 /）
            string assetPath = path.Replace('\\', '/');

            // 嘗試載入該圖檔為 Texture2D（確認為有效圖檔）
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture == null)
            {
                Debug.LogWarning($"忽略無法載入的圖片資源：{assetPath}");
                continue;
            }

            // 取得不含副檔名的檔名，例如 Alice_angry
            string fileName = Path.GetFileNameWithoutExtension(assetPath);

            // 根據檔名用底線分隔，取前段作為角色名（如 Alice）
            string[] parts = fileName.Split('_');
            if (parts.Length == 0) continue;

            string characterName = parts[0]; // e.g. "Alice"
            string groupName = $"Portrait_{characterName}"; // e.g. "Portrait_Alice"

            // 檢查該角色的 Group 是否已存在，若不存在則自動建立
            var group = settings.groups.FirstOrDefault(g => g != null && g.Name == groupName);
            if (group == null)
            {
                group = settings.CreateGroup(groupName, false, false, false, null,
                    typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
                Debug.Log($"建立新 Group：{groupName}");
            }

            // 將圖檔加入該 Group，若已存在會自動搬移
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            var entry = settings.CreateOrMoveEntry(guid, group);

            // 設定該圖的 address 為檔名（不含副檔名）
            entry.address = fileName;
        }

        // 儲存修改後的 Addressables 設定
        AssetDatabase.SaveAssets();
        Debug.Log("所有立繪已依角色名稱自動分類進 Addressables Group 並設定 address。");
    }
}
#endif
