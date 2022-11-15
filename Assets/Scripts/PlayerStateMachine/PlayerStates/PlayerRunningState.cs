using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunningState : PlayerBaseState {

    /*protected float fallTimeRequired = .2f;
    protected bool notGroundedTimerStarted = false;
    protected float notGroundedStartTime = 0f;
    protected bool notGroundedTimerFinished = false;*/

    public PlayerRunningState(PlayerStateMachineBase currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) {

    }

    public override void EnterState() {
        ctx.EPlayerState = EPlayerState.Running;
        //Debug.Log("Hello from the Running State");

        ctx.Controller.enableOverlapRecovery = true;
        ctx.ExtraJumpsLeft = ctx.ExtraJumps; //Reset our extra jumps count
        ctx.Controller.stepOffset = ctx.OriginalStepOffset;

        ctx.CurrentGravityValue = ctx.GroundedGravity;
    }

    public override void UpdateState() {
        //Debug.Log("We Running");

        ctx.UnchangedMove = ctx.GetMoveInput(); // dont need this line
        ctx.Move = ctx.UnchangedMove;

        UpdateGravity();
        MovePlayer();
        ctx.UpdateRotation2D();

        CheckSwitchStates();
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
            /*if (!notGroundedTimerStarted) {
                notGroundedTimerStarted = true;
                notGroundedStartTime = Time.time;
            }
            else {
                if (Time.time - notGroundedStartTime >= fallTimeRequired) {
                    Debug.Log("Timer Finished!");
                    SwitchState(factory.Falling());
                }
            }*/
            SwitchState(factory.Falling());
        }
        else if (ctx.Move == Vector3.zero) { //switch to idle
            SwitchState(factory.Idle());
        }
        else {
            //notGroundedTimerStarted = false;
        }
    }

    //Non-State Machine related Functions
    private void MovePlayer() {
        // make character turn here
        if (ctx.Move.x < 0) {
            ctx.Controller.Move(Vector3.left * Mathf.Abs(ctx.Move.x) * Time.deltaTime * ctx.CurrentPlayerSpeed);
            ctx.IsFacingRight = false;
        }
        else if (ctx.Move.x > 0) {
            ctx.Controller.Move(Vector3.right * Mathf.Abs(ctx.Move.x) * Time.deltaTime * ctx.CurrentPlayerSpeed);
            ctx.IsFacingRight = true;
        }
        else {
            ctx.Controller.Move(Vector3.zero);
        }
    }

    private void UpdateGravity() {
        ctx.PlayerVelocity = new Vector3(ctx.PlayerVelocity.x, ctx.CurrentGravityValue, ctx.PlayerVelocity.z);
        ctx.Controller.Move(ctx.PlayerVelocity * Time.deltaTime);
    }
}
