using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    public event Action Fell = delegate { };
    public event Action<Vector3> Walked = delegate { };
    public event Action Jumped = delegate { };
    public event Action Climbed = delegate { };

    private Rigidbody rb = null;

    private PlayerInputActions input = null;
    private Vector2 movementInput = Vector2.zero;

    [Header("Settings")]
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _climbSpeed = 5f;
    [SerializeField] private float _velocityLimit = 50f;

    [Header("Required")]

    [SerializeField] private GroundChecker _groundChecker = null;
    [SerializeField] private GroundChecker _leftWallCheck = null;
    [SerializeField] private GroundChecker _rightWallCheck = null;

    private bool _canWalljump = false;
    private Coroutine _climbWallsRoutine = null;


    private enum WallCheckDirection
    {
        Left = 0,
        Right = 1,
        Up = 2
    }
    private WallCheckDirection prevJump = WallCheckDirection.Up;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        Physics.gravity = new Vector3(Physics.gravity.x, Physics.gravity.y * 2, Physics.gravity.z);

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
        Fall();
        Climb();
    }

    private void Move()
    {
        //move horizontally
        Vector3 newMovement = new Vector3(movementInput.x, 0, 0) * Time.deltaTime * _moveSpeed;


        rb.MovePosition(transform.position + newMovement);

        //invoke movement if our movement is > 0
        if (Mathf.Abs(rb.velocity.x) > 0)
        {
            Walked.Invoke(newMovement);
            return;
        }
    }

    private void Fall()
    {
        //fall on DOWN input
        if (movementInput.y < 0)
        {
            rb.velocity += Vector3.down * _moveSpeed;
            Fell.Invoke();
        }

        //speed limit?
        if (rb.velocity.magnitude > _velocityLimit)
        {
            rb.velocity = rb.velocity.normalized * _velocityLimit;
        }
    }

    private void Climb()
    {
        //if we're not inputting greater than 0 (UP), return this function void
        if (!(movementInput.y > 0))
            return;

        bool canClimb = false;
        ClassInterfacer[] classesInterfaced = CheckWalls();
        WallCheckDirection direction = WallCheckDirection.Up;

        if (movementInput.x < 0)
        {
            if (classesInterfaced[0] is IClimbable)
            {
                canClimb = true;
                direction = WallCheckDirection.Left;
            }
        }

        if (movementInput.x > 0)
        {
            if (classesInterfaced[1] is IClimbable)
            {
                canClimb = true;
                direction = WallCheckDirection.Right;
            }
        }

        if (canClimb)
        {
            if (_climbWallsRoutine == null)
            {
                _climbWallsRoutine = StartCoroutine(PlayClimbAnimation(direction));
                Climbed.Invoke();
            }
        }
    }

    private void Jump()
    {
        bool canJump = false;

        //if on the ground, set jump direction to UP
        if (CheckGrounded() is IJumpable)
        {
            prevJump = WallCheckDirection.Up;
            canJump = true;
        }

        if (_canWalljump)
        {
            ClassInterfacer[] classesInterfaced = CheckWalls();
            if (classesInterfaced[0] is IJumpable && prevJump != WallCheckDirection.Left)
            {
                prevJump = WallCheckDirection.Left;
                canJump = true;
            }
            else if (classesInterfaced[1] is IJumpable && prevJump != WallCheckDirection.Right)
            {
                prevJump = WallCheckDirection.Right;
                canJump = true;
            }
        }

        if (canJump)
        {
            //set velocity to instantly jump up, do not "jump against air" and stall mid-air
            rb.velocity = (Vector3.up * _jumpForce);
        }
    }

    private ClassInterfacer CheckGrounded()
    {
        if(_groundChecker != null)
        {
            return _groundChecker.CollisionCheck();
        }

        return null;
    }

    private ClassInterfacer[] CheckWalls()
    {
        //if we've defined two collision checkers
        if(_leftWallCheck != null && _rightWallCheck != null)
        {
            //if either returns true, then this returns true
            var leftCheck = _leftWallCheck.CollisionCheck();
            var rightCheck = _rightWallCheck.CollisionCheck();

            //enum direction Left = 0, right = 1. In the array Left is stored as 0, right as 1
            List<ClassInterfacer> classesInterfaced = new List<ClassInterfacer>() { leftCheck, rightCheck };
            return classesInterfaced.ToArray();
        }
        return null;
    }

    private IEnumerator PlayClimbAnimation(WallCheckDirection direction)
    {
        Vector2 savedMovement = movementInput;

        input.Disable();
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        movementInput = Vector2.zero;


        bool isActiveClimbing = true;
        Vector3 climbMovement = Vector3.up * Time.deltaTime * _climbSpeed;
        ClassInterfacer[] walls;

        //needs to be scalable for height of wall
        while (isActiveClimbing)
        {
            transform.position += climbMovement;

            //adjust for animation time
            yield return new WaitForEndOfFrame();

            //check walls each whileloop
            walls = CheckWalls();

            //care about if we are climbing left or right
            //enum direction Left = 0, right = 1. In the array Left is stored as 0, right as 1
            if (walls[(int)direction] == null)
            {
                isActiveClimbing = false;
            }
        }

        movementInput = savedMovement;

        input.Enable();
        rb.useGravity = true;
        _climbWallsRoutine = null;
    }

    public void EnableWallJump()
    {
        _canWalljump = true;
    }
}
