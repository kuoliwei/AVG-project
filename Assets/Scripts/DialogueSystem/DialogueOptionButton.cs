using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// DialogueOptionButton 控制單一選項按鈕的顯示與點擊事件
/// </summary>
public class DialogueOptionButton : MonoBehaviour
{
    public Text labelText; // 顯示的選項文字
    private DialogueOption option;// 儲存這個按鈕對應的資料

    /// <summary>
    /// 初始化按鈕，設定顯示內容與點擊行為
    /// </summary>
    public void Setup(DialogueOption optionData, System.Action<DialogueOption> onClick)
    {
        option = optionData;
        labelText.text = option.optionText;

        GetComponent<Button>().onClick.AddListener(() => onClick?.Invoke(option));
    }
}
