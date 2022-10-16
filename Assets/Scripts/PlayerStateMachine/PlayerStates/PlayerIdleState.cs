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

        if (ctx.PlayerVelocity.y < -9f) {
            Vector3 oldVelocity = ctx.PlayerVelocity;
            ctx.PlayerVelocity = new Vector3(oldVelocity.x, oldVelocity.y + (9f * Time.deltaTime), oldVelocity.z);
        }
        if (ctx.MovementInputEnabled) {
            ctx.UnchangedMove = ctx.GetMoveInput();
        }
        else {
            ctx.UnchangedMove = Vector3.zero;
        }
        ctx.Move = ctx.UnchangedMove;

        UpdateGravity();
        CheckSwitchStates();
    }

    public override void ExitState() {

    }

    public override void CheckSwitchStates() {
        if (ctx.InputJumpButtonPressed || ctx.JumpBufferedCounter > 0f) { //jump if pressed
            if (Time.time > ctx.NextJumpTime) {
                SwitchState(factory.Jumping());
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
        float newYVelocity = ctx.PlayerVelocity.y + ctx.CurrentGravityValue * Time.deltaTime;
        ctx.PlayerVelocity = new Vector3(ctx.PlayerVelocity.x, newYVelocity, ctx.PlayerVelocity.z);
        ctx.Controller.Move(ctx.PlayerVelocity * Time.deltaTime);
    }
}
