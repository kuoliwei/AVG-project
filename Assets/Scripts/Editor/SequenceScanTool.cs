using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AddressableAssets;

/// <summary>
/// 編輯器工具：掃描所有 DialogueSequence，並自動建立 SequenceIndex
/// </summary>
public class SequenceScanTool : EditorWindow
{
    private List<DialogueSequence> foundSequences; // 儲存所有找到的序列資料
    private List<string> validationLogs; // 儲存所有檢查中發現的錯誤或警告

    private Vector2 scroll; // 用於 GUI 捲動

    // 註冊這個工具於 Unity 的 Tools 選單中
    [MenuItem("Tools/AVG/掃描序列並建立索引")]
    public static void OpenWindow()
    {
        GetWindow<SequenceScanTool>("序列掃描工具");
    }

    // 當視窗開啟時，自動掃描一次序列資料
    private void OnEnable()
    {
        ScanSequences();
    }

    // 編輯器視窗繪製邏輯
    private void OnGUI()
    {
        if (GUILayout.Button("重新掃描所有序列"))
        {
            ScanSequences();
        }

        GUILayout.Space(10);

        scroll = EditorGUILayout.BeginScrollView(scroll);

        if (foundSequences != null)
        {
            EditorGUILayout.LabelField("共找到序列數量：" + foundSequences.Count, EditorStyles.boldLabel);
            GUILayout.Space(5);

            // 顯示每個序列的物件資訊
            foreach (var seq in foundSequences)
            {
                EditorGUILayout.ObjectField("序列：", seq, typeof(DialogueSequence), false);
            }

            GUILayout.Space(10);
            EditorGUILayout.LabelField("驗證結果：", EditorStyles.boldLabel);

            if (validationLogs != null)
            {
                foreach (var log in validationLogs)
                {
                    EditorGUILayout.HelpBox(log, MessageType.Warning);
                }
            }
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        if (GUILayout.Button("建立 / 更新 SequenceIndex 資料"))
        {
            SaveSequenceIndex();
        }
    }

    /// <summary>
    /// 掃描 Resources/Dialogue/Sequences 中的所有 DialogueSequence
    /// 並進行簡單驗證檢查
    /// </summary>
    private void ScanSequences()
    {
        foundSequences = Resources.LoadAll<DialogueSequence>("Dialogue/Sequences").ToList();
        validationLogs = new List<string>();

        foreach (var seq in foundSequences)
        {
            if (seq == null)
            {
                validationLogs.Add("有無效序列資料存在！");
                continue;
            }

            if (string.IsNullOrEmpty(seq.name))
            {
                validationLogs.Add($"序列名稱空白：{seq}");
            }

            if (seq.lines == null || seq.lines.Count == 0)
            {
                validationLogs.Add($"序列「{seq.name}」沒有對話內容！");
            }

            for (int i = 0; i < seq.lines.Count; i++)
            {
                var line = seq.lines[i];

                if (line.speaker == null)
                    validationLogs.Add($"[{seq.name}] 第 {i + 1} 行缺少 speaker");

                // 可擴充檢查背景 key 是否存在於 BackgroundManager（略）
                if (!string.IsNullOrEmpty(line.backgroundKeyOverride))
                {
                    // 檢查邏輯略，需搭配背景管理器中的 key 資料
                }

                // 若角色有指定立繪資料庫，則檢查該 key 是否存在
                // === 修改區：改為從 Addressables Group 中查找對應立繪 key ===
                if (line.speaker != null &&
                    !string.IsNullOrEmpty(line.charactersPortraitsKeyOverride) &&
                    line.charactersPortraitsKeyOverride != "none")
                {
                    string groupName = line.speaker.portraitGroup;

                    // 檢查是否指定了有效的 portraitGroup
                    if (!string.IsNullOrEmpty(groupName))
                    {
                        var settings = AddressableAssetSettingsDefaultObject.Settings;
                        if (settings != null)
                        {
                            var group = settings.groups.FirstOrDefault(g => g != null && g.Name == groupName);
                            if (group != null)
                            {
                                bool found = group.entries.Any(e => e.address == line.charactersPortraitsKeyOverride);
                                if (!found)
                                {
                                    validationLogs.Add($"[{seq.name}] 第 {i + 1} 行：角色「{line.speaker.characterName}」在 Group「{groupName}」中找不到立繪 key「{line.charactersPortraitsKeyOverride}」");
                                }
                            }
                            else
                            {
                                validationLogs.Add($"[{seq.name}] 第 {i + 1} 行：角色「{line.speaker.characterName}」指定的 Group「{groupName}」不存在於 Addressables");
                            }
                        }
                    }
                }
                //if (line.speaker != null && line.speaker.portraitDatabase != null)
                //{
                //    if (!string.IsNullOrEmpty(line.charactersPortraitsKeyOverride) && line.charactersPortraitsKeyOverride != "none")
                //    {
                //        bool found = line.speaker.portraitDatabase.CharactersPortraits
                //            .Any(p => p.key == line.charactersPortraitsKeyOverride);

                //        if (!found)
                //        {
                //            validationLogs.Add($"[{seq.name}] 第 {i + 1} 行：角色「{line.speaker.characterName}」無此立繪 key「{line.charactersPortraitsKeyOverride}」");
                //        }
                //    }
                //}
            }
        }
    }

    /// <summary>
    /// 將掃描到的序列列表儲存到 SequenceIndex.asset 中
    /// 若沒有則自動建立新的資源檔
    /// </summary>
    private void SaveSequenceIndex()
    {
        string path = "Assets/Resources/Dialogue/SequenceIndex.asset";

        SequenceIndex index = AssetDatabase.LoadAssetAtPath<SequenceIndex>(path);

        if (index == null)
        {
            index = ScriptableObject.CreateInstance<SequenceIndex>();
            AssetDatabase.CreateAsset(index, path);
        }

        index.sequences = foundSequences;
        EditorUtility.SetDirty(index);
        AssetDatabase.SaveAssets();

        Debug.Log($"SequenceIndex 已更新，共 {index.sequences.Count} 筆序列");
    }
}
