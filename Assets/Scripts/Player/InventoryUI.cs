using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryUI;
    public Transform slotsPanel;
    public Transform weaponPanel;
    public InventorySlot headArmorSlot;
    public InventorySlot torsoArmorSlot;
    Inventory inventory;
    public GameObject currentEquippedWeapon;
    public GameObject weaponPrefab;
    [SerializeField] private GameObject pauseContainer;

    // Start is called before the first frame update
    void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;
        inventory.onHotbarChangedCallback += UpdateHotbarUI;
        inventory.onHeadArmorChangedCallback += UpdateHeadArmorSlot;
        inventory.onTorsoArmorChangedCallback += UpdateTorsoArmorSlot;
    }

    private void Update()
    {
        if (Input.GetButtonDown(Constants.INVENTORY_BUTTON_TAG) && !pauseContainer.activeSelf)
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
            if (inventoryUI.activeSelf)
            {
                UpdateUI();
            }
        }
        if (!inventoryUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {

            pauseContainer.SetActive(!pauseContainer.activeSelf);
            if (pauseContainer.activeSelf)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
        if (inventoryUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }


    }


    void UpdateHeadArmorSlot()
    {
        if (inventory.headArmorSlot != null)
        {
            headArmorSlot.AddItem(inventory.headArmorSlot, 1, false, SlotType.ARMOR_HEAD);
            ArmorSwitcher.instance.headArmor = (ArmorPiece)inventory.headArmorSlot;
            ArmorSwitcher.instance.headArmor.equipped = true;
        }
        else
        {
            headArmorSlot.ClearSlot();
            ArmorSwitcher.instance.headArmor.equipped = false;
        }
    }

    void UpdateTorsoArmorSlot()
    {
        if (inventory.torsoArmorSlot != null)
        {
            torsoArmorSlot.AddItem(inventory.torsoArmorSlot, 1, false, SlotType.ARMOR_TORSO);
            ArmorSwitcher.instance.torsoArmor = (ArmorPiece)inventory.torsoArmorSlot;
            ArmorSwitcher.instance.torsoArmor.equipped = true;
        }
        else
        {
            torsoArmorSlot.ClearSlot();
            ArmorSwitcher.instance.torsoArmor.equipped = false;
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

    //IMPROVEMENT -> get item/weapon pooling
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
                AddColliders(hotbarSlots[inventory.selectedSlot].item.id, currentEquippedWeapon);
                //currentEquippedWeapon.GetComponentInChildren<SpriteRenderer>().sprite = hotbarSlots[inventory.selectedSlot].item.sprite;
            }
            else
            {
                //TODO refactor to get rid of code duplication
                Destroy(currentEquippedWeapon);
                currentEquippedWeapon = (GameObject)Instantiate(weaponPrefab, GameObject.FindGameObjectWithTag(Constants.PLAYER_TAG).transform);
                currentEquippedWeapon.GetComponent<Throw>().weapon = (Weapon)hotbarSlots[inventory.selectedSlot].item;
                AddColliders(hotbarSlots[inventory.selectedSlot].item.id, currentEquippedWeapon);
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

    void AddColliders(string id, GameObject obj)
    {
        PolygonCollider2D colliderToUpdate = obj.GetComponentInChildren<PolygonCollider2D>();
        PolygonCollider2D polygoncollider = inventory.weaponsColliders[id];
        colliderToUpdate.pathCount = polygoncollider.pathCount;
        for (int p = 0; p < polygoncollider.pathCount; p++)
        {
            colliderToUpdate.SetPath(p, polygoncollider.GetPath(p));
        }
    }

}
