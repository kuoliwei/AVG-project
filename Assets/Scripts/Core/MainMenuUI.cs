using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject chapterMenuPanel;
    public GameObject dialoguePanel;
    public DialoguePlayer dialoguePlayer;
    private void Start()
    {
        mainMenuPanel.SetActive(true);
        chapterMenuPanel.SetActive(false);
        dialoguePanel.SetActive(false);
    }
    public void OnStartButton()
    {
        mainMenuPanel.SetActive(false);
        dialoguePanel.SetActive(true);
        // 播放預設劇情段落（testSequence）
        if (dialoguePlayer != null && dialoguePlayer.testSequence != null)
        {
            dialoguePlayer.PlaySequence(dialoguePlayer.testSequence);
        }
    }

    public void OnChapterButton()
    {
        mainMenuPanel.SetActive(false);
        chapterMenuPanel.SetActive(true);
    }

    public void OnOptionsButton()
    {
        Debug.Log("尚未實作選項功能");
    }

    public void OnQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
