using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwimmingState : PlayerBaseState {

    public PlayerSwimmingState(PlayerStateMachineBase currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) {

    }

    public override void EnterState() {
        ctx.EPlayerState = EPlayerState.Swimming;
        //Debug.Log("Hello from the Swimming State");

        ctx.Controller.enableOverlapRecovery = true;
        ctx.ExtraJumpsLeft = ctx.ExtraJumps; //Reset our extra jumps count
        ctx.Controller.stepOffset = ctx.OriginalStepOffset;

        // Make gravity 0
        ctx.PlayerVelocity = new Vector3(ctx.PlayerVelocity.x, 0, ctx.PlayerVelocity.z);
        ctx.CurrentPlayerSpeed = ctx.PlayerSwimSpeed;
    }

    public override void UpdateState() {
        //Debug.Log("We Swimming");

        ctx.UnchangedMove = ctx.GetMoveInput();
        ctx.Move = ctx.UnchangedMove;
        MovePlayer();
        ctx.UpdateRotation2D();

        CheckSwitchStates();
    }

    public override void ExitState() {
        ctx.CurrentPlayerSpeed = ctx.BasePlayerSpeed;
    }

    public override void CheckSwitchStates() {
        base.CheckSwitchStates();
        if (ctx.CurrentPlayerState != this) {
            // If we switched states within base, exit this function now
            return;
        }

        if (ctx.InputJumpButtonPressed || ctx.JumpBufferedCounter > 0f) { //jump if pressed
            if (ctx.MovementInputEnabled) {
                if (ctx.BelowSurface()) {
                    if (!ctx.bumpingHead()) {
                        if (Time.time > ctx.NextJumpTime) {
                            SwitchState(factory.Jumping(ctx.SwimJumpScalar));
                        }
                    }
                }
            }
            // Need to cast up and see if we are close enough to the surface to jump from the water
            ctx.InputJumpButtonPressed = false;
        }
        else if (ctx.Move == Vector3.zero) { //switch to swimming idle
            SwitchState(factory.SwimmingIdle());
        }
        else {
            //notGroundedTimerStarted = false;
        }
    }

    //Non-State Machine related Functions
    private void MovePlayer() {
        if (ctx.Move.magnitude > 0) {
            ctx.Controller.Move(ctx.Move * Time.deltaTime * ctx.CurrentPlayerSpeed);
        }

        if (ctx.Move.x > 0) {
            ctx.IsFacingRight = true;
        }
        else if (ctx.Move.x < 0) {
            ctx.IsFacingRight = false;
        }
        else {
            ctx.Controller.Move(Vector3.zero);
        }
    }
}
