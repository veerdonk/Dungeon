using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public Item item;
    private Inventory inventory;
    private string itemName;

    private void Start()
    {
        inventory = Inventory.instance;
        
        //When pickup is created it can be done with either an item or a sprite + name
        if (item == null)
        {
            itemName = gameObject.GetComponent<SpriteRenderer>().sprite.name;
            foreach (Item item in inventory.allItemObjects)
            {
                if (item.id == itemName)
                {
                    this.item = item;
                    break;
                }
            }
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = item.sprite;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constants.PLAYER_TAG))
        {
            if (inventory.AddItem(item))
            {
                Destroy(gameObject);
            }
        }
        
    }

}
