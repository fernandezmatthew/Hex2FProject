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
    }

    public override void UpdateState() {
        //Debug.Log("We Running");

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
        ctx.Move = ctx.SetMoveRelativeToCamera();
        ctx.UpdateRotation();
        ctx.UpdateGravity();
        MovePlayer();

        CheckSwitchStates();
    }

    public override void ExitState() {

    }

    public override void CheckSwitchStates() {
        if (Input.GetButtonDown("Jump")) { //jump if pressed
            if (Time.time > ctx.NextJumpTime) {
                SwitchState(factory.Jumping());
            }
        }
        else if (ctx.JumpBufferedCounter > 0f) { //jumpo if buffered
            SwitchState(factory.Jumping());
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
        else if (ctx.UnchangedMove.magnitude < ctx.RunThreshold) {
            SwitchState(factory.Walking());
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
        ctx.Controller.Move(ctx.transform.forward * Time.deltaTime * ctx.CurrentPlayerSpeed);
    }
}
