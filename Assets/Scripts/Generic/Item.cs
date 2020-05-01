using System;
using System.Net.Http.Headers;
using UnityEngine;

public class Item
{

    public string id { get; set; }
    public Guid uuid { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public ItemType itemType { get; set; }
    public Sprite sprite { get; set; }

    public Item(string id, string name, string description, ItemType itemType, Sprite sprite)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.itemType = itemType;
        this.sprite = sprite;
        this.uuid = Guid.NewGuid();
    }

    //What behaviour should be when item is 'used'
    //TODO add context? -> shops
    public void Use()
    {
        switch (itemType)
        {
            case ItemType.WEAPON:
                //Swap to weapon hotbar
                
                break;
            case ItemType.POTION:
                //Consume
                break;
            case ItemType.MATERIAL:
                //?
                break;
            default:
                break;
        }
    }

    public override string ToString()
    {
        return $"id={id}, name={name}, desc={description}, type={itemType}, spritename={sprite.name}" ;
    }

    //Check items uuid to make sure its the right one
    public override bool Equals(object obj)
    {
        Item item = obj as Item;

        if(item == null)
        {
            return false;
        }

        return this.uuid == item.uuid;
      
    }

    public override int GetHashCode()
    {
        return this.uuid.GetHashCode();
    }
}

public enum ItemType
{
    WEAPON,
    POTION,
    MATERIAL
}
