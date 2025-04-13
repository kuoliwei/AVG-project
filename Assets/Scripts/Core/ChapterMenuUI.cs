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

    /// <summary>
    /// 初始時建立一次章節按鈕，預設用於第一次開啟場景時
    /// </summary>
    private void Start()
    {
        RefreshUI();
    }

    /// <summary>
    /// 重新建立所有章節按鈕（可多次呼叫，例如從遊戲中回來時刷新 UI）
    /// </summary>
    public void RefreshUI()
    {
        // 清除所有原本已產生的按鈕（避免重複顯示）
        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }

        // 根據章節清單，逐一產生章節按鈕
        foreach (var chapter in chapterList.chapters)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonParent); // 產生按鈕
            var btn = btnObj.GetComponent<Button>();
            var txt = btn.GetComponentInChildren<Text>();

            txt.text = chapter.chapterTitle; // 顯示章節名稱（如「第一次見面」）

            // 根據是否解鎖，決定按鈕的互動性與顏色
            bool isUnlocked = ChapterProgress.Instance.IsChapterUnlocked(chapter.chapterId);
            if (!isUnlocked)
            {
                Debug.Log(chapter.chapterId + ", "+isUnlocked);
                txt.color = Color.gray;          // 灰色代表未解鎖
                btn.interactable = false;        // 禁止點擊
            }
            else
            {
                // 用 local 變數捕捉 chapter，避免 lambda 閉包錯誤
                ChapterData captured = chapter;

                btn.onClick.AddListener(() =>
                {
                    // 切換到遊戲中的對話 UI（可視為 Gaming 狀態）
                    UIManager.Instance.ShowUI(UIType.Gaming);

                    // 播放章節的起始對話
                    dialoguePlayer.PlaySequence(captured.startingSequence);

                    //Debug.Log("播放章節：" + captured.chapterId);
                });
            }
        }

        // 最後產生一個「回主選單」的按鈕
        GameObject returnButton = Instantiate(buttonPrefab, buttonParent);
        var rebtn = returnButton.GetComponent<Button>();
        var returnText = rebtn.GetComponentInChildren<Text>();
        returnText.text = "回主選單";

        rebtn.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowUI(UIType.MainMenu);
        });
    }
}
