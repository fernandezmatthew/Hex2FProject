using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : PlayerBaseState {

    /*protected float fallTimeRequired = .2f;
    protected bool notGroundedTimerStarted = false;
    protected float notGroundedStartTime = 0f;
    protected bool notGroundedTimerFinished = false;*/

    public PlayerDeadState(PlayerStateMachineBase currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) {

    }

    public override void EnterState() {
        ctx.EPlayerState = EPlayerState.Dead;
        //Debug.Log("Hello from the Dead State");
        //start death logic here
    }

    public override void UpdateState() {
        //Debug.Log("We Dead");

        // I dont think we need anything here
    }

    public override void ExitState() {

    }

    public override void CheckSwitchStates() {
        // dont think we need anything here either
    }
}
