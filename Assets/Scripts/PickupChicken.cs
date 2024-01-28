using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupChicken : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            FindObjectOfType<PlayerController>().AddNades();
            //maybe sound
            Destroy(gameObject);
        }
    }
}
