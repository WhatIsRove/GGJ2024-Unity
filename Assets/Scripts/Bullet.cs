using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float accuracy;
    public float fireRate;

    Vector3 offset;
    bool collided;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (accuracy != 100)
        {
            accuracy = 1 - (accuracy / 100);

            for (int i = 0; i < 2; i++)
            {
                var val = 1 * Random.Range(-accuracy, accuracy);
                var index = Random.Range(0, 2);
                if (i == 0)
                {
                    if (index == 0) offset = new Vector3(0, -val, 0);
                    else offset = new Vector3(0, val, 0);
                }
                else
                {
                    if (index == 0) offset = new Vector3(0, offset.y, -val);
                    else offset = new Vector3(0, offset.y, val);
                }
            }
        }
        StartCoroutine(DestroyParticle(4f));
    }

    void FixedUpdate()
    {
        if (speed != 0 && rb != null) rb.position += (transform.forward + offset) * (speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision co)
    {
        if (co.gameObject.tag != "Bullet" && !collided)
        {
            collided = true;

            speed = 0;
            GetComponent<Rigidbody>().isKinematic = true;

            StartCoroutine(DestroyParticle(0f));
        }
    }

    public IEnumerator DestroyParticle(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
