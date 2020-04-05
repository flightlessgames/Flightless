using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Required")]
    [SerializeField] private PlayerController player = null;
    [SerializeField] private Transform topRight = null;
    [SerializeField] private Transform botLeft = null;

    [Header("Settings")]
    [SerializeField] private Vector3 _playerOffset = Vector3.zero;
    [SerializeField] private Vector3 _maxYawOffset = Vector3.zero;

    private void LateUpdate()
    {
        transform.position = player.transform.position + LerpPosition();

        transform.rotation = CalculateYaw();

    }

    private Quaternion CalculateYaw()
    {
        float lerpY = player.transform.position.y / (topRight.position.y - botLeft.position.y);

        float yawX = Mathf.Lerp(-_maxYawOffset.x, _maxYawOffset.x, lerpY);

        return Quaternion.Euler(new Vector3(yawX, 0, 0));
    }

    private Vector3 LerpPosition()
    {
        float lerpY = player.transform.position.y / (topRight.position.y - botLeft.position.y);

        float positionY = Mathf.Lerp(-_playerOffset.y, _playerOffset.y, lerpY);

        return new Vector3(_playerOffset.x, positionY, _playerOffset.z);
    }
}
