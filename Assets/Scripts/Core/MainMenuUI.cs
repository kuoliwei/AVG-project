using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public DialoguePlayer dialoguePlayer;
    public SaveLoadUI saveLoadUI;

    private void Start()
    {
        // 一開始顯示主選單
        UIManager.Instance.ShowUI(UIType.MainMenu);
        saveLoadUI.Init();
    }

    public void OnStartButton()
    {
        UIManager.Instance.ShowUI(UIType.Gaming);

        if (dialoguePlayer != null && dialoguePlayer.testSequence != null)
        {
            dialoguePlayer.PlaySequence(dialoguePlayer.testSequence);
        }
    }

    public void OnChapterButton()
    {
        UIManager.Instance.ShowUI(UIType.ChapterMenu);
    }

    public void OnOptionsButton()
    {
        Debug.Log("尚未實作選項功能");
        // 若未來實作 Options UI，可加 UIType.Options
    }

    public void OnQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void OnLoadClicked()
    {
        saveLoadUI.SetReturnTarget(UIType.MainMenu);
        saveLoadUI.Show(SaveLoadUI.Mode.Load);
    }
}
