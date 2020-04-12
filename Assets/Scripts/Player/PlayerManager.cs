using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : HitManager
{

    public Animator animator;
    public int health = 100;

    public Healthbar healthbar;


    private void Start()
    {
        healthbar.SetMaxHealth(health);
        healthbar.SetHealth(health);
    }

    public override void TakeDamage(int damage, Vector2 attackerPos, float knockBack)
    {
        health -= damage;
        healthbar.SetHealth(health);

        animator.SetTrigger("isHit");

        if (health <= 0)
        {
            //Kill the player / game over state
        }
    }
}
