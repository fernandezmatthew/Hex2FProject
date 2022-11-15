using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState {

    /*protected float fallTimeRequired = .2f;
    protected bool notGroundedTimerStarted = false;
    protected float notGroundedStartTime = 0f;
    protected bool notGroundedTimerFinished = false;*/

    public PlayerIdleState(PlayerStateMachineBase currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) {

    }

    public override void EnterState() {
        ctx.EPlayerState = EPlayerState.Idle;
        //Debug.Log("Hello from the Idle State");

        ctx.Controller.enableOverlapRecovery = true;
        ctx.ExtraJumpsLeft = ctx.ExtraJumps; //Reset our extra jumps count
        ctx.Controller.stepOffset = ctx.OriginalStepOffset;

        ctx.CurrentGravityValue = ctx.GroundedGravity;
    }

    public override void UpdateState() {
        //Debug.Log("We Idling");

        ctx.UnchangedMove = ctx.GetMoveInput();
        ctx.Move = ctx.UnchangedMove;

        UpdateGravity();
        CheckSwitchStates();
        ctx.bumpingHead();
    }

    public override void ExitState() {

    }

    public override void CheckSwitchStates() {
        base.CheckSwitchStates();
        if (ctx.CurrentPlayerState != this) {
            // If we switched states within base, exit this function now
            return;
        }

        if (ctx.InputJumpButtonPressed || ctx.JumpBufferedCounter > 0f) { //jump if pressed
            if (ctx.MovementInputEnabled) {
                if (!ctx.bumpingHead()) {
                    if (Time.time > ctx.NextJumpTime) {
                        SwitchState(factory.Jumping());
                    }
                }
            }
            ctx.InputJumpButtonPressed = false;
        }
        else if (!ctx.IsGrounded()) { //chgange to falling
            SwitchState(factory.Falling());
        }
        else if (Mathf.Abs(ctx.Move.x) != 0) { //switch to either walking or running
            SwitchState(factory.Running());
        }
        else {
            //notGroundedTimerStarted = false;
        }
    }

    private void UpdateGravity() {
        ctx.PlayerVelocity = new Vector3(ctx.PlayerVelocity.x, ctx.CurrentGravityValue, ctx.PlayerVelocity.z);
        ctx.Controller.Move(ctx.PlayerVelocity * Time.deltaTime);
    }
}
