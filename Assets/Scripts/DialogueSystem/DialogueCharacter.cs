using UnityEngine;

/// <summary>
/// 代表一個可對話角色的資料（封裝角色名字、立繪、名稱顏色）
/// 使用 ScriptableObject 達到資料與邏輯分離（物件導向 - 封裝）
/// </summary>
[CreateAssetMenu(fileName = "NewCharacter", menuName = "AVG/Character")]
public class DialogueCharacter : ScriptableObject
{
    public string characterName; // 角色名稱
    public Color nameColor = Color.white; // 對話框顯示的名稱顏色
    //public CharactersPortraitDatabase portraitDatabase; // 角色立繪圖片database（封裝）
    public string portraitGroup; // 要載入立繪的 Addressables Group 名稱，例如 Portrait_Alice
    public string defaultPortraitKey; // 該角色預設立繪圖的 Addressable key（例如 Alice_normal）
}