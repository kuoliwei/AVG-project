#if UNITY_EDITOR // 只在編輯器模式下編譯這段
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
// === 修改區：加入 Addressables 編輯器 API 命名空間 ===
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
// === 修改區結束 ===
/// <summary>
/// 讓 string 欄位出現下拉式選單，從指定的 Addressables Group 中列出資源 key
/// 對應到 DialogueSequence 中的 backgroundKey 欄位
/// </summary>
[CustomPropertyDrawer(typeof(BackgroundKeyDropdownAttribute))] // 標記這個 drawer 專門給 BackgroundKeyDropdownAttribute 用
public class BackgroundKeyDropdownDrawer : PropertyDrawer
{
    // 重寫 OnGUI：決定該欄位要怎麼顯示在 Inspector 上
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // === 修改區：建立 Addressables key 清單，第一項為 "none" 表示不指定背景 ===
        List<string> keys = new List<string> { "none" };
        // === 修改區結束 ===

        // === 修改區：從 Addressables 設定中讀取指定 Group（例如 "Backgrounds"） ===
        var settings = AddressableAssetSettingsDefaultObject.Settings; // 取得預設的 Addressables 設定資源
        if (settings != null)
        {
            // 指定你用來放背景圖的 Addressables Group 名稱
            string targetGroupName = "Backgrounds";

            // 從所有 Group 中找出名稱符合者
            var group = settings.groups.FirstOrDefault(g => g != null && g.Name == targetGroupName);
            if (group != null)
            {
                // 遍歷 Group 中的所有資源 entry，將其 address 加入選單清單中
                foreach (var entry in group.entries)
                {
                    keys.Add(entry.address);
                }
            }
        }
        // === 修改區結束 ===

        //// 直接從 SerializedObject 根層取得 backgroundDatabase 欄位
        //SerializedProperty sequenceBgProp = property.serializedObject.FindProperty("backgroundDatabase");

        //// 取出對應 ScriptableObject
        //BackgroundDatabase bgDatabase = sequenceBgProp?.objectReferenceValue as BackgroundDatabase;
        //// 若無資料庫，則顯示為普通輸入欄位
        //if (bgDatabase == null)
        //{
        //    property.stringValue = EditorGUI.TextField(position, label.text, property.stringValue);
        //    return;
        //}

        //// 建立背景 key 清單，第一個為 "none"（代表不覆蓋）
        //List<string> keys = new List<string> { "none" };

        //// 加入資料庫中的背景 key（若有）
        //keys.AddRange(bgDatabase.backgrounds.Select(b => b.key));

        // 找目前值對應的 index，若找不到則使用第一個
        int selectedIndex = keys.IndexOf(property.stringValue);
        if (selectedIndex == -1) selectedIndex = 0;

        // 顯示下拉式選單
        selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, keys.ToArray());

        // 寫回選擇結果（保守加上 index 範圍檢查）
        if (selectedIndex >= 0 && selectedIndex < keys.Count)
        {
            property.stringValue = keys[selectedIndex];
        }
    }
}
#endif
