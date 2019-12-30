using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField]
    private Dialogue m_dialogue = new Dialogue();

    public void TriggerDialogue()
    {
        DialogueManager.instance.StartDialogue(m_dialogue);
    }
}
