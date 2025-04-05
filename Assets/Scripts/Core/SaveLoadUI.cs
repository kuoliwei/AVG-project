using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// 控制 Save / Load UI 面板，支援儲存或讀取不同槽位
/// </summary>
public class SaveLoadUI : MonoBehaviour
{
    public enum Mode { Save, Load } // 操作模式

    [Header("UI 元件")]
    public TMP_Text titleText;
    public Button[] slotButtons; // 連接 Slot0、Slot1、Slot2 三顆按鈕
    public TMP_Text[] slotTexts; // 顯示每槽的資訊文字（章節名稱 + 時間）
    public Button[] deleteButtons; // 長度需與 slotButtons 一樣
    public Button returnButton;

    [Header("設定")]
    public Mode currentMode = Mode.Save;
    public void Init()
    {
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int index = i;
            slotButtons[i].onClick.AddListener(() => OnSlotClicked(index));
        }

        for (int i = 0; i < deleteButtons.Length; i++)
        {
            int index = i;
            deleteButtons[i].onClick.AddListener(() => OnDeleteClicked(index));
        }
    }
    /// <summary>
    /// 根據模式刷新 UI 標題與每個存檔資訊
    /// </summary>
    public void RefreshUI()
    {
        titleText.text = (currentMode == Mode.Save) ? "選擇儲存槽" : "選擇讀取槽";

        for (int i = 0; i < slotTexts.Length; i++)
        {
            if (SaveManager.Instance.HasSaveInSlot(i))
            {
                var data = SaveManager.Instance.LoadFromSlot(i);
                slotTexts[i].text = $"{data.displayName}\n{data.savedAt}";
            }
            else
            {
                slotTexts[i].text = "<空的存檔槽>";
            }
        }
    }
    public DialoguePlayer dialoguePlayer;
    public MainMenuUI mainMenuUI;
    /// <summary>
    /// 點擊某個槽位按鈕時的行為
    /// </summary>
    private void OnSlotClicked(int slot)
    {
        if (currentMode == Mode.Save)
        {
            // 存檔：你可以從目前 DialoguePlayer 中抓資訊傳入
            DialoguePlayer player = dialoguePlayer;
            if (player != null && player.CurrentSequence != null)
            {
                string sequenceName = player.CurrentSequence.sequenceName;
                int index = player.CurrentLineIndex;
                string display = sequenceName; // TODO：可傳入真實描述

                SaveManager.Instance.SaveToSlot(slot, sequenceName, index, display);
                Debug.Log(slot+","+ sequenceName + "," + index + "," + display);
                RefreshUI();
            }
        }
        else if (currentMode == Mode.Load)
        {
            // 讀檔：從該槽載入，並指派給 DialoguePlayer 播放
            var data = SaveManager.Instance.LoadFromSlot(slot);
            if (data != null)
            {
                DialoguePlayer player = dialoguePlayer;
                if (player != null)
                {
                    dialoguePlayer.LoadSequenceByName(data.sequenceName, data.lineIndex);
                    Debug.Log(slot + "," + data.sequenceName + "," + data.lineIndex);
                    UIManager.Instance.ShowUI(UIType.Gaming); // <-- 替代原來的 SetActive(false)
                }
            }
        }
    }
    public void Show(Mode mode)
    {
        currentMode = mode;
        RefreshUI();
        UIManager.Instance.ShowUI(UIType.SaveLoad);
    }
    /// <summary>
    /// 點擊某個刪除按鈕時
    /// </summary>
    private void OnDeleteClicked(int slot)
    {
        bool hasSave = SaveManager.Instance.HasSaveInSlot(slot);
        if (!hasSave)
        {
            //Debug.Log("此槽已為空");
            return;
        }

        // 這裡可加彈窗確認（略）
        SaveManager.Instance.DeleteSlot(slot);
        RefreshUI(); // 更新畫面
    }
    public void SetReturnTarget(UIType target)
    {
        returnButton.onClick.RemoveAllListeners();
        returnButton.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowUI(target);
        });
    }
}
