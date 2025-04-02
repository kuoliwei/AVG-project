using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// DialogueUI 負責更新 UI 上的對話顯示內容
/// </summary>
public class DialogueUI : MonoBehaviour
{
    [Header("UI 元件")]
    public Text nameText;        // 顯示角色名稱
    public Text contentText;     // 顯示對話內容
    public GameObject continueHint; // 顯示「按空白鍵繼續」提示

    private Coroutine typingCoroutine;     // 打字效果的協程
    private bool isTyping = false;         // 是否正在打字中
    private string fullText = "";          // 完整句子（保留）

    public bool IsTyping => isTyping;      // 提供外部查詢

    [Header("設定")]
    public float charInterval = 0.05f;     // 每個字的間隔秒數

    /// <summary>
    /// 顯示對話內容
    /// </summary>
    public void SetLine(DialogueLine line)
    {
        if (line == null || line.speaker == null) return;

        nameText.text = line.speaker.characterName;
        fullText = line.content;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(fullText));
    }
    /// <summary>
    /// 顯示完整對話，不再逐字
    /// </summary>
    public void ShowFull()
    {
        if (isTyping)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            contentText.text = fullText;
            isTyping = false;
            continueHint.SetActive(true);
        }
    }
    /// <summary>
    /// 清空 UI 畫面
    /// </summary>
    public void Clear()
    {
        nameText.text = "";
        contentText.text = "";
        continueHint.SetActive(false);
        isTyping = false;
    }
    /// <summary>
    /// 執行逐字輸出效果
    /// </summary>
    private IEnumerator TypeText(string text)
    {
        contentText.text = "";
        isTyping = true;
        continueHint.SetActive(false);

        foreach (char c in text)
        {
            contentText.text += c;
            yield return new WaitForSeconds(charInterval);
        }

        isTyping = false;
        continueHint.SetActive(true);
    }
}
