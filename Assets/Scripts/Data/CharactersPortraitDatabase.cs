using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharactersPortraitDatabase", menuName = "AVG/Characters Portrait Database")]
public class CharactersPortraitDatabase : ScriptableObject
{
    public List<CharactersPortraitData> CharactersPortraits = new List<CharactersPortraitData>();

    public Sprite GetCharactersPortraitByKey(string key)
    {
        foreach (var portrait in CharactersPortraits)
        {
            if (portrait.key == key)
                return portrait.CharactersPortrait;
        }

        Debug.LogWarning("Characters Portrait not found for key: " + key);
        return null;
    }
}
