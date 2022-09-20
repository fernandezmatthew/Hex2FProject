using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRecoilState : PlayerBaseState {
    public PlayerRecoilState(PlayerStateMachineBase currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) {

    }

    public override void EnterState() {
        ctx.EPlayerState = EPlayerState.Recoil;
        //Debug.Log("Hello from the Recoil State");
    }

    public override void UpdateState() {
        //Debug.Log("We Recoiling");
        CheckSwitchStates();
    }

    public override void ExitState() {

    }

    public override void CheckSwitchStates() {

    }
}
