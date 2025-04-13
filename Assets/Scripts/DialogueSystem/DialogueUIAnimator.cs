using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// 控制日式 AVG 遊戲中 Dialogue 面板與選項按鈕的動畫效果（使用 DOTween）
/// </summary>
public class DialogueUIAnimator : MonoBehaviour
{
    // 主要 UI 元件區塊
    [Header("主要 UI 元件")]
    public RectTransform dialoguePanel;          // 對話面板（RectTransform），用於進出場動畫
    public Image leftCharacterImage;             // 左側角色立繪（Image 元件）
    public Image rightCharacterImage;            // 右側角色立繪（Image 元件）

    // 選項按鈕清單（會播放彈出與 Hover 動畫）
    [Header("選項動畫設定")]
    public List<Button> optionButtons;           // 對話選項按鈕的集合（依順序設定）

    // 所有動畫的時間與參數設定
    [Header("動畫參數")]
    public float panelMoveY = -760f;             // 對話面板初始 Y 座標（滑入前的起點）
    public float panelY = -320f;             // 對話面板 Y 座標（滑入後的位置）
    public float panelAnimDuration = 0.5f;       // 對話面板進出場動畫時間（秒）
    public float fadeDuration = 0.5f;            // 立繪淡入動畫時間（秒）
    public float optionPopDuration = 0.3f;       // 選項按鈕彈出動畫時間（秒）
    public float optionHoverScale = 1.1f;        // 選項按鈕滑入時的放大比例

    /// <summary>
    /// 顯示對話面板（由畫面底部滑入）
    /// </summary>
    public void ShowDialoguePanel()
    {
        //Vector2 currentPosition = dialoguePanel.anchoredPosition;
        //Vector2 currentSizeDelta = dialoguePanel.sizeDelta;
        //Debug.Log($"{currentPosition},{currentSizeDelta}");
        ////dialoguePanel.anchoredPosition = new Vector2(currentPosition.x, currentPosition.y - currentSizeDelta.y);    // 將起始位置設為畫面下方
        //Debug.Log($"{currentPosition},{dialoguePanel.anchoredPosition}");
        Image dialoguePanelImage = dialoguePanel.GetComponent<Image>();
        if (dialoguePanel.anchoredPosition.y != panelY)
        {
            dialoguePanel.DOAnchorPosY(panelY, panelAnimDuration)                // 播放滑入動畫到 Y=0
              .SetEase(Ease.OutBack);                           // 使用回彈曲線讓動畫更有彈性
            dialoguePanelImage.DOFade(0.5f, panelAnimDuration * 4);
        }
    }

    /// <summary>
    /// 隱藏對話面板（滑回畫面底部後隱藏）
    /// </summary>
    public void HideDialoguePanel()
    {
        //Vector2 currentPosition = dialoguePanel.anchoredPosition;
        //Vector2 currentSizeDelta = dialoguePanel.sizeDelta;
        Image dialoguePanelImage = dialoguePanel.GetComponent<Image>();
        if (dialoguePanel.anchoredPosition.y != panelMoveY)
        {
            dialoguePanel.DOAnchorPosY(panelMoveY, panelAnimDuration)       // 播放滑出動畫到指定 Y 座標
                          .SetEase(Ease.InBack)                             // 使用滑出感的曲線
                          //.OnComplete(() =>                                // 動畫完成後呼叫以下方法
                          //{
                          //    dialoguePanel.gameObject.SetActive(false);    // 將面板設為不啟用
                          //})
                          ;
            dialoguePanelImage.DOFade(0, panelAnimDuration / 2f);
        }

    }

    /// <summary>
    /// 顯示指定角色立繪，並使用淡入動畫
    /// </summary>
    /// <param name="characterImage">目標角色 Image 元件</param>
    /// <param name="sprite">要顯示的角色圖片</param>
    public void ShowCharacter(Image characterImage, Sprite sprite)
    {
        characterImage.sprite = sprite;                                 // 設定角色圖片
        characterImage.color = new Color(1, 1, 1, 0);                    // 設定為全透明
        characterImage.DOFade(1f, fadeDuration);                         // 淡入顯示（透明度從 0 到 1）
    }

    /// <summary>
    /// 顯示選項按鈕，並為每個按鈕播放彈出動畫與註冊滑入滑出事件
    /// </summary>
    public void ShowOptions()
    {
        for (int i = 0; i < optionButtons.Count; i++)                    // 遍歷所有選項按鈕
        {
            Button btn = optionButtons[i];                              // 取得當前按鈕
            Transform tf = btn.transform;                               // 取得其 Transform，用於控制縮放

            tf.localScale = Vector3.zero;                               // 初始縮放為 0（看不見）
            btn.gameObject.SetActive(true);                             // 啟用按鈕物件
            tf.DOScale(Vector3.one, optionPopDuration)                  // 播放彈出動畫（縮放至 1）
              .SetEase(Ease.OutBack)                                    // 使用彈性動畫曲線
              .SetDelay(i * 0.05f);                                     // 加入延遲，營造漸進式出現感

            AddHoverAnimation(btn);                                     // 為按鈕加上滑入滑出事件
        }
    }

    /// <summary>
    /// 為按鈕加上 Hover 動畫事件（PointerEnter / PointerExit）
    /// </summary>
    /// <param name="button">目標按鈕</param>
    private void AddHoverAnimation(Button button)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>();     // 嘗試取得 EventTrigger 元件
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();   // 若無，則新增一個
        }

        trigger.triggers.Clear();                                       // 清除舊的事件避免重複

        // 建立滑入事件（滑鼠進入時）
        EventTrigger.Entry entryEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter                     // 事件類型為 PointerEnter
        };
        entryEnter.callback.AddListener((data) => OnHoverEnter(button.transform)); // 加入放大回呼
        trigger.triggers.Add(entryEnter);                               // 加入到觸發清單

        // 建立滑出事件（滑鼠離開時）
        EventTrigger.Entry entryExit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit                      // 事件類型為 PointerExit
        };
        entryExit.callback.AddListener((data) => OnHoverExit(button.transform));   // 加入還原回呼
        trigger.triggers.Add(entryExit);                                // 加入到觸發清單
    }

    /// <summary>
    /// 滑入動畫（按鈕放大）
    /// </summary>
    /// <param name="button">目標按鈕的 Transform</param>
    public void OnHoverEnter(Transform button)
    {
        button.DOScale(optionHoverScale, 0.2f)                           // 縮放到指定比例
              .SetEase(Ease.OutQuad);                                   // 使用滑順曲線
    }

    /// <summary>
    /// 滑出動畫（按鈕恢復原大小）
    /// </summary>
    /// <param name="button">目標按鈕的 Transform</param>
    public void OnHoverExit(Transform button)
    {
        button.DOScale(1f, 0.2f)                                        // 縮放回原本大小（1.0）
              .SetEase(Ease.OutQuad);                                   // 使用滑順曲線
    }
    public void PrepareOptionButton(GameObject buttonObj, int index)
    {
        Transform tf = buttonObj.transform;
        tf.localScale = Vector3.zero;
        tf.DOScale(Vector3.one, optionPopDuration)
            .SetEase(Ease.OutBack)
            .SetDelay(index * 0.05f);

        Button btn = buttonObj.GetComponent<Button>();
        if (btn != null) AddHoverAnimation(btn);
    }

}
