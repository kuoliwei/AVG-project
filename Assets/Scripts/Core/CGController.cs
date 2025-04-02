using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 專責處理背景圖切換的控制器
/// </summary>
public class CGController : MonoBehaviour
{
    public Image backgroundImage; // 背景圖層

    /// <summary>
    /// 顯示背景圖（或用作角色圖）
    /// </summary>
    public void ShowBackground(Sprite sprite)
    {
        if (backgroundImage == null) return;

        backgroundImage.enabled = sprite != null;
        backgroundImage.sprite = sprite;
    }

    /// <summary>
    /// 清除全部圖片
    /// </summary>
    public void ClearAll()
    {
        ShowBackground(null);
    }
}
