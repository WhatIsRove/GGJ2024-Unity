using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class Grenade : MonoBehaviour
{
    public float fireRate;
    public float explosionRadius;
    public float timeTillExplode = 2f;
    public GameObject explosionVFX;
    public float timeTillEffectDispersed;

    void Start()
    {

        StartCoroutine(Explode(timeTillExplode));
    }

    public IEnumerator Explode(float explodeTime)
    {
        yield return new WaitForSeconds(explodeTime);
        
        GameObject vfx = Instantiate(explosionVFX, transform.position, Quaternion.identity);
        Destroy(vfx, timeTillEffectDispersed);

        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider obj in objectsInRange)
        {
            //Enemy dmg
        }

        StartCoroutine(DestroyParticle(0.1f));
    }

    public IEnumerator DestroyParticle(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
