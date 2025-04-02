using UnityEngine;
using UnityEngine.UI;

public class ChapterMenuUI : MonoBehaviour
{
    public ChapterList chapterList;
    public GameObject buttonPrefab;
    public Transform buttonParent;
    public DialoguePlayer dialoguePlayer;

    private void Start()
    {
        foreach (var chapter in chapterList.chapters)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonParent);
            var btn = btnObj.GetComponent<Button>();
            btn.GetComponentInChildren<Text>().text = chapter.chapterTitle;

            btn.onClick.AddListener(() =>
            {
                gameObject.SetActive(false); // 關閉選單
                dialoguePlayer.PlaySequence(chapter.startingSequence); // 播放章節
            });
        }
    }
}
