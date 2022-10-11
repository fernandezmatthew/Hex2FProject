using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachineProto : PlayerStateMachineBase
{
    protected override void Awake() {
        base.Awake();
        //Setup Default State
        playerStates = new PlayerStateFactory(this);
        if (!isSwimmer) {
            currentPlayerState = playerStates.Idle();
        }
        else {
            currentPlayerState = playerStates.SwimmingIdle();
        }
        // These lines seems very out of place here
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("LandPlayer"), LayerMask.NameToLayer("WaterSurface"), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("WaterPlayer"), LayerMask.NameToLayer("WaterSurface2"), true);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("WaterPlayer"), LayerMask.NameToLayer("LandPlayer"), true);

        currentPlayerState.EnterState();
    }

    protected void Update() {
        //Update the currentState
        currentPlayerState.UpdateState();

        // see on surfcase rays
        /*float heightThreshold = .2f + controller.skinWidth;
        Debug.DrawRay(controller.bounds.center, Vector3.up * (controller.bounds.extents.y + heightThreshold), Color.green);
        Debug.DrawRay(controller.bounds.center, Vector3.down * (controller.bounds.extents.y + heightThreshold), Color.green);*/

        //Set current animation
        //anim.SetInteger("playerState", (int)ePlayerState);

        /*float rotateCamX = Input.GetAxisRaw("RotateCamX");
        float rotateCamY = Input.GetAxisRaw("RotateCamY");
        Debug.Log("x = " + rotateCamX + " y = " + rotateCamY);*/


        //Debug Commands
        Debug.Log(ePlayerState);
        //Debug.Log(playerVelocity.y);
        //Debug.Log(IsGrounded());
    }
}
