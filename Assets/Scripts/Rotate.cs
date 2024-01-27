using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector2 xAxis;
    public Vector2 yAxis;
    public Vector2 zAxis;

    float x;
    float y;
    float z;

    Vector3 rotation;

    private void Start()
    {
        x = Random.Range(xAxis.x, xAxis.y);
        y = Random.Range(yAxis.x, yAxis.y);
        z = Random.Range(zAxis.x, zAxis.y);

        rotation = new Vector3 (x, y, z);
    }

    private void Update()
    {
        transform.Rotate(rotation * Time.deltaTime);
    }
}
