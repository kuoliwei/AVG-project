using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// DialogueUI 負責更新 UI 上的對話顯示內容
/// </summary>
public class DialogueUI : MonoBehaviour
{
    [Header("UI 元件")]
    public TextMeshProUGUI nameText;        // 顯示角色名稱
    public TextMeshProUGUI contentText;     // 顯示對話內容
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

        typingCoroutine = StartCoroutine(TypeText(fullText, line.lineStartDelay));
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

            contentText.text = StripTags(fullText);
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
    private IEnumerator TypeText(string text, float delay)
    {
        // 清空畫面上的文字
        contentText.text = "";

        // 標記目前是否正在打字中
        isTyping = true;

        // 關閉「按空白鍵繼續」提示
        continueHint.SetActive(false);

        // 進入打字前的延遲（來自 DialogueLine 的 lineStartDelay）
        yield return new WaitForSeconds(delay);

        int i = 0; // 目前的文字索引位置
        float currentSpeed = charInterval; // 當前每字間隔時間，可由 <speed=...> 動態調整

        // 用來累積目前畫面上應顯示的完整文字（含樣式標籤）
        System.Text.StringBuilder visibleText = new System.Text.StringBuilder();

        // 逐字處理輸入的原始 text 字串
        while (i < text.Length)
        {
            // --- 偵測是否遇到 < 開頭的標籤 ---
            if (text[i] == '<')
            {
                int tagEnd = text.IndexOf('>', i); // 尋找標籤結尾的位置

                // 若成功找到 '>'，則擷取整段標籤
                if (tagEnd != -1)
                {
                    string tag = text.Substring(i, tagEnd - i + 1); // 包含 < 和 >

                    // -------- 處理控制標籤：<wait=...> --------
                    if (tag.StartsWith("<wait="))
                    {
                        // 擷取中間的數字部分並轉為 float
                        if (float.TryParse(tag.Substring(6, tag.Length - 7), out float waitTime))
                        {
                            yield return new WaitForSeconds(waitTime); // 執行等待
                        }
                        i = tagEnd + 1; // 跳過整段標籤
                        continue;
                    }
                    // -------- 處理控制標籤：<speed=...> --------
                    else if (tag.StartsWith("<speed="))
                    {
                        // 擷取新速度數值，更新 currentSpeed
                        if (float.TryParse(tag.Substring(7, tag.Length - 8), out float newSpeed))
                        {
                            currentSpeed = newSpeed;
                        }
                        i = tagEnd + 1; // 跳過整段標籤
                        continue;
                    }
                    // 其他標籤一律視為樣式標籤 → 保留（包括 </b>、</color> 等）
                    visibleText.Append(tag); // 加入樣式標籤
                    contentText.text = visibleText.ToString(); // 更新畫面文字
                    i = tagEnd + 1; // 跳過整段標籤
                    continue;
                }
            }

            // --- 處理普通可見文字（非標籤） ---
            visibleText.Append(text[i]); // 累加一個字
            contentText.text = visibleText.ToString(); // 顯示目前的所有文字（含樣式）
            i++; // 文字索引往前推

            yield return new WaitForSeconds(currentSpeed); // 等待目前的速度設定
        }

        // 結束打字，開啟繼續提示
        isTyping = false;
        continueHint.SetActive(true);
    }

    // 移除文字中的所有 <...> 形式的標籤，用來避免打字中途跳出未處理的 wait/speed 等控制標籤
    private string StripTags(string input)
    {
        // 使用 StringBuilder 來累積處理過後的乾淨文字（效能比用 string 拼接好）
        System.Text.StringBuilder builder = new System.Text.StringBuilder();

        // 標記目前是否處於 <...> 標籤內部
        bool insideTag = false;

        System.Text.StringBuilder currentTag = new System.Text.StringBuilder();
        // 逐字掃描輸入文字
        foreach (char c in input)
        {
            // 如果遇到 <，代表一個新標籤開始
            if (c == '<')
            {
                insideTag = true;       // 設為標籤模式
                currentTag.Clear();     // 清空暫存標籤文字
                currentTag.Append(c);   // 加上 '<'
            }
            // 如果遇到 >，且目前在標籤中 → 標籤結束
            else if (c == '>' && insideTag)
            {
                currentTag.Append(c);   // 加上 '>'
                insideTag = false;      // 結束標籤模式

                string tagStr = currentTag.ToString(); // 取得整段完整的 <...>

                // 檢查是否為控制標籤（如 <wait=0.3>）
                if (!IsControlTag(tagStr))
                {
                    builder.Append(tagStr); // 如果不是控制標籤，保留它
                }
                // 若是控制標籤 → 不加入 builder，即為移除
            }
            // 標籤內部的其他字元，繼續累積
            else if (insideTag)
            {
                currentTag.Append(c);
            }
            // 不在標籤中 → 正常字元，直接加入 builder
            else
            {
                builder.Append(c);
            }
        }

        // 將處理後的結果字串回傳（已無 <...> 標籤）
        return builder.ToString();
    }
    // 判斷此 <...> 標籤是否為「控制用標籤」
    // 若是，將在 StripControlTags 中被移除
    private bool IsControlTag(string tag)
    {
        return tag.StartsWith("<wait=") || tag.StartsWith("<speed=");
    }
    public DialoguePlayer dialoguePlayer;
    /// <summary>
    /// 每幀執行一次，這裡用來偵測按鍵輸入（測試用）
    /// 正式版可用 UI 按鈕代替
    /// </summary>
    private void Update()
    {
        if (!UIManager.Instance.IsCurrent(UIType.Gaming)) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            dialoguePlayer.Continue();
        }
    }
    public SaveLoadUI saveLoadUI;
    public void OnSaveClicked()
    {
        saveLoadUI.SetReturnTarget(UIType.Gaming);
        saveLoadUI.Show(SaveLoadUI.Mode.Save);
    }

    public void OnLoadClicked()
    {
        saveLoadUI.SetReturnTarget(UIType.Gaming);
        saveLoadUI.Show(SaveLoadUI.Mode.Load);
    }
}
