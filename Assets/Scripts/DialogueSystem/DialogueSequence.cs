using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 對話序列：一整段對話流程的封裝
/// 這是典型的 Data-Driven Design（資料驅動設計）：
/// 不用寫死對話流程，而是透過資料決定執行內容
/// </summary>
[CreateAssetMenu(fileName = "NewSequence", menuName = "AVG/DialogueSequence")]
public class DialogueSequence : ScriptableObject // 封裝劇情資料結構
{
    [Header("章節代號 / 顯示名稱")]
    public string sequenceName; // 序列名稱，方便識別（封裝）
    [Header("預設背景 key")]
    public string backgroundKey;
    [Header("背景CG database")]
    public BackgroundDatabase backgroundDatabase; // 背景圖片database（封裝）
    [Header("進場延遲時間（秒）")]
    public float sequenceStartDelay = 0.5f;
    [Header("對話內容")]
    [Tooltip("這段劇情中要播放的所有對話句子")]
    public List<DialogueLine> lines; // 這段對話的所有句子（封裝 + 組合）
    [Header("劇情結束後的選項分支")]
    public DialogueBranch branchAfterSequence; // 對話播完後是否跳轉到選項分支（封裝 + 控制流程跳轉）
}
