using UnityEngine;
using UnityEngine.UI;

public class ChapterMenuUI : MonoBehaviour
{
    [Header("資料來源")]
    public ChapterList chapterList;               // 包含所有章節的 ScriptableObject
    [Header("UI 連結")]
    public GameObject buttonPrefab;               // 一個章節用的按鈕 prefab
    public Transform buttonParent;
    [Header("UI 面板控制")]
    public GameObject chapterMenuPanel;           // 章節面板（本身）
    public GameObject dialoguePanel;              // 對話面板（播劇情用）// 放置所有章節按鈕的容器
    [Header("對話播放器")]
    public DialoguePlayer dialoguePlayer;         // 控制劇情播放

    private void Start()
    {
        foreach (var chapter in chapterList.chapters)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonParent);
            var btn = btnObj.GetComponent<Button>();
            // 設定按鈕文字
            btn.GetComponentInChildren<Text>().text = chapter.chapterTitle;

            btn.onClick.AddListener(() =>
            {
                UIManager.Instance.ShowUI(UIType.Gaming);
                dialoguePlayer.PlaySequence(chapter.startingSequence);
            });
        }
        GameObject returnButton = Instantiate(buttonPrefab, buttonParent);
        var rebtn = returnButton.GetComponent<Button>();
        // 設定按鈕文字
        rebtn.GetComponentInChildren<Text>().text = "回主選單";
        rebtn.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowUI(UIType.MainMenu);
        });
    }
}
