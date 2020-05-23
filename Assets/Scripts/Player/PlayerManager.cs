using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : HitManager
{
    public static PlayerManager instance;
    public Animator animator;
    public int playerLevel = 1;
    public int expForNextLvl = 100;
    public ParticleSystem levelPS;

    public int maxHealth = 100;
    public int health = 100;
    public int exp = 0;
    
    public Healthbar healthbar;

    //TODO remove/move to other class?
    public int curMapX = 0;
    public int curMapY = 0;

    public delegate void StatsChanged();
    public event StatsChanged OnPlayerStatChange;

    public delegate void LevelIncreased(int level);
    public event LevelIncreased OnLevelIncrease;

    //Modifying stats
    public float defence = 0;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("PlayerManager is not a singleton");
        }
        instance = this;
    }

    private void Start()
    {
        healthbar.SetMaxHealth(health);
        healthbar.SetHealth(health);
    }

    public void gainExp(int amount, float delay = 0)
    {
        if (exp + amount >= expForNextLvl)
        {
            LevelUp();
        }

        //Delay adding gold/exp so it lines up with the effect
        StartCoroutine(Util.ExecuteAfterTime(delay, () =>
        {
            exp += amount;
            TriggerStatChange();
        }));


    }

    public void gainGold(int amount, float delay = 0)
    {
        //Delay adding gold/exp so it lines up with the effect
        StartCoroutine(Util.ExecuteAfterTime(delay, () =>
        {

            Inventory.instance.gold += amount;
            TriggerStatChange();

        }));
    }

    public override void TakeDamage(int damage, Vector2 attackerPos, float knockBack, bool isCrit)
    {
        if (!PlayerController2D.instance.isDashing)
        {

            if (health <= 0)
            {
                //Kill the player / game over state
                PlayerController2D.instance.dead = true;

            }

            //Reduce damage by defence
            damage = (int)(damage - ArmorSwitcher.instance.totalArmor);
            if(damage < 0)
            {
                damage = 0;
            }
            health -= damage;
            OnPlayerStatChange.Invoke();
            animator.SetTrigger("isHit");
            CameraShake.instance.ShakeCamera();
        }
    }

    public void subscribeToEnemy(EnemyManager em)
    {
        em.onEnemyDeath += OnEnemyKilled;
    }

    public void OnEnemyKilled(int gainedExp, int gainedGold, EnemyManager em)
    {

        gainGold(gainedGold, 0.8f);
        gainExp(gainedExp, 0.8f);

        //Unsubscribe since enemy will be dead
        em.onEnemyDeath -= OnEnemyKilled;
    }

    private void TriggerStatChange()
    {
        if (OnPlayerStatChange != null)
        {
            OnPlayerStatChange.Invoke();
        }
    }

    private void LevelUp()
    {
        //TODO display level
        //upgrade stats
        //Calculate next value better
        playerLevel++;
        exp = 0;
        maxHealth = (int)(maxHealth * 1.1f);
        health = maxHealth;
        expForNextLvl = (int)(expForNextLvl * 1.25);
        levelPS.Play();
        UIUpdater.instance.displayLevelUp = true;
        OnPlayerStatChange.Invoke();
        OnLevelIncrease.Invoke(playerLevel);
    }

}
