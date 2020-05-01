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
        itemName = gameObject.GetComponent<SpriteRenderer>().sprite.name;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constants.PLAYER_TAG))
        {
            if (inventory.AddItem(Constants.items[itemName]))
            {
                Destroy(gameObject);
            }
        }
        
    }

}
