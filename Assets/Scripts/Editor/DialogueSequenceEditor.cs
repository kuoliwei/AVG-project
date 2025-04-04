#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.TextCore.Text;

/// <summary>
/// 自訂 Inspector，讓 DialogueSequence 的 backgroundKey 顯示成下拉選單
/// </summary>
[CustomEditor(typeof(DialogueSequence))] // 指定這個自訂編輯器是給 DialogueSequence 用的
public class DialogueSequenceEditor : Editor
{
    private SerializedProperty linesProp;
    private void OnEnable()
    {
        linesProp = serializedObject.FindProperty("lines");
    }
    // 重寫 Unity 預設的 Inspector GUI
    public override void OnInspectorGUI()
    {
        // 把 target 轉型為 DialogueCharacter，以便使用其欄位
        DialogueSequence sequence = (DialogueSequence)target;

        // 顯示sequence名稱的文字欄位（手動輸入）
        EditorGUILayout.LabelField("章節代號 / 顯示名稱", EditorStyles.boldLabel);
        sequence.sequenceName = EditorGUILayout.TextField("Sequence Name", sequence.sequenceName);

        // 顯示 BackgroundDatabase 的 Object 欄位（可拖拉 ScriptableObject 進來）
        EditorGUILayout.LabelField("背景CG database", EditorStyles.boldLabel);
        sequence.backgroundDatabase = (BackgroundDatabase)EditorGUILayout.ObjectField(
            "Background Database",                     // 欄位名稱
            sequence.backgroundDatabase,              // 目前的值
            typeof(BackgroundDatabase),      // 限定類型
            false                                    // 不允許拖場景物件（只允許資產）
        );

        EditorGUILayout.LabelField("預設背景 key", EditorStyles.boldLabel);
        // 如果已經指定了 BackgroundDatabase，就從中抓出所有 key，做出下拉選單
        if (sequence.backgroundDatabase != null)
        {
            // 把資料庫中的 key 取出，變成一個 List<string>
            var keys = sequence.backgroundDatabase.backgrounds.Select(p => p.key).ToList();

            // 根據當前 backgroundKey 找到對應的索引值
            int selectedIndex = Mathf.Max(0, keys.IndexOf(sequence.backgroundKey));

            // 顯示下拉式選單，讓使用者選擇其中一個 key
            selectedIndex = EditorGUILayout.Popup("Background Key", selectedIndex, keys.ToArray());

            // 更新角色的 backgroundKey 值為選中的 key
            if (selectedIndex >= 0 && selectedIndex < keys.Count)
                sequence.backgroundKey = keys[selectedIndex];
        }
        else
        {
            // 如果沒有資料庫，就顯示為一般文字輸入欄位
            sequence.backgroundKey = EditorGUILayout.TextField("Default Portrait Key", sequence.backgroundKey);
        }

        // 顯示進場延遲時間（秒）欄位（手動輸入）
        EditorGUILayout.LabelField("進場延遲時間（秒）", EditorStyles.boldLabel);
        sequence.sequenceStartDelay = EditorGUILayout.FloatField("sequence Start Delay", sequence.sequenceStartDelay);

        serializedObject.Update();
        EditorGUILayout.Space();
        //EditorGUILayout.LabelField("對話句子列表", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(linesProp, new GUIContent("Lines"), true);
        serializedObject.ApplyModifiedProperties();


        // 顯示 DialogueBranch 的 Object 欄位（可拖拉 ScriptableObject 進來）
        EditorGUILayout.LabelField("劇情結束後的選項分支", EditorStyles.boldLabel);
        sequence.branchAfterSequence = (DialogueBranch)EditorGUILayout.ObjectField(
            "Branch After Sequence",                     // 欄位名稱
            sequence.branchAfterSequence,              // 目前的值
            typeof(DialogueBranch),      // 限定類型
            false                                    // 不允許拖場景物件（只允許資產）
        );

        // 如果有修改任何欄位，標記資料為已變更，讓 Unity 儲存它
        if (GUI.changed)
        {
            EditorUtility.SetDirty(sequence); // 告知 Unity：這個 ScriptableObject 有變化
        }
    }
}
#endif
