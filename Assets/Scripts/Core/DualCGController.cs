using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 控制左右兩個角色立繪的顯示與高亮狀態
/// </summary>
public class DualCGController : MonoBehaviour
{
    [Header("角色圖層")]
    public Image leftImage;
    public Image rightImage;

    [Header("設定")]
    public float dimAlpha = 0.5f;   // 聆聽者透明度
    public float normalAlpha = 1f;  // 說話者透明度

    /// <summary>
    /// 顯示一個角色立繪，並根據其站位與是否為說話者設定圖像與亮度
    /// </summary>
    public void ShowCharacter(DialogueCharacter character, bool isSpeaking, CharacterPosition position, Sprite overridePortrait = null)
    {
        if (character == null) return;

        Image target = position == CharacterPosition.Left ? leftImage : rightImage;
        if (target != null)
        {
            target.enabled = true;
            target.sprite = overridePortrait != null ? overridePortrait : character.portrait;
            SetAlpha(target, isSpeaking ? normalAlpha : dimAlpha);
        }
    }

    /// <summary>
    /// 設定指定角色是否高亮（說話）
    /// </summary>
    //public void SetSpeaking(DialogueCharacter character)
    //{
    //    if (character == null) return;

    //    if (leftImage != null)
    //    {
    //        SetAlpha(leftImage, ShouldHighlight(character, CharacterPosition.Left) ? normalAlpha : dimAlpha);
    //    }

    //    if (rightImage != null)
    //    {
    //        SetAlpha(rightImage, ShouldHighlight(character, CharacterPosition.Right) ? normalAlpha : dimAlpha);
    //    }
    //}

    /// <summary>
    /// 是否為說話者
    /// </summary>
    //private bool ShouldHighlight(DialogueCharacter speaker, CharacterPosition side)
    //{
    //    return speaker.defaultPosition == side;
    //}

    /// <summary>
    /// 設定透明度
    /// </summary>
    private void SetAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
    /// <summary>
    /// 將指定邊的角色圖像變暗（通常為非說話者）
    /// </summary>
    public void DimAt(CharacterPosition side)
    {
        Image target = side == CharacterPosition.Left ? leftImage : rightImage;

        if (target != null && target.enabled && target.sprite != null)
        {
            SetAlpha(target, dimAlpha);
        }
    }
    /// <summary>
    /// 清除所有角色圖
    /// </summary>
    public void ClearAll()
    {
        if (leftImage != null)
        {
            leftImage.sprite = null;
            leftImage.enabled = false;
        }

        if (rightImage != null)
        {
            rightImage.sprite = null;
            rightImage.enabled = false;
        }
    }
}
