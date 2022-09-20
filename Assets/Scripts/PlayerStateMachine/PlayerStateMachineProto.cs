using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachineProto : PlayerStateMachineBase
{
    protected override void Awake() {
        base.Awake();
        //Setup Default State
        playerStates = new PlayerStateFactory(this);
        currentPlayerState = playerStates.Idle();
        currentPlayerState.EnterState();
    }

    protected void Update() {
        //Update the currentState
        currentPlayerState.UpdateState();

        //Set current animation
        anim.SetInteger("playerState", (int)ePlayerState);

        /*float rotateCamX = Input.GetAxisRaw("RotateCamX");
        float rotateCamY = Input.GetAxisRaw("RotateCamY");
        Debug.Log("x = " + rotateCamX + " y = " + rotateCamY);*/


        //Debug Commands
        Debug.Log(ePlayerState);
        //Debug.Log(playerVelocity.y);
        //Debug.Log(IsGrounded());
    }
}
