using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 一個選項分支：出現一段提問，並給予多個選項讓玩家選擇
/// 封裝選項行為，並準備後續擴充為事件通知系統（可進化為 Observer Pattern）
/// </summary>
[CreateAssetMenu(fileName = "NewBranch", menuName = "AVG/DialogueBranch")]
public class DialogueBranch : ScriptableObject // 使用 ScriptableObject 封裝分支資料（封裝）
{
    [TextArea(1, 3)]
    public string question; // 玩家會看到的提問文字（封裝）

    public List<DialogueOption> options; // 可選擇的選項列表，每個指向不同路線（封裝 + 組合）
}
