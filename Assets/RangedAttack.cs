using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : AbstractAttack
{
    public GameObject arrowPrefab;


    public override void ExecuteAttack()
    {

        if (timeSinceAttack < 0)
        {

            Debug.Log("Attacking");
            GameObject arrow = Instantiate(arrowPrefab, transform.parent);
            arrow.transform.position = attackPos.position;
            Arrow arrowC = arrow.GetComponent<Arrow>();
            Vector2 heading = PlayerManager.instance.transform.position - attackPos.position;
            float distance = heading.magnitude;
            arrowC.direction = heading / distance;
            arrowC.weapon = weapon;
            arrowC.shooter = gameObject;
            timeSinceAttack = weapon.delay;
        }
    }

}
