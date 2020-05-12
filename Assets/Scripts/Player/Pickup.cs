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

        if(item.itemType == ItemType.WEAPON)
        {
            Weapon weap = (Weapon)item;
            transform.localScale = transform.localScale * weap.sizeModifier;
        }
    }

    public void SetStatic(bool isStatic)
    {
        if (isStatic)
        {
            GetComponent<Animator>().SetTrigger("IsStatic");
        }
        else
        {
            GetComponent<Animator>().SetTrigger("pickup");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constants.PLAYER_TAG))
        {
            if (inventory.AddItem(item))
            {
                AudioManager.instance.PlayRandomOfType(SoundType.PICKUP);
                Destroy(gameObject);
            }
        }
        
    }

}
