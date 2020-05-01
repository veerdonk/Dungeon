using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractAttack : MonoBehaviour
{

    public float timeSinceAttack;

    public Transform attackPos;
    public LayerMask layer;

    public Animator WeaponAnimator;
    public SpriteRenderer weaponRenderer;


    public Dictionary<string, object> weaponStats;


    // Start is called before the first frame update
    void Start()
    {
        attackPos = GetComponentInParent<Transform>();
        UpdateWeaponStats(weaponRenderer.sprite.name);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeSinceAttack -= Time.deltaTime;
    }

    public void UpdateWeaponStats(string weaponName)
    {
        weaponStats = Constants.weaponNameToStats[weaponName];
        WeaponAnimator.speed = (float)weaponStats[Constants.PARAM_ANIM_SPEED];
    }

    public void Attack()
    {
        ExecuteAttack();
    }

    public abstract void ExecuteAttack();
}
