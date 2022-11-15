using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOT USING FOR HEX2F
public class PlayerWalkingState : PlayerBaseState {

    /*protected float fallTimeRequired = .2f;
    protected bool notGroundedTimerStarted = false;
    protected float notGroundedStartTime = 0f;
    protected bool notGroundedTimerFinished = false;*/

    public PlayerWalkingState(PlayerStateMachineBase currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) {

    }

    public override void EnterState() {
        ctx.EPlayerState = EPlayerState.Walking;
        //Debug.Log("Hello from the Walking State");

        ctx.Controller.enableOverlapRecovery = true;
        ctx.ExtraJumpsLeft = ctx.ExtraJumps; //Reset our extra jumps count
        ctx.Controller.stepOffset = ctx.OriginalStepOffset;

        ctx.CurrentGravityValue = ctx.GroundedGravity;
    }

    public override void UpdateState() {
        //Debug.Log("We Walking");

        ctx.UnchangedMove = ctx.GetMoveInput();
        ctx.Move = ctx.UnchangedMove;

        MovePlayer();
        ctx.UpdateRotation2D();
        UpdateGravity();

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

        if (ctx.InputJumpButtonPressed || ctx.JumpBufferedCounter > 0f) { //jump if jump is pressed
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
        else if (ctx.UnchangedMove.magnitude > ctx.RunThreshold) {
            SwitchState(factory.Running());
        }
        else if (ctx.Move == Vector3.zero) { //change to idle
            SwitchState(factory.Idle());
        }
        else {
            //notGroundedTimerStarted = false;
        }
    }

    //Non-State Machine related Functions
    private void MovePlayer() {
        if (ctx.Move.x < 0) {
            ctx.Controller.Move(Vector3.left * Time.deltaTime * ctx.CurrentPlayerSpeed);
            ctx.IsFacingRight = false;
        }
        else if (ctx.Move.x > 0) {
            ctx.Controller.Move(Vector3.right * Time.deltaTime * ctx.CurrentPlayerSpeed);
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
