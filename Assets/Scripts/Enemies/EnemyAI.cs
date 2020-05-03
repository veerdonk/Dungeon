using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

public class EnemyAI : MonoBehaviour
{
    public float forceSpeed = 300f;
    public float nextWaypointDistance = 3f;
    public float followDistance = 1f;
    public float minDistance = 0f;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;
    Seeker seeker;
    Rigidbody2D rb;
    Animator animator;
    Transform target;

    bool seesTarget = false;
    bool flipped = false;

    // Start is called before the first frame update
    void Start()
    {
        Enemy enemy = GetComponent<EnemyManager>().enemy;
        forceSpeed = enemy.speed;
        followDistance = enemy.followDistance;
        minDistance = enemy.minDistance;

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

            //float localScaleX = transform.localScale.x;
            if (force.x > 0 && flipped)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                flipped = false;
            }
            else if (force.x < 0 && !flipped)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                flipped = true;
            }


        }
        else if (Vector2.Distance(target.position, rb.position) < minDistance)
        {
            Vector2 direction = (transform.position - target.position).normalized;
            Vector2 force = direction * forceSpeed * Time.deltaTime;
            rb.AddForce(force);
            animator.SetFloat("Speed", force.sqrMagnitude);

            if (target.position.x > transform.position.x && flipped)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                flipped = false;
            }
            else if (target.position.x < transform.position.x && !flipped)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                flipped = true;
            }

        }

        else
        {
            if (target.position.x > transform.position.x && flipped)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                flipped = false;
            }
            else if (target.position.x < transform.position.x && !flipped)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                flipped = true;
            }

            animator.SetFloat("Speed", 0f);
            if (GetComponent<EnemyManager>().timeSinceLastHit <= 0)
            {
                GetComponent<AbstractAttack>().Attack();
            }
        }
    }


}
