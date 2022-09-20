using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpingState : PlayerBaseState {

    bool jumpButtonHeld;
    public PlayerJumpingState(PlayerStateMachineBase currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) {
        jumpButtonHeld = true;
    }

    public override void EnterState() {
        ctx.EPlayerState = EPlayerState.Jumping;
        //Debug.Log("Hello from the Jumping State");

        ctx.Controller.stepOffset = 0f;
        ctx.CurrentGravityValue = ctx.BaseGravityValue * 8f;
        ctx.JumpBufferedCounter = 0f;
        Vector3 oldVelocity = ctx.PlayerVelocity;
        ctx.PlayerVelocity = new Vector3(oldVelocity.x, Mathf.Sqrt(ctx.JumpHeight * -3.0f * ctx.BaseGravityValue), oldVelocity.z);
        ctx.NextJumpTime = Time.time + (1f / ctx.JumpRate); //Sets the next time we should be able to jump
        ctx.JumpHoldTimeCounter = ctx.MaxJumpHoldTime;
        //Debug.Log("Jumped");

        //Disable collision with platforms while in jumping state
        ctx.Controller.enableOverlapRecovery = false;
    }

    public override void UpdateState() {
        //Debug.Log("We Jumping");

        if (ctx.MovementInputEnabled) {
            ctx.UnchangedMove = ctx.GetMoveInput();
        }
        else {
            ctx.UnchangedMove = Vector3.zero;
        }
        ctx.Move = ctx.SetMoveRelativeToCamera();
        ctx.UpdateRotation();
        ctx.UpdateGravity();

        MovePlayer();

        if (Input.GetButtonUp("Jump")) {
            //Button is released, change boolean to false
            jumpButtonHeld = false;
            ctx.JumpHoldTimeCounter = 0f;
        }

        ctx.JumpBufferedCounter -= Time.deltaTime;
        //Use an extra jump while already in jumping state
        if (Input.GetButtonDown("Jump")) {
            if (Time.time > ctx.NextJumpTime) {
                if (ctx.ExtraJumpsLeft > 0) {
                    Vector3 oldVelocity = ctx.PlayerVelocity;
                    //reset velocity so that this jump will have the same velocity as the original jump
                    ctx.PlayerVelocity = new Vector3(oldVelocity.x, Mathf.Sqrt(ctx.JumpHeight * -3.0f * ctx.BaseGravityValue), oldVelocity.z);
                    ctx.ExtraJumpsLeft -= 1;
                    ctx.NextJumpTime = Time.time + (1f / ctx.JumpRate); //Sets the next time we should be able to jump
                    ctx.JumpHoldTimeCounter = ctx.MaxJumpHoldTime;
                    jumpButtonHeld = true;
                    //Debug.Log("Jumped");
                }
            }
        }

        //If holdJump is enabled, then continuously push player up while they hold jump
        if (ctx.HoldJump) {
            if (Input.GetButton("Jump")) {
                if (ctx.JumpHoldTimeCounter > 0) {
                    Vector3 oldVelocity = ctx.PlayerVelocity;
                    //reset velocity so that this jump will have the same velocity as the original jump
                    ctx.PlayerVelocity = new Vector3(oldVelocity.x, Mathf.Sqrt(ctx.JumpHeight * -3.0f * ctx.BaseGravityValue), oldVelocity.z);
                    ctx.JumpHoldTimeCounter -= Time.deltaTime;
                }
                else {
                    //ran out of holdTime, change boolean to false
                    jumpButtonHeld = false;
                }
            }
        }

        CheckSwitchStates();
    }

    public override void ExitState() {
        ctx.CurrentGravityValue = ctx.BaseGravityValue;
    }

    public override void CheckSwitchStates() {
        if (ctx.IsGrounded()) {
            SwitchState(factory.Idle());
        }
        else if (ctx.PlayerVelocity.y <= 0f) {
            SwitchState(factory.Falling());
        }
    }

    //Non-State Machine related Functions
    private void MovePlayer() {
        if (ctx.Move != Vector3.zero) {
            ctx.Controller.Move(ctx.transform.forward * Time.deltaTime * ctx.CurrentPlayerSpeed * ctx.AirSpeedRatio);
        }
    }
}
