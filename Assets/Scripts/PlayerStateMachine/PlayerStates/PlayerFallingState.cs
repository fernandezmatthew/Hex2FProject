using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallingState : PlayerBaseState
{
    public PlayerFallingState(PlayerStateMachineBase currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) {

    }

    public override void EnterState() {
        ctx.EPlayerState = EPlayerState.Falling;
        //Debug.Log("Hello from the Falling State");

        ctx.Controller.stepOffset = 0;

        ctx.CurrentGravityValue = ctx.FallingGravity;
    }

    public override void UpdateState() {
        //Debug.Log("We Falling");

        if (ctx.MovementInputEnabled) {
            ctx.UnchangedMove = ctx.GetMoveInput();
        }
        else {
            ctx.UnchangedMove = Vector3.zero;
        }
        ctx.Move = ctx.UnchangedMove;

        MovePlayer();
        ctx.UpdateRotation2D();
        ctx.JumpBufferedCounter -= Time.deltaTime;

        UpdateGravity();
        CheckSwitchStates();
    }

    public override void ExitState() {

    }

    public override void CheckSwitchStates() {
        if (ctx.InputJumpButtonPressed) {
            if (!ctx.bumpingHead()) {
                if (Time.time > ctx.NextJumpTime) {
                    if (ctx.ExtraJumpsLeft > 0) {
                        ctx.ExtraJumpsLeft -= 1;
                        SwitchState(factory.Jumping());
                    }
                    else {
                        ctx.JumpBufferedCounter = ctx.JumpBufferedCounterMax;
                    }
                }
            }
            ctx.InputJumpButtonPressed = false;
        }
        else if (ctx.IsGrounded()) {
            Vector3 oldVelocity = ctx.PlayerVelocity;
            ctx.PlayerVelocity = new Vector3(oldVelocity.x, -50f, oldVelocity.z);
            SwitchState(factory.Idle());
        }
        else if (ctx.IsSwimmer) {
            if (ctx.BelowSurface()) {
                SwitchState(factory.SwimmingIdle());
            }
        }
        else if (!ctx.IsSwimmer) {
            if (ctx.AboveSurface()) {
                SwitchState(factory.FloatingIdle());
            }
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
        ctx.Controller.Move(ctx.PlayerVelocity * Time.deltaTime); ;
    }
}
