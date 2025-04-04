#if UNITY_EDITOR // 只在編輯器模式下編譯這段
using UnityEngine;
using UnityEditor;
using System.Linq;

/// <summary>
/// 對有加上 [PortraitKeyDropdown] 的 string 欄位顯示下拉式選單
/// 根據 DialogueLine.speaker 中的 portraitDatabase 自動生成可選 key 清單
/// </summary>
[CustomPropertyDrawer(typeof(PortraitKeyDropdownAttribute))] // 標記這個 drawer 專門給 PortraitKeyDropdownAttribute 用
public class PortraitKeyDropdownDrawer : PropertyDrawer
{
    // 重寫 OnGUI：決定該欄位要怎麼顯示在 Inspector 上
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 嘗試取得對應的 speaker 欄位（與當前欄位位於相同 DialogueLine 中）
        string speakerPath = property.propertyPath.Replace(property.name, "speaker");
        SerializedProperty speakerProp = property.serializedObject.FindProperty(speakerPath);

        // 從 speaker 中取出對應的 DialogueCharacter
        DialogueCharacter speaker = speakerProp != null ? speakerProp.objectReferenceValue as DialogueCharacter : null;
        //Debug.Log(speakerProp);
        // 從 speaker 拿到 portraitDatabase，如果沒有則為 null
        CharactersPortraitDatabase database = speaker != null ? speaker.portraitDatabase : null;
        // 若無資料庫，則顯示為普通輸入欄位
        if (database == null)
        {
            property.stringValue = EditorGUI.TextField(position, label.text, property.stringValue);
            return;
        }
        // 建立一個 portrait key 的清單，預設第一個是空字串（代表不覆蓋、使用預設圖）
        var keys = new System.Collections.Generic.List<string> { "none" };

        // 如果有指定 database，就從中取得所有的 key，加入清單
        keys.AddRange(database.CharactersPortraits.Select(p => p.key)); // 用 LINQ 選出所有 key

        // 正確比對目前欄位值的位置（允許 ""）
        int selectedIndex = keys.IndexOf(property.stringValue);
        if (selectedIndex == -1) selectedIndex = 0; // 若找不到就預設選第一個（空）

        // 顯示 dropdown（下拉式選單），將選到的 index 回存回 selectedIndex
        selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, keys.ToArray());

        // 將使用者選擇的值寫回到該字串欄位
        if (selectedIndex >= 0 && selectedIndex < keys.Count)
        {
            property.stringValue = keys[selectedIndex];
        }
    }
}
#endif
