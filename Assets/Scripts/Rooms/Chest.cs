﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{

    public ChestType type;
    public ParticleSystem coinPS;
    public ParticleSystem spawnPS;
    public ParticleSystem lootSpawnPS;
    public GameObject itemSpawnerPrefab;
    public Animator animator;
    public GameObject weaponPickupPrefab;

    public List<Item> items;

    bool isClosed = true;
    public int coinCount = 30;
    string triggerName;

    private void Awake()
    {
        if (isClosed)
        {
            animator.SetTrigger("IsClosed");
            spawnPS.Play();
        }
    }

    private void Start()
    {
        switch (type)
        {
            case ChestType.ITEM:
                triggerName = "Trigger_Item";
                break;
            case ChestType.COIN:
                triggerName = "Trigger_Money";
                break;
            case ChestType.MIMIC:
                triggerName = "Trigger_Mimic";
                break;
            default:
                Debug.LogWarning("Unknown chest type: " + type);
                break;
        }


    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constants.PLAYER_TAG))
        {
            if (isClosed)
            {
                //Play Chest opening sound
                AudioManager.instance.PlayOnce(Constants.CHEST_OPEN_SOUND);
            }
            switch (type)
            {
                case ChestType.ITEM:

                    if (lootSpawnPS != null && isClosed)
                    {
                        animator.SetTrigger(triggerName);

                        GameObject lootObj = Instantiate(itemSpawnerPrefab, RoomSpawner.instance.getCurrentRoom().transform);
                        lootObj.transform.position = transform.position;
                        lootObj.GetComponent<ParticleItemSpawner>().items = items;
                        lootObj.GetComponent<ParticleItemSpawner>().delay = 0.8f;

                        isClosed = false;
                    }
                    break;
                case ChestType.COIN:

                    if (coinPS != null && isClosed)
                    {
                        animator.SetTrigger(triggerName);

                        StartCoroutine(Util.ExecuteAfterTime(0.8f, () =>
                        {

                            //open chest and add stuff to player
                            coinPS.emission.SetBursts(new[] { new ParticleSystem.Burst(0, coinCount) });
                            coinPS.Play();

                            PlayerManager.instance.gainGold(coinCount);


                        }));
                        isClosed = false;
                    }
                    break;
                case ChestType.MIMIC:

                    //remove chest, spawn mimic

                    break;
                default:
                    break;
            }
        }
    }
}

public enum ChestType
{
    ITEM,
    COIN,
    MIMIC
}