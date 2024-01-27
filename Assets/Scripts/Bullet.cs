using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour
{
    public float speed;
    public float accuracy;
    public float fireRate;
    public float damageAmount;
    public float explosionRadius;

    public GameObject muzzlePrefab;
    public GameObject hitPrefab;

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

        if (muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward + offset;
            var ps = muzzleVFX.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(muzzleVFX, ps.main.duration);
            else
            {
                var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, psChild.main.duration);
            }
        }

        StartCoroutine(DestroyParticle(4f));
    }

    void FixedUpdate()
    {
        if (speed != 0 && rb != null) rb.position += (transform.forward + offset) * (speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider co)
    {
        if (co.gameObject.tag != "Bullet" && !collided)
        {
            collided = true;

            speed = 0;
            GetComponent<Rigidbody>().isKinematic = true;

            Collider[] objectsInRange = Physics.OverlapSphere(transform.position, explosionRadius);

            foreach (Collider obj in objectsInRange)
            {
                if (obj.gameObject.GetComponent<EnemyController>() != null)
                {
                    var enemy = obj.gameObject.GetComponent<EnemyController>();
                    enemy.TakeDamage(damageAmount);
                }
            }

            var contact = co.ClosestPointOnBounds(transform.position);
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, transform.position - contact);

            if (hitPrefab != null)
            {
                var hitVFX = Instantiate(hitPrefab, contact, rot) as GameObject;

                var ps = hitVFX.GetComponent<ParticleSystem>();
                if (ps == null)
                {
                    var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                    Destroy(hitVFX, psChild.main.duration);
                }
                else
                    Destroy(hitVFX, ps.main.duration);
            }

            StartCoroutine(DestroyParticle(0f));
        }
    }

    public IEnumerator DestroyParticle(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
