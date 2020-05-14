using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject NPCPrefab;

    public void SpawnNPC(string startNode, Vector3 location)
    {
        GameObject newNPC = Instantiate(NPCPrefab, RoomSpawner.instance.getCurrentRoom().transform);
        newNPC.GetComponent<npcCharacter>().startNode = startNode;
        newNPC.transform.position = location;
    }
}
