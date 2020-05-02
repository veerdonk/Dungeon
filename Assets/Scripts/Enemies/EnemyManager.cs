using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : HitManager
{
    public AbstractAttack AAttack;
    public LayerMask layer;

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

    public EnemyType type;

    private void Start()
    {
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
        int coinCount = 10;

        onEnemyDeath.Invoke(10, coinCount, this);

        ParticleSystem cs = Instantiate(coinSplosion);
        cs.transform.position = transform.position;
        cs.emission.SetBursts(new[] { new ParticleSystem.Burst(0, coinCount) });
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

public enum EnemyType
{
    MELEE,
    RANGED
}