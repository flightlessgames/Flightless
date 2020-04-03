using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    private Rigidbody rb = null;

    private PlayerInputActions input = null;
    private Vector2 movementInput = Vector2.zero;

    [Header("Settings")]
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _jumpForce = 10f;

    [Header("Required")]
    [SerializeField] private GroundChecker _groundChecker = null;

    private Vector3 preMove = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        input = new PlayerInputActions();
        input.PlayerControls.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        input.PlayerControls.Jump.performed += ctx => Jump();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        float horizontal = movementInput.x;
        Debug.Log(movementInput + ". horizontal: " + horizontal);

        rb.AddForce(new Vector3(horizontal, 0, 0) * _moveSpeed);
    }

    private void Jump()
    {
        Debug.Log("JUMP");

        if (!CheckGrounded())
            return;

        rb.AddForce(new Vector3(0, _jumpForce, 0));
    }

    private bool CheckGrounded()
    {
        if(_groundChecker != null)
        {
            return _groundChecker._isGrounded;
        }

        return true;
    }
}
