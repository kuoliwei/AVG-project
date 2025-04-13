using System.IO;
using UnityEngine;

public static class ChapterProgressStorage
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "chapter_progress.json");

    public static void Save(ChapterProgressData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static ChapterProgressData Load()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<ChapterProgressData>(json);
        }
        else
        {
            // 檔案不存在，建立初始資料（預設解鎖第一章）
            ChapterProgressData newData = new ChapterProgressData();
            newData.unlockedChapterIds.Add("chapter_1");

            // 儲存到硬碟
            Save(newData);

            //Debug.Log("初次建立 chapter_progress.json，已預設解鎖 chapter_1");

            return newData;
        }
    }
}
