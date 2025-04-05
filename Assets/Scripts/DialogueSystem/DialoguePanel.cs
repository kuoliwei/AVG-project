using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialoguePanel : MonoBehaviour, IPointerClickHandler
{
    public DialoguePlayer dialoguePlayer;
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (dialoguePlayer != null)
        {
            dialoguePlayer.Continue();
        }
        else
        {
            Debug.LogWarning("DialoguePlayer ¥¼«ü©w¡I");
        }
    }
}
