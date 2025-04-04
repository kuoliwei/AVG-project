using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 用來集中儲存所有章節資料清單，供主選單載入與章節選擇
/// </summary>
[CreateAssetMenu(fileName = "SequenceIndex", menuName = "AVG/Sequence Index")]
public class SequenceIndex : ScriptableObject
{
    public List<DialogueSequence> sequences = new List<DialogueSequence>();
}
