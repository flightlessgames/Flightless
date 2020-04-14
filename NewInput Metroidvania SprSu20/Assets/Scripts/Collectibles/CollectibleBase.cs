using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleBase : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if ( player != null)
        {
            CollectibleEvent(player);
            gameObject.SetActive(false);
        }
    }

    public virtual void CollectibleEvent(PlayerController player)
    {
        Debug.Log("Collected Me!");
    }
}
