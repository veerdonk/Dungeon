using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Inventory : MonoBehaviour
{
    //Singleton
    public static Inventory instance;

    public Dictionary<Item, int> items = new Dictionary<Item, int>();
    public Dictionary<Item, int> hotbarItems = new Dictionary<Item, int>();

    public Item[] inventoryItemPositions = new Item[numInvSlots];
    public Item[] hotbarItemPositions = new Item[numHotbarSlots];

    public int selectedSlot = 0;

    public const int numInvSlots = 30;
    public const int numHotbarSlots = 3;

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public delegate void OnHotbarChanged();
    public OnHotbarChanged onHotbarChangedCallback;

    public InventoryUI inventoryUI;

    public int gold = 0;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("Inventory is not a singleton");
        }
        instance = this;

        //Set all positions to null instead of broken item objs
        for (int i = 0; i < inventoryItemPositions.Length; i++)
        {
            inventoryItemPositions[i] = null;
        }
        for (int i = 0; i < hotbarItemPositions.Length; i++)
        {
            hotbarItemPositions[i] = null;
        }

    }

    public bool HotbarContainsItem()
    {
        bool hasItem = false;
        for (int i = 0; i < hotbarItemPositions.Length; i++)
        {
            if (hotbarItemPositions[i] != null)
            {
                //Second check because setting to null doesnt always work
                if (hotbarItemPositions[i].name != null)
                {
                    hasItem = true;
                    break;
                }
            }
        }

        return hasItem;
    }

    public void SetSelectedSlot(int change)
    {
        if (HotbarContainsItem())
        {
            //Update slot and wrap around if necessary
            selectedSlot += change;

            if (selectedSlot >= numHotbarSlots)
            {
                selectedSlot = 0;
            }
            else if (selectedSlot < 0)
            {
                selectedSlot = numHotbarSlots - 1;
            }

            //If selected slot is null call method again
            if (hotbarItemPositions[selectedSlot] == null)
            {
                SetSelectedSlot(change);
            }
            //Make hotbar UI and slots update
            HotbarCallback();
        }
    }

    public bool AddToInventory(Item itemToAdd)
    {
        return AddToInventory(itemToAdd, null);
    }
    public bool AddToInventory(Item itemToAdd, int? number)
    {
        if(number == null)
        {
            number = 1;
        }

        if (items.Count < numInvSlots)
        {
            if (items.ContainsKey(itemToAdd))
            {
                items[itemToAdd] += (int)number;
            }
            else
            {
                items[itemToAdd] = (int)number;

                for (int i = 0; i < inventoryItemPositions.Length; i++)
                {
                    if(inventoryItemPositions[i] == null)
                    {
                        inventoryItemPositions[i] = itemToAdd;
                        break;
                    }
                }

            }

            if (onItemChangedCallback != null)
            {
                onItemChangedCallback.Invoke();
            }
            return true;
        }
        return false;
    }
    public bool AddToHotbar(Item itemToAdd)
    {
        return AddToHotbar(itemToAdd, null);
    }
    public bool AddToHotbar(Item itemToAdd, int? number)
    {

        if(number == null)
        {
            number = 1;
        }

        if (hotbarItems.Count < numHotbarSlots)
        {
            if (hotbarItems.ContainsKey(itemToAdd))
            {
                hotbarItems[itemToAdd] += (int)number;
            }
            else
            {
                hotbarItems[itemToAdd] = (int)number;
                for (int i = 0; i < hotbarItemPositions.Length; i++)
                {
                    if (hotbarItemPositions[i] == null)
                    {
                        hotbarItemPositions[i] = itemToAdd;
                        break;
                    }
                }
            }

            if (onHotbarChangedCallback != null)
            {
                onHotbarChangedCallback.Invoke();
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool AddItem(Item itemToAdd)
    {

        switch (itemToAdd.itemType)
        {
            case ItemType.WEAPON:

                if (!AddToHotbar(itemToAdd))
                {
                    return AddToInventory(itemToAdd);
                }
                else
                {
                    return true;
                }

            case ItemType.POTION:
            case ItemType.MATERIAL:
                return AddToInventory(itemToAdd);

            default:
                Debug.LogWarning($"No behaviour for ItemType {itemToAdd.itemType}");
                return false;
        }

    }

    public void switchBars(Item itemToSwitch, int number, SlotType type, bool isActiveItem)
    {

        switch (type)
        {
            case SlotType.INVENTORY:
                if (AddToHotbar(itemToSwitch, number))
                {
                    RemoveAllInInventory(itemToSwitch);
                }
                return;
            case SlotType.HOTBAR:

                if(AddToInventory(itemToSwitch, number))
                {
                    RemoveAllInHotbar(itemToSwitch);
                }
                return;
            default:
                Debug.LogWarning($"No behaviour for SlotType: {type}");
                break;
        }

    }


    //Only works for items in inventory
    public void RemoveItem(Item item)
    {

        if (items[item] <= 1)
        {
            RemoveAllInInventory(item);
        }
        else
        {
            items[item]--;
            InventoryCallback();
        }


    }

    public void RemoveAllInInventory(Item item)
    {
        items.Remove(item);
        inventoryItemPositions[Array.IndexOf(inventoryItemPositions, item)] = null;
        InventoryCallback();
    }

    public void InventoryCallback()
    {
        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }

    public bool hasWeapon()
    {
        return hotbarItems.Count > 0;
    }

    public void RemoveFromHotbar(Item item)
    {
        if (hotbarItems[item] <= 1)
        {
            RemoveAllInHotbar(item);
        }
        else
        {
            hotbarItems[item]--;
            HotbarCallback();
        }
    }

    public void HotbarCallback()
    {
        if (onHotbarChangedCallback != null)
        {
            onHotbarChangedCallback.Invoke();
        }
    }

    public void RemoveAllInHotbar(Item item)
    {
        hotbarItems.Remove(item);
        hotbarItemPositions[Array.IndexOf(hotbarItemPositions, item)] = null;
        
        HotbarCallback();
    }

    public void RemoveEquippedWeapon()
    {
        Item equipped = hotbarItemPositions[selectedSlot];
        
        if (hotbarItems[equipped] <= 1)
        {
            RemoveAllInHotbar(equipped);
            //If last item of stack is used automatically switch
            SetSelectedSlot(1);
        }
        else
        {
            hotbarItems[equipped]--;
            HotbarCallback();
        };

    }
}
