using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GroundChecker : MonoBehaviour
{
    public bool isRecentlyTrue { get; private set; } = false;

    private Collider _myCollider = null;

    private void Awake()
    {
        _myCollider = GetComponent<BoxCollider>();
    }

    public ClassInterfacer CollisionCheck()
    {
        Collider[] collisions = Physics.OverlapBox(transform.position, _myCollider.bounds.extents);

        foreach (Collider col in collisions)
        {
            var foo = col.GetComponent<ClassInterfacer>();
            if (foo != null)
            {
                isRecentlyTrue = true;
                return foo;
            }
        }

        isRecentlyTrue = false;
        return null;
    }
}
