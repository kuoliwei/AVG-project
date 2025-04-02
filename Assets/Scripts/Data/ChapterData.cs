using UnityEngine;

[CreateAssetMenu(fileName = "NewChapter", menuName = "AVG/Chapter Data")]
public class ChapterData : ScriptableObject
{
    public string chapterTitle;                 // 顯示在選單的章節標題
    public DialogueSequence startingSequence;   // 此章節對應的開場對話段落
}
