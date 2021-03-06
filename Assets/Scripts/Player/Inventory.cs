﻿using System;
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
    public Item headArmorSlot = null;
    public Item torsoArmorSlot = null;

    public Item[] allItemObjects;
    public List<Item> whiteItems;
    public List<Item> greenItems;
    public List<Item> purpleItems;
    public List<Item> legendaryItems;

    Dictionary<WeaponType, Dictionary<Rarity, List<Weapon>>> typeAndWeaponSelect = new Dictionary<WeaponType, Dictionary<Rarity, List<Weapon>>>();

    public Dictionary<string, PolygonCollider2D> weaponsColliders = new Dictionary<string, PolygonCollider2D>();

    public int selectedSlot = 0;

    public const int numInvSlots = 30;
    public const int numHotbarSlots = 3;

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public delegate void OnHotbarChanged();
    public OnHotbarChanged onHotbarChangedCallback;

    public delegate void OnHeadArmorChanged();
    public OnHotbarChanged onHeadArmorChangedCallback;

    public delegate void OnTorsoArmorChanged();
    public OnHotbarChanged onTorsoArmorChangedCallback;

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
        allItemObjects = Resources.LoadAll<Item>("Weapons/");
    }

    private void Start()
    {
        foreach (Item item in allItemObjects)
        {
            switch (item.rarity)
            {
                case Rarity.COMMON:
                    whiteItems.Add(item);
                    break;
                case Rarity.RARE:
                    greenItems.Add(item);
                    break;
                case Rarity.EPIC:
                    purpleItems.Add(item);
                    break;
                case Rarity.LEGENDARY:
                    legendaryItems.Add(item);
                    break;
                default:
                    break;
            }

            if (item.itemType == ItemType.WEAPON)
            {
                Weapon weapon = (Weapon)item;
                if (typeAndWeaponSelect.ContainsKey(weapon.type))
                {
                    if (typeAndWeaponSelect[weapon.type].ContainsKey(weapon.rarity))
                    {
                        typeAndWeaponSelect[weapon.type][weapon.rarity].Add(weapon);
                    }
                    else
                    {
                        typeAndWeaponSelect[weapon.type][weapon.rarity] = new List<Weapon> { weapon };
                    }
                }
                else
                {
                    typeAndWeaponSelect[weapon.type] = new Dictionary<Rarity, List<Weapon>> { { weapon.rarity, new List<Weapon> { weapon } } };
                }

                if (weaponsColliders.ContainsKey(item.id))
                {
                    //Should not happen
                    Debug.LogWarning("Multiple colliders for item with id: " + item.id);
                }
                else
                {
                    weaponsColliders[item.id] = weapon.collider;
                }
            }
        }
    }

    public Weapon getRandomWeaponOfTypeAndRarity(WeaponType type, Rarity rarity)
    {
        if (typeAndWeaponSelect.ContainsKey(type))
        {
            if (typeAndWeaponSelect[type].ContainsKey(rarity))
            {
                return typeAndWeaponSelect[type][rarity][UnityEngine.Random.Range(0, typeAndWeaponSelect[type][rarity].Count)];
            }
        }
        return null;
    }

    public bool HotBarHasSpecificItem(Item item)
    {
        for (int i = 0; i < hotbarItemPositions.Length; i++)
        {
            if (hotbarItemPositions[i] == item)
            {
                return true;
            }
        }

        return false;
    }

    public bool InventoryHasSpecificItem(Item item)
    {
        for (int i = 0; i < inventoryItemPositions.Length; i++)
        {
            if (inventoryItemPositions[i] == item)
            {
                return true;
            }
        }

        return false;
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

    public bool InventoryContainsItem()
    {
        bool hasItem = false;
        for (int i = 0; i < inventoryItemPositions.Length; i++)
        {
            if (inventoryItemPositions[i] != null)
            {
                //Second check because setting to null doesnt always work
                if (inventoryItemPositions[i].name != null)
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
        else
        {
            selectedSlot = 0;
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
        if (items.ContainsKey(itemToAdd))
        {
            items[itemToAdd] += (int)number;
        }
        else if (items.Count < numInvSlots)
        {

            items[itemToAdd] = (int)number;

            for (int i = 0; i < inventoryItemPositions.Length; i++)
            {
                if (inventoryItemPositions[i] == null)
                {
                    inventoryItemPositions[i] = itemToAdd;
                    break;
                }
            }
        }
        else
        {
            return false;
        }

        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
        return true;
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

        if (hotbarItems.ContainsKey(itemToAdd))
        {
            hotbarItems[itemToAdd] += (int)number;
        }
        else if(hotbarItems.Count < numHotbarSlots)
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
        else
        {
            return false;
        }

        if (onHotbarChangedCallback != null)
        {
            onHotbarChangedCallback.Invoke();
        }
        return true;
    }

    public bool AddItem(Item itemToAdd)
    {

        switch (itemToAdd.itemType)
        {
            case ItemType.WEAPON:

                if (InventoryHasSpecificItem(itemToAdd))
                {
                    return AddToInventory(itemToAdd);
                }
                

                if (!AddToHotbar(itemToAdd))
                {
                    return AddToInventory(itemToAdd);
                }
                else
                {
                    return true;
                }

            case ItemType.ARMOR:
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

                if(itemToSwitch.itemType == ItemType.ARMOR)
                {
                    ArmorPiece armor = (ArmorPiece)itemToSwitch;
                    switch (armor.armorPart)
                    {
                        case ArmorPart.HEAD:
                            if(headArmorSlot == null)
                            {
                                headArmorSlot = armor;
                                onHeadArmorChangedCallback.Invoke();
                                RemoveAllInInventory(itemToSwitch);
                            }
                            break;
                        case ArmorPart.TORSO:
                            if (torsoArmorSlot == null)
                            {
                                torsoArmorSlot = armor;
                                onTorsoArmorChangedCallback.Invoke();
                                RemoveAllInInventory(itemToSwitch);
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                }

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
            case SlotType.ARMOR_HEAD:
                if (AddToInventory(itemToSwitch, 0))
                {
                    headArmorSlot = null;
                    onHeadArmorChangedCallback.Invoke();
                }
                return;

            case SlotType.ARMOR_TORSO:
                if (AddToInventory(itemToSwitch, 0))
                {
                    torsoArmorSlot = null;
                    onTorsoArmorChangedCallback.Invoke();
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
