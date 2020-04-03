using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GroundChecker : MonoBehaviour
{
    public bool _isGrounded { get; private set; } = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
            _isGrounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Ground"))
            _isGrounded = false;
    }
}
