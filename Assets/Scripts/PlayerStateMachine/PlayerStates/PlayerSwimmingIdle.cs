using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwimmingIdleState : PlayerBaseState {

    /*protected float fallTimeRequired = .2f;
    protected bool notGroundedTimerStarted = false;
    protected float notGroundedStartTime = 0f;
    protected bool notGroundedTimerFinished = false;*/

    public PlayerSwimmingIdleState(PlayerStateMachineBase currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) {

    }

    public override void EnterState() {
        ctx.EPlayerState = EPlayerState.SwimmingIdle;
        //Debug.Log("Hello from the SwimmingIdle State");

        ctx.Controller.enableOverlapRecovery = true;
        ctx.ExtraJumpsLeft = ctx.ExtraJumps; //Reset our extra jumps count
        ctx.Controller.stepOffset = ctx.OriginalStepOffset;

        // Make gravity 0
        ctx.PlayerVelocity = new Vector3(ctx.PlayerVelocity.x, 0, ctx.PlayerVelocity.z);
        if (ctx.IsSwimmer) {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("WaterPlayer"), LayerMask.NameToLayer("WaterSurface"), false);
        }
        ctx.CurrentPlayerSpeed = ctx.PlayerSwimSpeed;
    }

    public override void UpdateState() {
        //Debug.Log("We SwimIdling");
        if (ctx.MovementInputEnabled) {
            ctx.UnchangedMove = ctx.GetMoveInput();
        }
        else {
            ctx.UnchangedMove = Vector3.zero;
        }
        ctx.Move = ctx.UnchangedMove;

        CheckSwitchStates();
    }

    public override void ExitState() {
        ctx.CurrentPlayerSpeed = ctx.BasePlayerSpeed;
    }

    public override void CheckSwitchStates() {
        if (ctx.InputJumpButtonPressed || ctx.JumpBufferedCounter > 0f) { //jump if pressed
            // Need to cast up and see if we are close enough to the surface to jump from the water
            if (ctx.BelowSurface()) {
                if (Time.time > ctx.NextJumpTime) {
                    SwitchState(factory.Jumping(ctx.SwimJumpScalar));
                }
            }
            ctx.InputJumpButtonPressed = false;
        }
        else if (ctx.Move != Vector3.zero) { //switch to swimming
            SwitchState(factory.Swimming());
        }
        else {
            
        }
    }
}

