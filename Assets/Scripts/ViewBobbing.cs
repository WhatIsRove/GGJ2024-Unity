using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewBobbing : MonoBehaviour
{
    public float bobIntensity;
    public float bobIntensityX;
    public float bobSpeed;

    Vector3 originalOffset;
    float sinTime;

    public Transform targetTransform;
    public Vector3 offset;


    // Start is called before the first frame update
    void Start()
    {
        originalOffset = offset;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = targetTransform.position + offset;
        transform.rotation = targetTransform.rotation;

        Vector3 moveDir = FindObjectOfType<PlayerController>().moveDir;
        if (moveDir.magnitude > 0)
        {
            sinTime += Time.deltaTime * bobSpeed;
        }
        else
        {
            if (sinTime >= 10)
            {
                sinTime = 0;
            }
        }

        float sinAmountY = -Mathf.Abs(bobIntensity * Mathf.Sin(sinTime));
        Vector3 sinAmountX = transform.right * bobIntensity * Mathf.Cos(sinTime) * bobIntensityX;

        offset = new Vector3
        {
            x = originalOffset.x,
            y = originalOffset.y + sinAmountY,
            z = originalOffset.z
        };

        offset += sinAmountX;
    }
}
