using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ChapterList", menuName = "AVG/Chapter List")]
public class ChapterList : ScriptableObject
{
    public List<ChapterData> chapters = new List<ChapterData>(); // 所有章節集合
}
