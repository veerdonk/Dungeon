﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractAttack : MonoBehaviour
{
    public Weapon weapon;
    public float timeSinceAttack;

    public Transform attackPos;
    public LayerMask layer;

    public Animator WeaponAnimator;
    public SpriteRenderer weaponRenderer;

    // Start is called before the first frame update
    void Start()
    {
        attackPos = GetComponentInParent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeSinceAttack -= Time.deltaTime;
    }

    public void Attack()
    {
        ExecuteAttack();
    }

    public abstract void ExecuteAttack();
}
