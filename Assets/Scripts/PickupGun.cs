using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupGun : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            FindObjectOfType<PlayerController>().hasGun = true;
            FindObjectOfType<PlayerController>().gunCrosshair.SetActive(true);
            FindObjectOfType<PlayerController>().gun.SetActive(true);
            FindObjectOfType<AudioManager>().Play("Music1");
            Destroy(gameObject);
        }
    }
}
