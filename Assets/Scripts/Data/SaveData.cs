using System;

[Serializable]
public class SaveData
{
    public string sequenceName;   // 對話段落（ScriptableObject 的 name）
    public int lineIndex;         // 第幾句對話
}
