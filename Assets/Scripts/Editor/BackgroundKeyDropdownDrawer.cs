#if UNITY_EDITOR // 只在編輯器模式下編譯這段
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// 對有加上 [BackgroundKeyDropdown] 的 string 欄位顯示下拉式選單
/// 根據 DialogueLine.speaker 中的 backgroundDatabase 自動生成可選 key 清單
/// </summary>
[CustomPropertyDrawer(typeof(BackgroundKeyDropdownAttribute))] // 標記這個 drawer 專門給 BackgroundKeyDropdownAttribute 用
public class BackgroundKeyDropdownDrawer : PropertyDrawer
{
    // 重寫 OnGUI：決定該欄位要怎麼顯示在 Inspector 上
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 直接從 SerializedObject 根層取得 backgroundDatabase 欄位
        SerializedProperty sequenceBgProp = property.serializedObject.FindProperty("backgroundDatabase");

        // 取出對應 ScriptableObject
        BackgroundDatabase bgDatabase = sequenceBgProp?.objectReferenceValue as BackgroundDatabase;
        // 若無資料庫，則顯示為普通輸入欄位
        if (bgDatabase == null)
        {
            property.stringValue = EditorGUI.TextField(position, label.text, property.stringValue);
            return;
        }

        // 建立背景 key 清單，第一個為 "none"（代表不覆蓋）
        List<string> keys = new List<string> { "none" };

        // 加入資料庫中的背景 key（若有）
        keys.AddRange(bgDatabase.backgrounds.Select(b => b.key));

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
