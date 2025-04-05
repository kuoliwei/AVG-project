using UnityEngine;
public enum UIType
{
    None,
    MainMenu,
    ChapterMenu,
    Gaming,
    SaveLoad
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("主要 UI 面板")]
    public GameObject gamePanel;           // 新增 GamePanel（含 Dialogue & Option）
    public GameObject mainMenuPanel;
    public GameObject chapterMenuPanel;
    public GameObject saveLoadPanel;

    public UIType CurrentUI { get; private set; } = UIType.None;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void ShowUI(UIType ui)
    {
        // 全部關閉
        gamePanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        chapterMenuPanel.SetActive(false);
        saveLoadPanel.SetActive(false);

        // 開啟指定面板
        switch (ui)
        {
            case UIType.MainMenu:
                mainMenuPanel.SetActive(true);
                break;
            case UIType.ChapterMenu:
                chapterMenuPanel.SetActive(true);
                break;
            case UIType.Gaming:
                gamePanel.SetActive(true);
                break;
            case UIType.SaveLoad:
                saveLoadPanel.SetActive(true);
                break;
        }

        CurrentUI = ui;
    }

    public bool IsCurrent(UIType ui)
    {
        return CurrentUI == ui;
    }
}
