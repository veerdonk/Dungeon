using System.Collections.Generic;
using UnityEngine;


public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryUI;
    public Transform slotsPanel;
    public Transform weaponPanel;
    Inventory inventory;
    public GameObject currentEquippedWeapon;
    public GameObject weaponPrefab;

    // Start is called before the first frame update
    void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;
        inventory.onHotbarChangedCallback += UpdateHotbarUI;
    }

    private void Update()
    {
        if (Input.GetButtonDown(Constants.INVENTORY_BUTTON_TAG))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
    }

    void UpdateUI()
    {
        
        InventorySlot[] itemSlots = slotsPanel.GetComponentsInChildren<InventorySlot>();

        for (int i = 0; i < inventory.inventoryItemPositions.Length; i++)
        {
            if (inventory.inventoryItemPositions[i] != null)
            {
                itemSlots[i].AddItem(inventory.inventoryItemPositions[i], inventory.items[inventory.inventoryItemPositions[i]], true, SlotType.INVENTORY);
            }
            else
            {
                itemSlots[i].ClearSlot();
            }
        }


    }

    void UpdateHotbarUI()
    {
      
        InventorySlot[] hotbarSlots = weaponPanel.GetComponentsInChildren<InventorySlot>();

        for (int i = 0; i < inventory.hotbarItemPositions.Length; i++)
        {
            //Unselect all slots
            hotbarSlots[i].SetUnselected();
            if (inventory.hotbarItemPositions[i] != null)
            {
                hotbarSlots[i].AddItem(inventory.hotbarItemPositions[i], inventory.hotbarItems[inventory.hotbarItemPositions[i]], false, SlotType.HOTBAR);
            }
            else
            {
                hotbarSlots[i].ClearSlot();
            }
        }
        //Select active slot
        hotbarSlots[inventory.selectedSlot].SetSelected();

        //Update the equipped weapon
        if (hotbarSlots[inventory.selectedSlot].item != null)
        {
            if (currentEquippedWeapon == null || currentEquippedWeapon.GetComponent<Throw>().isThrown)
            {
                currentEquippedWeapon = Instantiate(weaponPrefab, GameObject.FindGameObjectWithTag(Constants.PLAYER_TAG).transform);
                currentEquippedWeapon.GetComponent<Throw>().weapon = (Weapon)hotbarSlots[inventory.selectedSlot].item;
                //currentEquippedWeapon.GetComponentInChildren<SpriteRenderer>().sprite = hotbarSlots[inventory.selectedSlot].item.sprite;
            }
            else
            {
                //TODO refactor to get rid of code duplication
                Destroy(currentEquippedWeapon);
                currentEquippedWeapon = (GameObject)Instantiate(weaponPrefab, GameObject.FindGameObjectWithTag(Constants.PLAYER_TAG).transform);
                currentEquippedWeapon.GetComponent<Throw>().weapon = (Weapon)hotbarSlots[inventory.selectedSlot].item;
                //currentEquippedWeapon.GetComponentInChildren<SpriteRenderer>().sprite = hotbarSlots[inventory.selectedSlot].item.sprite;
            }
        }
        //no item in hotbar left but the player still has a version equipped!
        //Also make sure the weapon is still in hand so we dont destroy flying weapons
        else if(hotbarSlots[inventory.selectedSlot].item == null && currentEquippedWeapon != null && !currentEquippedWeapon.GetComponent<Throw>().isThrown)
        {
            Destroy(currentEquippedWeapon);
        }
    }
}
