using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueAccessor : MonoBehaviour
{

    public static DialogueRunner dialogueRunner;
    public static DialogueUI dialogueUI;

    // Start is called before the first frame update
    void Start()
    {
        dialogueRunner = GetComponentInChildren<DialogueRunner>();
        dialogueUI = GetComponentInChildren<DialogueUI>();
    }
}
