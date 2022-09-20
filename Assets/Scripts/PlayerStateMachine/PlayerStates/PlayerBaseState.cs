using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState {
    //this is the abstract base for our states

    //variables
    protected PlayerStateMachineBase ctx;
    protected PlayerStateFactory factory;

    public PlayerBaseState(PlayerStateMachineBase currentContext, PlayerStateFactory playerStateFactory) {
        ctx = currentContext;
        factory = playerStateFactory;
    }

    //Abstract functions that MUST be defined inside all child classes
    public abstract void EnterState();  //called when we first enter a state
                                        //best location for setting our animation

    public abstract void UpdateState(); //called every frame from the state we are currently in

    public abstract void ExitState(); //called when leaving the state

    public abstract void CheckSwitchStates(); //here we will put all of our conditions for switching to other states
                                              //the states we can switch to depend on the state we are currently in

    protected void SwitchState(PlayerBaseState newState) {
        ExitState();
        newState.EnterState();
        ctx.CurrentPlayerState = newState;
    }
}
