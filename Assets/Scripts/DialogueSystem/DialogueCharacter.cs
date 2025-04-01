using UnityEngine;

/// <summary>
/// 代表一個可對話角色的資料（封裝角色名字、立繪、名稱顏色）
/// 使用 ScriptableObject 達到資料與邏輯分離（物件導向 - 封裝）
/// </summary>
[CreateAssetMenu(fileName = "NewCharacter", menuName = "AVG/Character")]
public class DialogueCharacter : ScriptableObject // 透過繼承 Unity 的 ScriptableObject 使資料可獨立儲存於資產中
{
    public string characterName; // 角色名稱（封裝）
    public Sprite portrait; // 角色立繪圖片（封裝）
    public Color nameColor = Color.white; // 對話框顯示的名稱顏色，預設為白色（封裝）
}
