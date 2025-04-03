using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BackgroundDatabase", menuName = "AVG/Background Database")]
public class BackgroundDatabase : ScriptableObject
{
    public List<BackgroundData> backgrounds = new List<BackgroundData>();

    public Sprite GetBackgroundByKey(string key)
    {
        foreach (var bg in backgrounds)
        {
            if (bg.key == key)
                return bg.background;
        }

        Debug.LogWarning("Background not found for key: " + key);
        return null;
    }
}
