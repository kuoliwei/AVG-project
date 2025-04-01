using UnityEngine;

/// <summary>
/// 玩家在選項中可選的每個分支，點選後連到下一段對話
/// 這個設計與 Strategy Pattern（策略模式）非常類似：
/// 每個選項可視為一種跳轉策略，決定遊戲如何繼續（策略模式雛型）
/// </summary>
[System.Serializable]
public class DialogueOption
{
    public string optionText; // 顯示在 UI 上的選項文字（封裝）
    public DialogueSequence nextSequence; // 該選項指向的下個對話序列（封裝）
}
