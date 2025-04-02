using UnityEngine;
using System.IO;

/// <summary>
/// 控制儲存與讀取遊戲進度（單例）
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private string savePath => Path.Combine(Application.persistentDataPath, "save.json");

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 儲存進度至 JSON 檔案
    /// </summary>
    public void SaveProgress(string sequenceName, int lineIndex)
    {
        SaveData data = new SaveData
        {
            sequenceName = sequenceName,
            lineIndex = lineIndex
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Progress saved: " + savePath);
    }

    /// <summary>
    /// 載入進度，若無資料則回傳 null
    /// </summary>
    public SaveData LoadProgress()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("No save file found.");
            return null;
        }

        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<SaveData>(json);
    }
}
