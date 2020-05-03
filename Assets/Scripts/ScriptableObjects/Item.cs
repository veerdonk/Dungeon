using System;
using System.Net.Http.Headers;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public abstract class Item : ScriptableObject
{
    [SerializeField]
    public string id;
    [SerializeField]
    public Guid uuid = new Guid();
    [SerializeField]
    public new string name;
    [SerializeField]
    public string description;
    [SerializeField]
    public ItemType itemType;
    [SerializeField]
    public Sprite sprite;
    public Rarity rarity;

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

        return this.id == item.id;
      
    }

    public override int GetHashCode()
    {
        return this.id.GetHashCode();
    }
}

public enum ItemType
{
    WEAPON,
    POTION,
    MATERIAL
}

public enum Rarity
{
    WHITE,
    GREEN
}