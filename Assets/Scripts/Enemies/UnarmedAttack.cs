using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnarmedAttack : AbstractAttack
{
    [SerializeField] Animator animator;
    [SerializeField] ParticleSystem ps;
    [SerializeField] Transform gfxPos;

    public override void ExecuteAttack()
    {
        UnarmedEnemy em = (UnarmedEnemy)enemy;

        if (timeSinceAttack <= 0)
        {
            animator.SetTrigger(Constants.ATTACK_ANIM);

            //Delay hitscan by .5 seconds
            StartCoroutine(Util.ExecuteAfterTime(em.attackDuration, () =>
            {
                //Collect things to hit on layer
                Collider2D[] charactersToDamage = Physics2D.OverlapCircleAll(attackPos.position, em.range * enemy.sizeModifier, layer);

                for (int i = 0; i < charactersToDamage.Length; i++)
                {
                    ParticleSystem attPS = Instantiate(ps, RoomSpawner.instance.getCurrentRoom().transform);
                    attPS.transform.position = gfxPos.position;
                    attPS.startColor = em.color;
                    attPS.Play();
                    charactersToDamage[i].GetComponent<HitManager>().TakeDamage(em.damage, attackPos.position, em.knockback, false);
                }
            }));

            timeSinceAttack = em.delay * enemy.attackSpeedModifier;
        }
    }
}
