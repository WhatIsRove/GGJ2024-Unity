using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveAccel = 10f;
    public float maxSpeed = 50f;
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

    public InputActionReference fireAction;
    bool isFiring = false;

    public InputActionReference jumpAction;
    bool isJumping = false;

    public float jumpForce = 10f;
    public float gravity;
    public LayerMask groundLayer;
    RaycastHit hit;
    public bool grounded;

    void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        xViewRotate += mousePos.x * mouseSensitivity;
        yViewRotate -= mousePos.y * mouseSensitivity;

        yViewRotate = Mathf.Clamp(yViewRotate, mouseVerticalRange.x, mouseVerticalRange.y);

        camera.transform.localRotation = Quaternion.Euler(yViewRotate, 0, 0);
        transform.localRotation = Quaternion.Euler(0, xViewRotate, 0);

        fireAction.action.performed += _ => { isFiring = true; };
        fireAction.action.canceled += _ => { isFiring = false; };

        jumpAction.action.performed += _ => { isJumping = true; };
        jumpAction.action.canceled += _ => { isJumping = false; };

        if (Physics.Raycast(transform.position, -transform.up, out hit, 1.05f, groundLayer))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }

    void FixedUpdate()
    {
        var velocity = rb.velocity;

        
        XZmag = new Vector3(velocity.x, 0, velocity.z).magnitude;

        if (XZmag > maxSpeed)
        {
            Vector3 reducedSpeed = rb.velocity;
            float keepY = reducedSpeed.y;
            reducedSpeed = Vector3.ClampMagnitude(reducedSpeed, maxSpeed);
            reducedSpeed.y = keepY;
            rb.velocity = reducedSpeed;
        }

        if (moveDir == Vector3.zero)
        {
            velocity.x = 0;
            velocity.z = 0;
        }

        var localVelocity = transform.InverseTransformDirection(velocity);

        var lateralVelocty = new Vector3(localVelocity.x, 0, localVelocity.z);

        var normalSpeed = Vector3.Dot(lateralVelocty, moveDir.normalized);

        if (normalSpeed < maxSpeed)
        {
            normalSpeed += moveAccel * moveDir.magnitude * Time.fixedDeltaTime;
            normalSpeed = Mathf.Min(normalSpeed, maxSpeed);
        }

        var keepLocalY = localVelocity.y;
        localVelocity = normalSpeed * moveDir.normalized;
        localVelocity.y = keepLocalY;



        if (grounded)
        {
            //velocity.y = -2f;

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
            if (Physics.Raycast(transform.position, -transform.up, out hit, 1.06f, groundLayer))
            {
                localVelocity.y /= gravity;
            }
        }

        velocity = transform.TransformDirection(localVelocity);

        rb.velocity = velocity;
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
