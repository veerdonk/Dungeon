using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : HitManager
{

    public LayerMask layer;

    public int health = 100;

    Rigidbody2D rb;

    public float timeSinceLastHit = 0;
    public float staggerTime = 10f;

    public Healthbar healthbar;

    public Transform attackPos;

    PlayerManager pm;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();

        healthbar.SetMaxHealth(health);
        healthbar.SetHealth(health);

        healthbar.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //increase time till stagger wears off
        timeSinceLastHit -= Time.deltaTime;


    }

    public override void TakeDamage(int damage, Vector2 attackerPos, float knockBack)
    {
        healthbar.gameObject.SetActive(true);

        health -= damage;

        healthbar.SetHealth(health);

        if(health <= 0)
        {
            Die();
        }
        
        rb.AddForceAtPosition(new Vector2((rb.position - attackerPos).normalized.x, 0) * knockBack, attackerPos);

        //reset time since last hit
        timeSinceLastHit = staggerTime;

        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.red;
        StartCoroutine(WhiteColor());

    }


    //Destroy gameobject
    //TODO play death animation/effect
    private void Die()
    {
        Destroy(transform.parent.gameObject);
        Destroy(healthbar.gameObject);
        Destroy(gameObject);
    }

    IEnumerator WhiteColor()
    {
        yield return new WaitForSeconds(0.3f);
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, 0.8f);
    }
}
