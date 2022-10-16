using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpingState : PlayerBaseState {

    float jumpHeight;

    public PlayerJumpingState(PlayerStateMachineBase currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) {
        jumpHeight = ctx.JumpHeight;
    }

    public PlayerJumpingState(PlayerStateMachineBase currentContext, PlayerStateFactory playerStateFactory, float customJumpHeight)
    : base(currentContext, playerStateFactory) {
        jumpHeight = customJumpHeight;
    }

    public override void EnterState() {
        ctx.EPlayerState = EPlayerState.Jumping;
        //Debug.Log("Hello from the Jumping State");

        ctx.Controller.stepOffset = 0f;
        ctx.CurrentGravityValue = ctx.BaseGravityValue * 8f;
        ctx.JumpBufferedCounter = 0f;
        Vector3 oldVelocity = ctx.PlayerVelocity;
        ctx.PlayerVelocity = new Vector3(oldVelocity.x, Mathf.Sqrt(jumpHeight * -3.0f * ctx.BaseGravityValue), oldVelocity.z); //JUMP
        ctx.NextJumpTime = Time.time + (1f / ctx.JumpRate); //Sets the next time we should be able to jump
        ctx.JumpHoldTimeCounter = ctx.MaxJumpHoldTime;

        //Disable the intial jump press
        ctx.InputJumpButtonPressed = false;


        //Debug.Log("Jumped");

        //Disable collision with platforms while in jumping state
        ctx.Controller.enableOverlapRecovery = false;
        if (ctx.IsSwimmer) {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("WaterPlayer"), LayerMask.NameToLayer("WaterSurface"), true);
        }
    }

    public override void UpdateState() {
        //Debug.Log("We Jumping");

        if (ctx.MovementInputEnabled) {
            ctx.UnchangedMove = ctx.GetMoveInput();
        }
        else {
            ctx.UnchangedMove = Vector3.zero;
        }
        ctx.Move = ctx.UnchangedMove;
        UpdateGravity();

        MovePlayer();

        if (!ctx.InputJumpButtonHeld) {
            //Button is released, change boolean to false
            ctx.JumpHoldTimeCounter = 0f;
        }

        ctx.JumpBufferedCounter -= Time.deltaTime;

        //If holdJump is enabled, then continuously push player up while they hold jump
        if (ctx.HoldJump) {
            if (ctx.InputJumpButtonHeld) {
                if (ctx.JumpHoldTimeCounter > 0) {
                    Vector3 oldVelocity = ctx.PlayerVelocity;
                    //reset velocity so that this jump will have the same velocity as the original jump
                    ctx.PlayerVelocity = new Vector3(oldVelocity.x, Mathf.Sqrt(jumpHeight * -3.0f * ctx.BaseGravityValue), oldVelocity.z);
                    ctx.JumpHoldTimeCounter -= Time.deltaTime;
                }
                else {
                    //ran out of holdTime, change boolean to false
                    ctx.InputJumpButtonHeld = false;
                }
            }
        }

        CheckSwitchStates();
    }

    public override void ExitState() {
        ctx.CurrentGravityValue = ctx.BaseGravityValue;
        ctx.Controller.enableOverlapRecovery = true;
    }

    public override void CheckSwitchStates() {
        // myabe we can just start a new instance of the jump state?
        if (ctx.InputJumpButtonPressed) {
            if (Time.time > ctx.NextJumpTime) {
                if (ctx.ExtraJumpsLeft > 0) {
                    ctx.ExtraJumpsLeft -= 1;
                    SwitchState(factory.Jumping());
                }
            }
            ctx.InputJumpButtonPressed = false;
        }
        else if (ctx.IsGrounded()) {
            SwitchState(factory.Idle());
        }
        else if (ctx.PlayerVelocity.y <= 0f) {
            SwitchState(factory.Falling());
        }
    }

    //Non-State Machine related Functions
    private void MovePlayer() {
        if (ctx.Move.x < 0) {
            ctx.Controller.Move(Vector3.left * Mathf.Abs(ctx.Move.x) * Time.deltaTime * ctx.CurrentPlayerSpeed * ctx.AirSpeedRatio);
        }
        else if (ctx.Move.x > 0) {
            ctx.Controller.Move(Vector3.right * Mathf.Abs(ctx.Move.x) * Time.deltaTime * ctx.CurrentPlayerSpeed * ctx.AirSpeedRatio);
        }
        else {
            ctx.Controller.Move(Vector3.zero);
        }
    }

    private void UpdateGravity() {
        if (ctx.PlayerVelocity.y > -1f * ctx.TerminalVelocity) {
            //Going to try to use Velocity Verlet here
            float previousYVelocity = ctx.PlayerVelocity.y;
            float newYVelocity = ctx.PlayerVelocity.y + ctx.CurrentGravityValue * Time.deltaTime;
            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;
            ctx.PlayerVelocity = new Vector3(ctx.PlayerVelocity.x, nextYVelocity, ctx.PlayerVelocity.z);
        }
        ctx.Controller.Move(ctx.PlayerVelocity * Time.deltaTime); ;
    }

    private void SetupJumpVariables() {
        
    }
}
