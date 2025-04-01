using UnityEngine;

/// <summary>
/// 封裝一段對話資訊：誰說、說什麼、附帶什麼表現（封裝 + 組合）
/// 強化物件導向設計中的「資料封裝」與「資料導向」結構
/// </summary>
[System.Serializable] // 讓這個類別能在 ScriptableObject 或其他容器中序列化
public class DialogueLine
{
    public DialogueCharacter speaker; // 組合角色物件，非繼承 → 表現 Composition 組合關係
    [TextArea(2, 5)]
    public string content; // 對話內容，允許換行（封裝）

    public Sprite cgOverride; // 若有特殊 CG 圖，可在該句切換（封裝）
    public AudioClip voiceClip; // 搭配語音素材播放（封裝）

    public float waitTime = 0.5f; // 播放完該句後等待時間（封裝）
}
