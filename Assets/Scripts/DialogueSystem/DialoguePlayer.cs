using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

/// <summary>
/// DialoguePlayer 是對話播放控制器，負責根據 ScriptableObject（DialogueSequence）
/// 逐句播放對話，播放完後檢查是否有分支選項，並切換狀態。
/// 採用狀態機（State Machine）設計模式來管理對話流程狀態。
/// </summary>
public class DialoguePlayer : MonoBehaviour
{
    [Header("測試用劇情")]
    public DialogueSequence testSequence; // 可直接從 Inspector 拖進來

    private void Start()
    {
        //LoadAllSequences(); // 自動從 Resources 資料夾載入全部劇情段
        //LoadFromSave(); // 根據 json 存檔內容跳到對應段落 / 句子
        //if (currentSequence == null && testSequence != null)
        //{
        //    PlaySequence(testSequence); // 開始播放指定的劇情
        //}
    }

    // 當前播放的對話資料（ScriptableObject 資料結構）
    // 封裝：資料來自外部，不在這裡寫死，而是透過參數注入
    private DialogueSequence currentSequence;

    // 播放進度：目前播放到第幾行對話
    private int currentLineIndex = 0;

    /// <summary>
    /// 播放流程的所有狀態
    /// 使用狀態機設計模式（State Machine Pattern）
    /// 將流程明確分為幾個階段，讓控制邏輯簡潔、可維護
    /// </summary>
    private enum DialogueState
    {
        Idle,           // 尚未播放任何對話
        Playing,        // 正在播放對話內容
        WaitingInput,   // 等待玩家輸入（例如按空白鍵繼續）
        ShowingBranch   // 顯示選項中（對話已結束，準備跳轉）
    }

    // 當前播放狀態（狀態機核心成員）
    private DialogueState currentState = DialogueState.Idle;

    // 控制是否將內容輸出到 Console（方便開發階段除錯）
    // 此處先不連結 UI，改以 Debug.Log 方式顯示結果
    [Header("Console Output")]
    public bool autoPrintToConsole = true;
    private string currentBackgroundKey;
    /// <summary>
    /// 啟動播放指定的對話序列
    /// 此為對外介面（例如由 UI 呼叫）
    /// </summary>
    /// <param name="sequence">要播放的對話資料</param>
    public void PlaySequence(DialogueSequence sequence)
    {
        // 檢查資料是否合法，防止空參考錯誤
        if (sequence == null)
        {
            Debug.LogWarning("Dialogue sequence is null.");
            return;
        }

        // 設定目前對話為輸入的資料
        currentSequence = sequence;

        // 將播放進度重設為第一句
        currentLineIndex = 0;

        // 將狀態設定為播放中
        currentState = DialogueState.Playing;
        if (backgroundManager != null && !string.IsNullOrEmpty(currentSequence.backgroundKey) && currentSequence.backgroundKey != currentBackgroundKey)
        {
            backgroundManager.ChangeBackground(currentSequence.backgroundKey);
            currentBackgroundKey = currentSequence.backgroundKey;
        }
        // 等待指定時間後開始第一句
        StartCoroutine(DelayedPlay(currentSequence.sequenceStartDelay, PlayNextLine));
    }
    private IEnumerator DelayedPlay(float delay, Action function)
    {
        yield return new WaitForSeconds(delay);
        function.Invoke();
    }
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private DialogueOptionUI optionUI;
    //[SerializeField] private CGController cgController;
    [SerializeField] private DualCGController dualCGController;
    [SerializeField] private List<DialogueSequence> allSequences; // 所有段落可查詢
    [ContextMenu("Auto Load All Sequences")]
    public void LoadAllSequences()
    {
        allSequences = Resources.LoadAll<DialogueSequence>("Dialogue/Sequences").ToList();
    }
    public void LoadFromSave()
    {
        SaveData data = SaveManager.Instance.LoadProgress();
        if (data == null) return;

        DialogueSequence found = allSequences.Find(seq => seq.name == data.sequenceName);
        if (found != null)
        {
            currentSequence = found;
            currentLineIndex = data.lineIndex;
            currentState = DialogueState.Playing;
            PlayNextLine();
        }
    }
    [SerializeField] private BackgroundManager backgroundManager;
    /// <summary>
    /// 播放下一句對話，並進入等待輸入狀態
    /// 若已達最後一行，則轉入結束處理
    /// </summary>
    private void PlayNextLine()
    {
        // 檢查播放資料是否存在，或是否已達結尾
        if (currentSequence == null || currentLineIndex >= currentSequence.lines.Count)
        {
            EndSequence();
            return;
        }
        // 取得目前要播放的對話資料（封裝：從資料結構讀取）
        DialogueLine line = currentSequence.lines[currentLineIndex];
        // 播放前先儲存進度
        SaveManager.Instance.SaveProgress(currentSequence.name, currentLineIndex);
        if (backgroundManager != null && !string.IsNullOrEmpty(line.backgroundKeyOverride) && line.backgroundKeyOverride != currentBackgroundKey)
        {
            backgroundManager.ChangeBackground(line.backgroundKeyOverride);
            currentBackgroundKey = line.backgroundKeyOverride;
        }
        else
        {
            if (backgroundManager != null && !string.IsNullOrEmpty(currentSequence.backgroundKey) && currentSequence.backgroundKey != currentBackgroundKey)
            {
                backgroundManager.ChangeBackground(currentSequence.backgroundKey);
                currentBackgroundKey = currentSequence.backgroundKey;
            }
        }

        // 原本播放邏輯不變...
        // 角色立繪控制
        if (dualCGController != null && line.speaker != null)
        {
            if (line.clearCharacterCGBeforeLine)
                dualCGController.ClearAll();
            CharacterPosition otherSide = line.position == CharacterPosition.Left ? CharacterPosition.Right : CharacterPosition.Left;
            string portraitKeyToUse = line.charactersPortraitsKeyOverride != "none" ? line.charactersPortraitsKeyOverride : line.speaker.defaultPortraitKey;
            Debug.Log(line.speaker.defaultPortraitKey);
            dualCGController.ShowCharacter(line.speaker, true, line.position, portraitKeyToUse);
            dualCGController.DimAt(otherSide);
        }
        //if (cgController != null)
        //{
        //    if (line.backgroundOverride != null)
        //        cgController.ShowBackground(line.backgroundOverride);
        //}
        if (dialogueUI != null)
        {
            //StartCoroutine(DelayedPlay(line.lineStartDelay, () => dialogueUI.SetLine(line)));
            dialogueUI.Clear();
            dialogueUI.SetLine(line);
        }
        // 若啟用 Console 模擬輸出，則印出角色與對話內容
        if (autoPrintToConsole && line != null && line.speaker != null)
        {
            // 注意：這裡輸出僅使用 ASCII 字元，避免 Unicode 儲存錯誤
            Debug.Log("[Speaker]: " + line.speaker.name + " / [Content]: " + line.content);
        }

        // TODO：可在這裡播放語音、CG 切換、立繪等效果（資料導向）

        // 將狀態切換為「等待玩家輸入」
        currentState = DialogueState.WaitingInput;
    }
    private bool IsTyping => dialogueUI != null && dialogueUI.IsTyping;
    /// <summary>
    /// 玩家按下空白鍵（或點擊 UI）後，進入下一句對話
    /// </summary>
    public void Continue()
    {
        // 若目前不在等待狀態，則忽略此呼叫
        if (currentState != DialogueState.WaitingInput) return;
        // 如果正在打字中，則先強制顯示全文
        if (IsTyping)
        {
            dialogueUI.ShowFull();
            return;
        }
        // 將對話索引往下推進
        currentLineIndex++;
        // 切換回播放狀態，繼續播下一句
        currentState = DialogueState.Playing;
        PlayNextLine();
    }
    /// <summary>
    /// 當對話序列播放完畢時執行此方法
    /// 檢查是否有設定分支，若有則顯示選項，否則結束對話流程
    /// </summary>
    private void EndSequence()
    {
        if (dialogueUI != null)
            dialogueUI.Clear();
        // 若這段劇情有設定後續分支選項
        if (currentSequence.branchAfterSequence != null)
        {
            // 切換為選項分支狀態
            currentState = DialogueState.ShowingBranch;

            // 顯示選項問題與各選項（僅在 Console 輸出）
            Debug.Log("[Branch] Question: " + currentSequence.branchAfterSequence.question);

            foreach (var option in currentSequence.branchAfterSequence.options)
            {
                Debug.Log("[Option] " + option.optionText + " -> Next: " + option.nextSequence?.name);
            }

            // 等待玩家從 UI 選擇
            // 顯示選項 UI
            if (optionUI != null)
            {
                optionUI.ShowOptions(currentSequence.branchAfterSequence, SelectOption);
            }
        }
        else
        {
            // 沒有分支，對話流程正式結束
            currentState = DialogueState.Idle;
            Debug.Log("Dialogue sequence finished.");
        }

    }
    /// <summary>
    /// 當玩家從 UI 中選擇某個選項時，由外部呼叫此方法
    /// 將根據該選項指定的劇情路徑，繼續播放
    /// </summary>
    /// <param name="selectedOption">玩家選擇的選項</param>
    public void SelectOption(DialogueOption selectedOption)
    {
        // 確保狀態正確、資料不為 null
        if (currentState != DialogueState.ShowingBranch || selectedOption == null) return;

        Debug.Log("Selected option: " + selectedOption.optionText);

        // 清除舊選項 UI
        if (optionUI != null)
        {
            optionUI.ClearOptions();
        }
        // 重新開始播放選項對應的新序列
        PlaySequence(selectedOption.nextSequence);
    }
    /// <summary>
    /// 每幀執行一次，這裡用來偵測按鍵輸入（測試用）
    /// 正式版可用 UI 按鈕代替
    /// </summary>
    private void Update()
    {
        // 按下空白鍵時，推進對話
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Continue();
        }
    }
}
