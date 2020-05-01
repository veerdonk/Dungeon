using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{

    public ChestType type;
    public ParticleSystem coinPS;
    public ParticleSystem spawnPS;
    public ParticleSystem lootSpawnPS;
    public Animator animator;
    public GameObject weaponPickupPrefab;

    public List<string> items;

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
            switch (type)
            {
                case ChestType.ITEM:

                    if (lootSpawnPS != null && isClosed)
                    {
                        //Spit out items one by one
                        animator.SetTrigger(triggerName);
                        StartCoroutine(Util.ExecuteAfterTime(0.8f, () =>
                        {

                            List<Vector3> lootLocations = RoomSpawner.instance.FindLocationsNearPoint(transform.position, items.Count);
                            List<GameObject> spawnedItems = new List<GameObject>();

                            for (int i = 0; i < items.Count; i++)
                            {
                                Debug.Log($"Spawning item #{i}: {items[i]} at position {lootLocations[i]}");

                                ParticleSystem lootEffect = Instantiate(lootSpawnPS);
                                lootEffect.transform.position = lootLocations[i];
                                lootEffect.Play();

                                GameObject spawnedLoot = Instantiate(weaponPickupPrefab, RoomSpawner.instance.getCurrentRoom().transform);
                                spawnedLoot.GetComponent<SpriteRenderer>().sprite = Constants.items[items[i]].sprite;
                                spawnedLoot.transform.position = lootLocations[i];
                                spawnedLoot.SetActive(false);
                                spawnedItems.Add(spawnedLoot);
                            }

                            StartCoroutine(Util.ExecuteAfterTime(0.05f, () =>
                            {

                                foreach (GameObject item in spawnedItems)
                                {
                                    item.SetActive(true);
                                }

                            }));

                        }));


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

                            isClosed = false;

                        }));

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