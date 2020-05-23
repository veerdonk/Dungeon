using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{

    public Image icon;
    public Button itemButton;
    public Button removeButton;
    public Text numbers;
    public Item item;
    public SlotType slotType;
    public Image itemContainerImage;
    public bool isSelectedSlot;

    private bool resetRot = false;

    private void Awake()
    {
        //Start out with empty slots
        ClearSlot();
    }

    public void AddItem(Item newItem, int number, bool displayRemove, SlotType slotType)
    {
        item = newItem;
        itemButton.interactable = true;
        icon.sprite = item.sprite;
        icon.enabled = true;
        this.slotType = slotType;
        numbers.text = number.ToString();
        
        if(newItem.itemType == ItemType.ARMOR)
        {
            ArmorPiece ap = (ArmorPiece)newItem;
            icon.color = ap.color;
            ResetRot();
        }

        if (newItem.itemType == ItemType.WEAPON)
        {
            Weapon weap = (Weapon)newItem;
            if(weap.type == WeaponType.TOME)
            {
                ResetRot();
            }
        }

        if (newItem.itemType != ItemType.ARMOR)
        {
            numbers.enabled = true;
        }
        if (displayRemove)
        {
            removeButton.interactable = true;
        }
    }

    void ResetRot()
    {
        icon.transform.eulerAngles = new Vector3(0, 0, 0);
        resetRot = true;
        icon.preserveAspect = true;
        icon.transform.localScale = new Vector3(1, 1, 1);
    }


    public void SetSelected()
    {
        isSelectedSlot = true;
        //Set alpha to 100%
        Color tempColor = itemContainerImage.color;
        tempColor.a = 1;
        itemContainerImage.color = tempColor;
    }

    public void SetUnselected()
    {
        isSelectedSlot = false;
        Color tempColor = itemContainerImage.color;
        tempColor.a = 0.4f;
        itemContainerImage.color = tempColor;
    }

    public void ClearSlot()
    {
        
        if (resetRot)
        {
            icon.transform.eulerAngles = new Vector3(0, 0, -45);
            resetRot = false;
            icon.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        }
        item = null;
        icon.sprite = null;
        icon.color = Color.white;
        icon.enabled = false;
        numbers.enabled = false;
        itemButton.interactable = false;
        removeButton.interactable = false;               
    }

    public void RemoveItemFromInventory()
    {
        Inventory.instance.RemoveItem(item);
        int currentNum = int.Parse(numbers.text);
        if(currentNum <= 1)
        {
            numbers.enabled = false;
        }
    }

    public void OnItemButton()
    {
        Inventory.instance.switchBars(item, int.Parse(numbers.text), slotType, isSelectedSlot);
    }
}
public enum SlotType
{
    INVENTORY,
    HOTBAR,
    ARMOR_HEAD,
    ARMOR_TORSO
}
