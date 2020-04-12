using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{

    private Inventory inventory;
    public GameObject itemIcon;
    public bool isWeapon;

    public bool playerOverPickup = false;
    private Collider2D other;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(Constants.PICKUP_WEAPON_KEY))
        {
            PickupItem();
        }   
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        playerOverPickup = true;
        other = collider;


    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerOverPickup = false;
    }

    public void PickupItem()
    {
        if (other.CompareTag("Player") && isWeapon)
        {

            int? slotToAdd = null;

            for (int i = 0; i < inventory.weaponslots.Length; i++)
            {
                //try to finf an empty slot
                if (inventory.weaponSlotsFull[i] == false)
                {
                    slotToAdd = i;

                    break;
                }
            }
            //If no free slot exists
            if (slotToAdd == null)
            {
                slotToAdd = inventory.currentSelectedSlot;
                Instantiate(
                    Resources.Load(
                        Constants.PREFABS_FOLDER +
                        Constants.PICKUPS_FOLDER +
                        inventory.GetCurrentWeaponName() +
                        Constants.PICKUP_SUFFIX));
            }

            //TODO check slot and empty out icon

            inventory.weaponSlotsFull[(int)slotToAdd] = true;
            

            Instantiate(itemIcon, inventory.weaponslots[(int)slotToAdd].transform, false);

            Destroy(gameObject);
        }
    }
}
