using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : AbstractAttack
{
   
    public override void ExecuteAttack()
    {

        if (timeSinceAttack <= 0)
        {
            WeaponAnimator.SetTrigger("Attack");

            //Delay hitscan by .5 seconds
            StartCoroutine(Util.ExecuteAfterTime(0.5f, () =>
            {
                //Collect things to hit on layer
                Collider2D[] charactersToDamage = Physics2D.OverlapCircleAll(attackPos.position, weapon.range, layer);

                for (int i = 0; i < charactersToDamage.Length; i++)
                {
                    charactersToDamage[i].GetComponent<HitManager>().TakeDamage(weapon.damage, attackPos.position, weapon.knockback);
                }
            }));

            timeSinceAttack = weapon.delay;
        }

    }

}
