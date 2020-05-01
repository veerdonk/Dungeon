using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : AbstractAttack
{

    //private float timeSinceAttack;

    //Transform attackPos;
    //public LayerMask layer;
    
    //public Animator WeaponAnimator;
    //public SpriteRenderer weaponRenderer;
    
    
    //Dictionary<string, object> weaponStats;


    // Start is called before the first frame update
    //void Start()
    //{
    //    attackPos = GetComponentInParent<Transform>();
    //    UpdateWeaponStats(weaponRenderer.sprite.name);
    //}

    //// Update is called once per frame
    //void FixedUpdate()
    //{

    //    timeSinceAttack -= Time.deltaTime;

    //}
    
    public override void ExecuteAttack()
    {

        if (timeSinceAttack <= 0)
        {

            //Collect things to hit on layer
            Collider2D[] charactersToDamage = Physics2D.OverlapCircleAll(attackPos.position, (float)weaponStats[Constants.PARAM_RANGE], layer);

            for (int i = 0; i < charactersToDamage.Length; i++)
            {
                charactersToDamage[i].GetComponent<HitManager>().TakeDamage(Convert.ToInt32(weaponStats[Constants.PARAM_DAMAGE]), attackPos.position, (float)weaponStats[Constants.PARAM_KNOCKBACK]);
            }

            WeaponAnimator.SetTrigger("Attack");

            timeSinceAttack = (float)weaponStats[Constants.PARAM_DELAY];
        }

    }

    //public void UpdateWeaponStats(string weaponName)
    //{
    //    weaponStats = Constants.weaponNameToStats[weaponName];
    //    WeaponAnimator.speed = (float)weaponStats[Constants.PARAM_ANIM_SPEED];
    //}

}
