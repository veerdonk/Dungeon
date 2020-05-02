using System;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class PlayerController2D : MonoBehaviour
{
    public static PlayerController2D instance;

    public Rigidbody2D rb;
    public Animator animator;
    public ParticleSystem throwPS;
    public ParticleSystem dustPS;
    public ParticleSystem dashPS;

    public float MOVEMENT_BASE_SPEED = 1.0f;

    public Vector2 movementDirection;
    public float movementSpeed = 5f;

    public int maxDashCharges = 3;
    public int dashCharges = 3;
    public float dashSpeed = 10f;
    public float startDashTime;
    private float dashTime = 0;
    public float dashCooldown;
    private float timeTillNextDash = 0f;
    public bool isDashing;
    private Vector2 dashDirection;
    private int childCountMinusWeapon;

    public bool dead = false;

    Inventory inventory;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Inventory is not a singleton");
        }
        instance = this;
    }

    private void Start()
    {
        inventory = Inventory.instance;
        dashTime = startDashTime;
        childCountMinusWeapon = transform.childCount;
    }

    // Update is called once per frame
    void Update()
    {

        ProcessInputs();

    }

    private void UpdateLookDirection()
    {
        Vector3 newScale = transform.localScale;

        Vector3 pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        float WorldXPos = Camera.main.ScreenToWorldPoint(pos).x;

        if (WorldXPos > transform.position.x) // character it's your char game object
        {
            newScale.x = 1;
        }
        else
        {
            newScale.x = -1;
        }

        transform.localScale = newScale;
    }

    private void FixedUpdate()
    {
        if(movementDirection.magnitude > 0 && dashDirection == Vector2.zero)
        {
            Move();
            
        }
        UpdateLookDirection();

        if(dashDirection != Vector2.zero)
        {
            Dash();
        }

    }


    void ProcessInputs()
    {
        if (dead)
        {
            return;
        }

        movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        movementDirection.Normalize();
        animator.SetFloat("Speed", movementDirection.sqrMagnitude);

        timeTillNextDash -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && dashDirection == Vector2.zero && dashCharges > 0 && movementDirection.magnitude > 0)
        {
            dashCharges--;
            UIUpdater.instance.SetDashResetIndicator();
            StartCoroutine(Util.ExecuteAfterTime(dashCooldown, () =>
            {
                dashCharges++;
            }));
            ParticleSystem dashPSObj = Instantiate(dashPS);
            dashPSObj.transform.position = transform.position;
            CameraShake.instance.ShakeCamera();
            isDashing = true;
            timeTillNextDash = dashCooldown;
            dashDirection = movementDirection;
        }


        if (Input.GetKeyDown(KeyCode.Mouse0) && transform.childCount > childCountMinusWeapon && !IsMouseOverUI())
        {
            //Throw currently equipped weapon
            GetComponentInChildren<Throw>().ExecuteThrow();
            inventory.RemoveEquippedWeapon();
            //Play particle effect
            PlayThrowPS();
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            int change = 0;
            if(Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                change = 1;
            }
            else
            {
                change = -1;
            }

            inventory.SetSelectedSlot(change);
        }


    }

    private void Dash()
    {

        float tempDashSpeed = dashSpeed;
        if(dashTime <= startDashTime / 4)
        {
            tempDashSpeed = dashSpeed / 4;
        }
        else if(dashTime <= startDashTime / 2)
        {
            tempDashSpeed = dashSpeed / 2;
        }
        //Check if dash is over
        if (dashTime <= 0)
        {
            Debug.Log("Dash ended");
            isDashing = false;
            dashDirection = Vector2.zero;
            dashTime = startDashTime;
            //rb.velocity = Vector2.zero;
        }
        else
        {
            dashTime -= Time.deltaTime;
            rb.MovePosition(rb.position + dashDirection * dashSpeed * MOVEMENT_BASE_SPEED * Time.deltaTime);
        }
        
    }

    private void PlayThrowPS()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        throwPS.transform.rotation = Quaternion.Euler(0f, 0f, rotZ - 45);
        throwPS.Play();

    }

    void Move()
    {
        rb.velocity = Vector2.zero;
        CreateDust();
        rb.MovePosition(rb.position + movementDirection * movementSpeed * MOVEMENT_BASE_SPEED * Time.fixedDeltaTime);
        
    }

    public void setPlayerPosition(Vector3 position)
    {
        rb.position = position;
    }

    void CreateDust()
    {
        dustPS.Play();
    }

    private bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

}


public enum WeaponType
{
    SWORD,
    BOW
}
