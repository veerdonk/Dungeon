﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class EnemyAI : MonoBehaviour
{
    public float forceSpeed = 300f;
    public float nextWaypointDistance = 3f;
    public float followDistance = 1f;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;
    Seeker seeker;
    Rigidbody2D rb;
    Animator animator;
    Transform target;

    bool seesTarget = false;

    // Start is called before the first frame update
    void Start()
    {

        target = GameObject.FindGameObjectWithTag("Player").transform;

        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, .5f);
        
        animator = gameObject.GetComponentInChildren<Animator>();
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        //check distance to player, if in range dont move
        if (Vector2.Distance(target.position, rb.position) > followDistance)
        { 

            if (path == null)
            {
                return;
            }

            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                return;
            }
            else
            {
                reachedEndOfPath = false;
            }

            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * forceSpeed * Time.deltaTime;

            rb.AddForce(force);
            animator.SetFloat("Speed", force.sqrMagnitude);

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            if (force.x > 0)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else if (force.x < 0)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }


        }
        else
        {
            animator.SetFloat("Speed", 0f);
            if (GetComponent<EnemyManager>().timeSinceLastHit <= 0)
            {
                GetComponent<Attack>().ExecuteAttack();
            }
        }
    }


}
