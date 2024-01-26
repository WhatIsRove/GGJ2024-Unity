using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float moveAccel = 10f;
    public float maxSpeed = 50f;
    public float mouseSensitivity = 0.1f;

    public Vector2 mouseVerticalRange = new Vector2( -90, 90 );

    GameObject camera;
    Rigidbody rb;




    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        
    }
}
