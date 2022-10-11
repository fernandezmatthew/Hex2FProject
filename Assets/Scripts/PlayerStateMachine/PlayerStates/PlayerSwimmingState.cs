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

        /*if (ctx.PlayerVelocity.y < -9f) {
            Vector3 oldVelocity = ctx.PlayerVelocity;
            ctx.PlayerVelocity = new Vector3(oldVelocity.x, oldVelocity.y + (9f * Time.deltaTime), oldVelocity.z);
        }*/
        if (ctx.MovementInputEnabled) {
            ctx.UnchangedMove = ctx.GetMoveInput();
        }
        else {
            ctx.UnchangedMove = Vector3.zero;
        }
        ctx.Move = ctx.UnchangedMove;
        MovePlayer();

        CheckSwitchStates();
    }

    public override void ExitState() {
        ctx.CurrentPlayerSpeed = ctx.BasePlayerSpeed;
    }

    public override void CheckSwitchStates() {
        if (Input.GetButtonDown("Jump")) { //jump if pressed
            // Need to cast up and see if we are close enough to the surface to jump from the water
            /*if (Time.time > ctx.NextJumpTime) {
                SwitchState(factory.Jumping());
            }*/
        }
        else if (ctx.JumpBufferedCounter > 0f) { //jumpo if buffered
            // Need to cast up and see if we are close enough to the surface to jump from the water
            //SwitchState(factory.Jumping());
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
        if (Input.GetButtonDown("Jump") || ctx.JumpBufferedCounter > 0f) { //jump if pressed
            // Need to cast up and see if we are close enough to the surface to jump from the water
            if (ctx.BelowSurface()) {
                if (Time.time > ctx.NextJumpTime) {
                    SwitchState(factory.Jumping(ctx.SwimJumpHeight));
                }
            }
        }
        else if (ctx.Move.magnitude > 0) {
            ctx.Controller.Move(ctx.Move * Time.deltaTime * ctx.CurrentPlayerSpeed);
        }
        else {
            ctx.Controller.Move(Vector3.zero);
        }
    }
}