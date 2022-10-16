using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFloatingIdleState : PlayerBaseState {

    /*protected float fallTimeRequired = .2f;
    protected bool notGroundedTimerStarted = false;
    protected float notGroundedStartTime = 0f;
    protected bool notGroundedTimerFinished = false;*/

    public PlayerFloatingIdleState(PlayerStateMachineBase currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) {

    }

    public override void EnterState() {
        ctx.EPlayerState = EPlayerState.FloatingIdle;
        //Debug.Log("Hello from the FloatingIdle State");

        ctx.Controller.enableOverlapRecovery = true;
        ctx.ExtraJumpsLeft = ctx.ExtraJumps; //Reset our extra jumps count
        ctx.Controller.stepOffset = ctx.OriginalStepOffset;

        // Make gravity 0
        ctx.PlayerVelocity = new Vector3(ctx.PlayerVelocity.x, 0, ctx.PlayerVelocity.z);
        ctx.CurrentPlayerSpeed = ctx.PlayerSwimSpeed;
        //ctx.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
    }

    public override void UpdateState() {
        //Debug.Log("We FloatIdling");
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
        ctx.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public override void CheckSwitchStates() {
        if (ctx.InputJumpButtonPressed || ctx.JumpBufferedCounter > 0f) { //jump if pressed
            if (Time.time > ctx.NextJumpTime) {
                SwitchState(factory.Jumping(ctx.SwimJumpScalar));
            }
            ctx.InputJumpButtonPressed = false;
        }
        else if (ctx.Move != Vector3.zero) { //switch to floating
            SwitchState(factory.Floating());
        }
        else {

        }
    }
}

