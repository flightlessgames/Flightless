using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    private PlayerMotor motor = null;

    public PlayerState currState { get; private set; } = PlayerState.Idle;

    public enum PlayerState
    {
        Idle = 0,
        Walking = 1,
        Jumping = 2,
        Falling = 3,
        Climbing = 4
    }

    public enum PlayerAbility
    {
        WallJump = 0
    }

    private void Awake()
    {
        motor = GetComponent<PlayerMotor>();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public void SetPlayerState(PlayerState state)
    {
        currState = state;
    }

    public void EnableAbility(PlayerAbility ability)
    {
        switch (ability)
        {
            case PlayerAbility.WallJump:
                motor.EnableWallJump();
                break;

            default:
                Debug.Log("Invalid Ability");
                break;
        }
    }
}
