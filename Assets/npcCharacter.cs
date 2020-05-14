using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class npcCharacter : MonoBehaviour
{
    public string startNode;
    public GameObject text;
    bool playerIsNear = false;
    bool dialogStarted = false;
    DialogueRunner dr;
    DialogueUI du;

    private void Start()
    {
        dr = DialogueAccessor.dialogueRunner;
        du = DialogueAccessor.dialogueUI;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerIsNear && dialogStarted)
        {
            du.MarkLineComplete();
        }

        if(Input.GetKeyDown(KeyCode.Mouse0) && playerIsNear && dialogStarted)
        {
            du.MarkLineComplete();
        }

        if (Input.GetKeyDown(KeyCode.E) && playerIsNear && !dialogStarted)
        {
            if (startNode != null) {
                dr.startNode = startNode;
            }
            dr.StartDialogue();
            dialogStarted = true;
            PlayerController2D.instance.allowedToProcess = false;
        }


    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constants.PLAYER_TAG))
        {
            playerIsNear = true;
            text.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Constants.PLAYER_TAG))
        {
            playerIsNear = false;
            text.SetActive(false);
            dialogStarted = false;
        }
    }
}
