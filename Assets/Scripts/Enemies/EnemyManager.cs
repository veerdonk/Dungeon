﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : HitManager
{
    public Enemy enemy;
    public AbstractAttack AAttack;
    public LayerMask layer;
    public Animator enemyAnimator;
    public SpriteRenderer weaponRenderer;
    public int health = 100;
    public ParticleSystem HurtPS;

    Rigidbody2D rb;

    public float timeSinceLastHit = 0;
    public float staggerTime = 10f;

    public Healthbar healthbar;
    public ParticleSystem coinSplosion;
    public Transform attackPos;

    public delegate void DeathDelegate(int exp, int gold, EnemyManager em);
    public event DeathDelegate onEnemyDeath;

    public Vector3 enemyGridPosition;

    private void Start()
    {
        health = enemy.health;
        staggerTime = enemy.staggerTime;

        switch (enemy.race)
        {
           case EnemyRace.ORC:
                enemyAnimator.SetTrigger(Constants.ORC_ANIM);
                break;
            case EnemyRace.SKELETON:
                enemyAnimator.SetTrigger(Constants.SKELETON_ANIM);
                break;
            case EnemyRace.LIZARD_F:
                enemyAnimator.SetTrigger(Constants.LIZARD_F_ANIM);
                break;
            default:
                break;
        }

        transform.localScale = transform.localScale * enemy.sizeModifier;

        rb = gameObject.GetComponent<Rigidbody2D>();
        weaponRenderer.sprite = AAttack.weapon.sprite;
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
        PlayHurtPS();
        healthbar.gameObject.SetActive(true);

        health -= damage;

        healthbar.SetHealth(health);

        if(health <= 0)
        {
            Die();
        }
        //new Vector2((rb.position - attackerPos).normalized.x, 0)
        rb.AddForceAtPosition((rb.position - attackerPos).normalized * knockBack, attackerPos);

        //reset time since last hit
        timeSinceLastHit = staggerTime;

        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.red;
        StartCoroutine(WhiteColor());

    }


    //Destroy gameobject
    //TODO play death animation/effect
    private void Die()
    {
        GameObject newWeapon = (GameObject)Instantiate(Resources.Load(Constants.PREFABS_FOLDER + Constants.PICKUPS_FOLDER + "weapon_pickup"), transform.parent.parent);
        newWeapon.transform.position = transform.position;
        newWeapon.GetComponent<Pickup>().item = AAttack.weapon;

        //TODO change values to actual gold/exp
        onEnemyDeath.Invoke(enemy.expValue, enemy.coinValue, this);

        ParticleSystem cs = Instantiate(coinSplosion);
        cs.transform.position = transform.position;
        cs.emission.SetBursts(new[] { new ParticleSystem.Burst(0, enemy.coinValue) });
        cs.Play();

        Destroy(transform.gameObject);
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

    void PlayHurtPS()
    {
        ParticleSystem hurtPS = Instantiate(HurtPS);
        hurtPS.transform.position = transform.position;
        hurtPS.Play();
    }
}
