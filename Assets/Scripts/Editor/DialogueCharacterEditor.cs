#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;

/// <summary>
/// 自訂 Inspector，讓 DialogueCharacter 的 defaultPortraitKey 顯示成下拉選單
/// </summary>
[CustomEditor(typeof(DialogueCharacter))] // 指定這個自訂編輯器是給 DialogueCharacter 用的
public class DialogueCharacterEditor : Editor
{
    // 重寫 Unity 預設的 Inspector GUI
    public override void OnInspectorGUI()
    {
        // 把 target 轉型為 DialogueCharacter，以便使用其欄位
        DialogueCharacter character = (DialogueCharacter)target;

        // 顯示角色名稱的文字欄位（手動輸入）
        character.characterName = EditorGUILayout.TextField("Character Name", character.characterName);

        // 顯示名稱顏色的色彩選擇器（Color Picker）
        character.nameColor = EditorGUILayout.ColorField("Name Color", character.nameColor);

        // 顯示 Portrait Database 的 Object 欄位（可拖拉 ScriptableObject 進來）
        character.portraitDatabase = (CharactersPortraitDatabase)EditorGUILayout.ObjectField(
            "Portrait Database",                     // 欄位名稱
            character.portraitDatabase,              // 目前的值
            typeof(CharactersPortraitDatabase),      // 限定類型
            false                                    // 不允許拖場景物件（只允許資產）
        );

        // 如果已經指定了 portraitDatabase，就從中抓出所有 key，做出下拉選單
        if (character.portraitDatabase != null)
        {
            // 把資料庫中的 key 取出，變成一個 List<string>
            var keys = character.portraitDatabase.CharactersPortraits.Select(p => p.key).ToList();

            // 根據當前 defaultPortraitKey 找到對應的索引值
            int selectedIndex = Mathf.Max(0, keys.IndexOf(character.defaultPortraitKey));

            // 顯示下拉式選單，讓使用者選擇其中一個 key
            selectedIndex = EditorGUILayout.Popup("Default Portrait Key", selectedIndex, keys.ToArray());

            // 更新角色的 defaultPortraitKey 值為選中的 key
            if (selectedIndex >= 0 && selectedIndex < keys.Count)
                character.defaultPortraitKey = keys[selectedIndex];
        }
        else
        {
            // 如果沒有資料庫，就顯示為一般文字輸入欄位
            character.defaultPortraitKey = EditorGUILayout.TextField("Default Portrait Key", character.defaultPortraitKey);
        }

        // 如果有修改任何欄位，標記資料為已變更，讓 Unity 儲存它
        if (GUI.changed)
        {
            EditorUtility.SetDirty(character); // 告知 Unity：這個 ScriptableObject 有變化
        }
    }
}
#endif
