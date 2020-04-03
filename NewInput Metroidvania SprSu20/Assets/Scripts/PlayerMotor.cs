using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private Rigidbody rb = null;

    private PlayerInputActions input = null;
    private Vector2 movementInput = Vector2.zero;

    [Header("Settings")]
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _dragForce = 1f;
    [SerializeField] private float _gravityForce = 10f;

    [Header("Required")]
    [SerializeField] private GroundChecker _groundChecker = null;

    private Vector3 moveVector = Vector3.zero;

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
        CreateMoveVector();
        CalculateDragForce();
        CalculateGravity();

        Move();
    }

    #region Movement Calculations, Custom RB
    private void CreateMoveVector()
    {
        float horizontal = movementInput.x;

        moveVector += new Vector3(horizontal, 0, 0);
    }

    private void CalculateDragForce()
    {
        //0 - our current direction (normalized to 1), times our dragForce chips away at the direction the player is moving and creates inertia towards 0 
        //(might jitter when near 0, less than dragForce)
        Vector3 drag = Vector3.zero - moveVector.normalized * _dragForce;

        moveVector += drag;
    }

    private void CalculateGravity()
    {
        Vector3 gravity = Vector3.down * _gravityForce;

        moveVector += gravity;
    }
    #endregion

    private void Move()
    {
        #region Testing Custom RB
        /*
        //check for collision with ground/terrain
        RaycastHit hit;
        Ray ray = new Ray(transform.position, moveVector);

        if(Physics.Raycast(ray, out hit))
        {
            //if we hit gound/terrain, set the movement distance vector to a new magnitude, equal to the distance to that object... do not go PAST that object 
            if (hit.transform.CompareTag("Ground"))
                moveVector = moveVector.normalized * (hit.distance - 1f); //hit.distance - 1f means the closest we can get to an object is 1 unit

            Debug.Log("position: " + transform.position + ", distance: " + hit.distance);
        }

        //then move (if no hit then use normal vector, if hit then vector is adjusted)
        transform.position += moveVector;
        */
        #endregion

        transform.position += new Vector3(movementInput.x, movementInput.y, 0) * Time.deltaTime * _moveSpeed;
    }



    private void Jump()
    {
        Debug.Log("JUMP");

        if (!CheckGrounded())
            return;

        //moveVector += new Vector3(0, _jumpForce, 0);
        rb.velocity = new Vector3(0, _jumpForce, 0);
        _groundChecker._isGrounded = false;
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
