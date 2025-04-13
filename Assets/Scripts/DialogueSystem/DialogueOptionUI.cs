using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;

/// <summary>
/// 控制整個選項列表的 UI 生成與清除
/// </summary>
public class DialogueOptionUI : MonoBehaviour
{
    public Text questionText;
    public GameObject optionButtonPrefab; // 指向 OptionButton 預製件
    public Transform optionContainer;     // 放選項按鈕的容器（通常就是 OptionPanel）

    private List<GameObject> currentButtons = new List<GameObject>();
    public DialogueUIAnimator uiAnimator; // 從外部注入
    /// <summary>
    /// 顯示多個選項按鈕
    /// </summary>
    public void ShowOptions(DialogueBranch branch, System.Action<DialogueOption> onOptionSelected)
    {
        ClearOptions();
        // 顯示提問內容
        if (questionText != null)
        {
            questionText.text = branch.question;
            questionText.transform.parent.gameObject.SetActive(true);
        }
        for (int i = 0; i < branch.options.Count; i++)
        {
            GameObject buttonObj = Instantiate(optionButtonPrefab, optionContainer);
            var btn = buttonObj.GetComponent<DialogueOptionButton>();
            btn.Setup(branch.options[i], onOptionSelected);
            currentButtons.Add(buttonObj);

            if (uiAnimator != null)
            {
                uiAnimator.PrepareOptionButton(buttonObj, i); // 實作彈出 + Hover 動畫註冊
            }
        }
    }

    /// <summary>
    /// 移除現有按鈕（例如切換劇情後）
    /// </summary>
    public void ClearOptions()
    {
        foreach (var obj in currentButtons)
        {
            Destroy(obj);
        }
        currentButtons.Clear();
        if (questionText != null)
        {
            questionText.text = "";
            questionText.transform.parent.gameObject.SetActive(false);
        }
    }
}
