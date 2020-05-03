using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public Item item;
    private Inventory inventory;

    private void Start()
    {
        inventory = Inventory.instance;
        gameObject.GetComponent<SpriteRenderer>().sprite = item.sprite;

    }

    public void SetStatic()
    {
        GetComponent<Animator>().SetTrigger("IsStatic");
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
