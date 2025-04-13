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
    [Header("功能控制元件")]
    public ChapterMenuUI chapterMenuUI;    // <-- 連結 ChapterMenuUI 腳本（新增）

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
    /// <summary>
    /// 顯示指定 UI 面板，並隱藏其他面板
    /// </summary>
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
                // 每次顯示章節面板時，刷新解鎖狀態
                if (chapterMenuUI != null)
                {
                    chapterMenuUI.RefreshUI();
                    Debug.Log("RefreshUI by reopen chapterMenuPanel");
                }
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
    /// <summary>
    /// 判斷目前顯示中的 UI 是否為指定類型
    /// </summary>
    public bool IsCurrent(UIType ui)
    {
        return CurrentUI == ui;
    }
}
