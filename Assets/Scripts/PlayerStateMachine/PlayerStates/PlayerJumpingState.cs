using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpingState : PlayerBaseState {

    float jumpVelocity;

    public PlayerJumpingState(PlayerStateMachineBase currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) {
        jumpVelocity = ctx.InitialJumpVelocity;
    }

    public PlayerJumpingState(PlayerStateMachineBase currentContext, PlayerStateFactory playerStateFactory, float initialJumpVelocityMultiplier)
    : base(currentContext, playerStateFactory) {
        jumpVelocity = ctx.InitialJumpVelocity * Mathf.Sqrt(initialJumpVelocityMultiplier);
    }

    public override void EnterState() {
        ctx.EPlayerState = EPlayerState.Jumping;
        //Debug.Log("Hello from the Jumping State");

        ctx.Controller.stepOffset = 0f;
        ctx.JumpBufferedCounter = 0f;

        ctx.CurrentGravityValue = ctx.JumpingGravityButtonHeld;


        Jump();

        ctx.NextJumpTime = Time.time + (1f / ctx.JumpRate); //Sets the next time we should be able to jump

        //Disable the intial jump press
        ctx.InputJumpButtonPressed = false;

        //JumpButtonHad has not been released yet
        ctx.JumpButtonReleased = false;


        //Debug.Log("Jumped");

        //Disable collision with platforms while in jumping state
        ctx.Controller.enableOverlapRecovery = false;
        if (ctx.IsSwimmer) {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("WaterPlayer"), LayerMask.NameToLayer("WaterSurface"), true);
        }
    }

    public override void UpdateState() {
        //Debug.Log("We Jumping");

        ctx.UnchangedMove = ctx.GetMoveInput(); //dont need this line
        ctx.Move = ctx.UnchangedMove;

        MovePlayer();
        ctx.UpdateRotation2D();

        ctx.JumpButtonHoldTimer += Time.deltaTime;
        ctx.JumpBufferedCounter -= Time.deltaTime;

        if (ctx.HoldJump) {
            if (!ctx.JumpButtonReleased) {
                if (ctx.InputJumpButtonHeld) {
                    //Use jumpingGravityButtonHeld
                }
                else {
                    ctx.JumpButtonReleased = true;
                    if (ctx.JumpButtonHoldTimer < ctx.MinJumpHoldTime) {
                        //Use jumpingGravityButtonHeld
                    }
                    else {
                        ctx.CurrentGravityValue = ctx.JumpingGravityButtonReleased;
                    }
                }
            }
            else {
                if (ctx.JumpButtonHoldTimer < ctx.MinJumpHoldTime) {
                    //Use jumpingGravityButtonHeld
                }
                else {
                    ctx.CurrentGravityValue = ctx.JumpingGravityButtonReleased;
                }
            }
        }

        UpdateGravity();
        CheckSwitchStates();
    }

    public override void ExitState() {
        ctx.Controller.enableOverlapRecovery = true;
    }

    public override void CheckSwitchStates() {
        base.CheckSwitchStates();
        if (ctx.CurrentPlayerState != this) {
            // If we switched states within base, exit this function now
            return;
        }

        if (ctx.bumpingHead()) {
            SwitchState(factory.Falling());
        }
        else if (ctx.InputJumpButtonPressed) {
            if (ctx.MovementInputEnabled) {
                if (Time.time > ctx.NextJumpTime) {
                    if (ctx.ExtraJumpsLeft > 0) {
                        ctx.ExtraJumpsLeft -= 1;
                        SwitchState(factory.Jumping());
                    }
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
            ctx.IsFacingRight = false;
        }
        else if (ctx.Move.x > 0) {
            ctx.Controller.Move(Vector3.right * Mathf.Abs(ctx.Move.x) * Time.deltaTime * ctx.CurrentPlayerSpeed * ctx.AirSpeedRatio);
            ctx.IsFacingRight = true;
        }
        else {
            ctx.Controller.Move(Vector3.zero);
        }
    }

    private void UpdateGravity() {
        if (ctx.PlayerVelocity.y > -1f * ctx.TerminalVelocity) {
            float newYVelocity = ctx.PlayerVelocity.y + ctx.CurrentGravityValue * Time.deltaTime;
            ctx.PlayerVelocity = new Vector3(ctx.PlayerVelocity.x, newYVelocity, ctx.PlayerVelocity.z);
        }
        ctx.Controller.Move(ctx.PlayerVelocity * Time.deltaTime);
    }

    protected void Jump() {
        Vector3 oldVelocity = ctx.PlayerVelocity;
        ctx.PlayerVelocity = new Vector3(oldVelocity.x, jumpVelocity, oldVelocity.z);

        ctx.JumpButtonHoldTimer = 0f;
    }
}
