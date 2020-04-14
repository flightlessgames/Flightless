using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinning : MonoBehaviour
{
    [SerializeField] private Vector3 spinAxes = Vector3.zero;

    private void FixedUpdate()
    {
        transform.Rotate(spinAxes);

    }
}
