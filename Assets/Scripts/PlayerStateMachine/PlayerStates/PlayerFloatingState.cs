using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFloatingState : PlayerBaseState {

    public PlayerFloatingState(PlayerStateMachineBase currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) {

    }

    public override void EnterState() {
        ctx.EPlayerState = EPlayerState.Floating;
        //Debug.Log("Hello from the Floating State");

        ctx.Controller.enableOverlapRecovery = true;
        ctx.ExtraJumpsLeft = ctx.ExtraJumps; //Reset our extra jumps count
        ctx.Controller.stepOffset = ctx.OriginalStepOffset;

        // Make gravity 0
        ctx.PlayerVelocity = new Vector3(ctx.PlayerVelocity.x, 0, ctx.PlayerVelocity.z);
        ctx.CurrentPlayerSpeed = ctx.PlayerSwimSpeed;
        ctx.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
    }

    public override void UpdateState() {
        //Debug.Log("We Floating");

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
        ctx.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public override void CheckSwitchStates() {
        if (ctx.InputJumpButtonPressed) { //jump if pressed
            // Need to cast up and see if we are close enough to the surface to jump from the water
            /*if (Time.time > ctx.NextJumpTime) {
                SwitchState(factory.Jumping());
            }*/
        }
        else if (ctx.JumpBufferedCounter > 0f) { //jumpo if buffered
            // Need to cast up and see if we are close enough to the surface to jump from the water
            //SwitchState(factory.Jumping());
        }
        else if (ctx.Move == Vector3.zero) { //switch to floating idle
            SwitchState(factory.FloatingIdle());
        }
        else {
            //notGroundedTimerStarted = false;
        }
    }

    //Non-State Machine related Functions
    private void MovePlayer() {
        if (ctx.InputJumpButtonPressed || ctx.JumpBufferedCounter > 0f) { //jump if pressed
            if (Time.time > ctx.NextJumpTime) {
                SwitchState(factory.Jumping(ctx.SwimJumpHeight));
            }
            ctx.InputJumpButtonPressed = false;
        }
        else if (ctx.Move.x > 0) {
            ctx.Controller.Move(Vector3.right * Mathf.Abs(ctx.Move.x) * Time.deltaTime * ctx.CurrentPlayerSpeed);
        }
        else if (ctx.Move.x < 0) {
            ctx.Controller.Move(Vector3.left * Mathf.Abs(ctx.Move.x) * Time.deltaTime * ctx.CurrentPlayerSpeed);
        }
        else {
            ctx.Controller.Move(Vector3.zero);
        }
    }
}
