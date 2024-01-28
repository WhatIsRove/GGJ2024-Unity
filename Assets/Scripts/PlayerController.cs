using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float moveAccel = 10f;
    public float sprintAccel = 20f;
    public float maxSpeed = 50f;
    public float sprintMaxSpeed = 10f;
    public float mouseSensitivity = 1f;

    public Vector2 mouseVerticalRange;

    GameObject camera;
    Rigidbody rb;

    [HideInInspector]
    public Vector3 moveDir;
    Vector2 mousePos;

    float xViewRotate;
    float yViewRotate;

    float XZmag;

    public GameObject bulletPrefab;
    public Transform bulletPoint;

    public GameObject nadePrefab;
    public Transform nadePoint;
    public float throwForce = 10f;
    public float throwForceUp = 15f;

    public InputActionReference fireAction;
    bool isFiring = false;
    float timeToFire = 0f;

    public InputActionReference jumpAction;
    bool isJumping = false;

    public InputActionReference sprintAction;
    bool isSprinting = false;

    public float jumpForce = 10f;
    public float gravity;
    public LayerMask groundLayer;
    RaycastHit groundHit;
    RaycastHit camHit;
    public bool grounded;

    public InputActionReference[] hotbar;
    int hotbarIndex = 1;
    int prevHotbarIndex = 2;
    public GameObject gun;
    public GameObject chicken;

    public GameObject gunCrosshair;
    GameObject chickenCrosshair;

    public float maxHP;
    float currentHP;
    public RectMask2D hpMask;

    float maxRightMask;
    float initialRightMask;

    public bool hasGun;

    public float startingNades;
    public float currentNades;
    public TextMeshProUGUI nadeText;

    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;

        gunCrosshair = GameObject.FindGameObjectWithTag("CannonCrosshair");
        chickenCrosshair = GameObject.FindGameObjectWithTag("ChickenCrosshair");

        gunCrosshair.SetActive(false);
        chickenCrosshair.SetActive(false);

        currentHP = maxHP;

        maxRightMask = hpMask.rectTransform.rect.width - hpMask.padding.x - hpMask.padding.z;
        initialRightMask = hpMask.padding.z;

        currentNades = startingNades;
    }

    void Update()
    {
        if (currentHP <= 0 && !FindObjectOfType<GameManager>().gameIsOver)
        {
            FindObjectOfType<GameManager>().GameOver();
        }

        if (FindObjectOfType<GameManager>().isPaused || FindObjectOfType<GameManager>().gameIsOver) return;

        xViewRotate += mousePos.x * mouseSensitivity;
        yViewRotate -= mousePos.y * mouseSensitivity;

        yViewRotate = Mathf.Clamp(yViewRotate, mouseVerticalRange.x, mouseVerticalRange.y);

        camera.transform.localRotation = Quaternion.Euler(yViewRotate, 0, 0);
        transform.localRotation = Quaternion.Euler(0, xViewRotate, 0);

        fireAction.action.performed += _ => { isFiring = true; };
        fireAction.action.canceled += _ => { isFiring = false; };

        jumpAction.action.performed += _ => { isJumping = true; };
        jumpAction.action.canceled += _ => { isJumping = false; };

        sprintAction.action.performed += _ => { isSprinting = true; };
        sprintAction.action.canceled += _ => { isSprinting = false; };

        if (isSprinting)
        {
            FindObjectOfType<ViewBobbing>().bobSpeed = 11;
        } else
        {
            FindObjectOfType<ViewBobbing>().bobSpeed = 8;
        }

        if (hasGun)
        {

            hotbar[1].action.performed += _ => {
                if (hotbarIndex != 1)
                {
                    prevHotbarIndex = hotbarIndex;
                    hotbarIndex = 1;
                    SwitchHotbar();
                }
            };

            if (currentNades > 0)
            {
                hotbar[0].action.performed += _ => {
                    int tempIndex = hotbarIndex;
                    hotbarIndex = prevHotbarIndex;
                    prevHotbarIndex = tempIndex;
                    SwitchHotbar();
                };
                hotbar[2].action.performed += _ => {
                    if (hotbarIndex != 2)
                    {
                        prevHotbarIndex = hotbarIndex;
                        hotbarIndex = 2;
                        SwitchHotbar();
                    }
                };

                if (hotbarIndex == 2 && Time.time >= timeToFire - 0.1f)
                {
                    chicken.SetActive(true);
                }
            }

            if (isFiring && Time.time >= timeToFire)
            {
                if (hotbarIndex == 1)
                {
                    timeToFire = Time.time + 1f / bulletPrefab.GetComponent<Bullet>().fireRate;
                    SpawnBullet();
                    FindObjectOfType<AudioManager>().Play("ConfettiCannon");
                }
                else if (hotbarIndex == 2 && currentNades > 0)
                {
                    timeToFire = Time.time + 1f / nadePrefab.GetComponent<Grenade>().fireRate;
                    chicken.SetActive(false);
                    ThrowNade();
                    currentNades--;
                    nadeText.SetText(": " + currentNades);
                }

            }
        }

        if (Physics.Raycast(transform.position, -transform.up, out groundHit, 1.05f, groundLayer))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        ManageHealthBar();
    }

    void FixedUpdate()
    {
        var velocity = rb.velocity;
        
        XZmag = new Vector3(velocity.x, 0, velocity.z).magnitude;

        if (isSprinting)
        {
            if (XZmag > sprintMaxSpeed)
            {
                Vector3 reducedSpeed = rb.velocity;
                float keepY = reducedSpeed.y;
                reducedSpeed = Vector3.ClampMagnitude(reducedSpeed, sprintMaxSpeed);
                reducedSpeed.y = keepY;
                rb.velocity = reducedSpeed;
            }
        } else
        {
            if (XZmag > maxSpeed)
            {
                Vector3 reducedSpeed = rb.velocity;
                float keepY = reducedSpeed.y;
                reducedSpeed = Vector3.ClampMagnitude(reducedSpeed, maxSpeed);
                reducedSpeed.y = keepY;
                rb.velocity = reducedSpeed;
            }
        }

        if (moveDir == Vector3.zero)
        {
            velocity.x = 0;
            velocity.z = 0;
        }

        var localVelocity = transform.InverseTransformDirection(velocity);

        var lateralVelocty = new Vector3(localVelocity.x, 0, localVelocity.z);

        var normalSpeed = Vector3.Dot(lateralVelocty, moveDir.normalized);

        if (normalSpeed < sprintMaxSpeed && isSprinting)
        {
            normalSpeed += sprintAccel * moveDir.magnitude * Time.fixedDeltaTime;
            normalSpeed = Mathf.Min(normalSpeed, sprintMaxSpeed);
        } else if (normalSpeed < maxSpeed && !isSprinting)
        {
            normalSpeed += moveAccel * moveDir.magnitude * Time.fixedDeltaTime;
            normalSpeed = Mathf.Min(normalSpeed, maxSpeed);
        }

        var keepLocalY = localVelocity.y;
        localVelocity = normalSpeed * moveDir.normalized;
        localVelocity.y = keepLocalY;



        if (grounded)
        {
            var keepX = localVelocity.x;
            var keepZ = localVelocity.z;

            if (isJumping)
            {
                isJumping = false;
                rb.AddForce(jumpForce * transform.up * 10, ForceMode.Impulse);
            }

            localVelocity.x = keepX;
            localVelocity.z = keepZ;
        }
        else
        {
            localVelocity.y -= gravity * Time.fixedDeltaTime * 10;
            if (Physics.Raycast(transform.position, -transform.up, out groundHit, 1.06f, groundLayer))
            {
                localVelocity.y /= gravity;
            }
        }

        velocity = transform.TransformDirection(localVelocity);

        rb.velocity = velocity;
    }

    void SwitchHotbar()
    {
        switch (hotbarIndex)
        {
            case 1:
                gun.SetActive(true);
                chicken.SetActive(false);

                gunCrosshair.SetActive(true);
                chickenCrosshair.SetActive(false);
                break;
            case 2:
                if (currentNades > 0)
                {
                    gun.SetActive(false);
                    chicken.SetActive(true);

                    gunCrosshair.SetActive(false);
                    chickenCrosshair.SetActive(true);
                }
                break;
        }
    }

    void ThrowNade()
    {
        Vector3 aimDir = camera.transform.forward;
        GameObject nade = Instantiate(nadePrefab, nadePoint.position, camera.transform.rotation);
        Rigidbody nadeRB = nade.GetComponent<Rigidbody>();

        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out camHit, 500f))
        {
            aimDir = (camHit.point - bulletPoint.position).normalized;
        }

        nadeRB.AddForce(aimDir * throwForce + transform.up * throwForceUp, ForceMode.Impulse);

    }

    private void ManageHealthBar()
    {
        var targetWidth = currentHP * (maxRightMask / maxHP);
        var newRightMask = maxRightMask + initialRightMask - targetWidth;
        var padding = hpMask.padding;

        padding.z = newRightMask;
        hpMask.padding = padding;
    }

    public void TakeDamage()
    {
        if (currentHP > 0)
        {
            currentHP--;
        }
    }

    public void AddNades()
    {
        currentNades++;
        nadeText.SetText(": " + currentNades);
    }


    void SpawnBullet()
    {
        Vector3 aimDir = camera.transform.forward;

        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out camHit, 500f))
        {
            if (camHit.collider.tag != "Bullet") aimDir = (camHit.point - bulletPoint.position).normalized;
        }

        Instantiate(bulletPrefab, bulletPoint.position, Quaternion.LookRotation(aimDir));
    }


    void OnMove(InputValue input)
    {
        var temp = input.Get<Vector2>();
        moveDir = new Vector3(temp.x, 0, temp.y);
    }

    void OnLook(InputValue input)
    {
        mousePos = input.Get<Vector2>();
    }
}
