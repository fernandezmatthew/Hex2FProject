public class PlayerStateFactory {
    //This class allows us to access every state within our machine, FROM any state within our machine
    //It also gives each state access to the context, which is the player information itself.
    PlayerStateMachineBase context;

    public PlayerStateFactory(PlayerStateMachineBase currentContext) {
        //Factory is initialized with the context so that it can pass it to all of the states.
        context = currentContext;
    }

    //These functions initialize a new state object, giving it access to the context, and the factory itself.
    //This also serves as a list of all the states within this state machine.

    //More states to define:
    
    public PlayerBaseState Idle() {
        return new PlayerIdleState(context, this);
    }
    public PlayerBaseState Falling() {
        return new PlayerFallingState(context, this);
    }
    public PlayerBaseState Running() {
        return new PlayerRunningState(context, this);
    }
    public PlayerBaseState Jumping() {
        //used a third parameter "isJumpButtonDown" in first version
        return new PlayerJumpingState(context, this);
    }
    public PlayerBaseState Walking() {
        return new PlayerWalkingState(context, this);
    }
    public PlayerBaseState Swimming() {
        return new PlayerSwimmingState(context, this);
    }
    public PlayerBaseState SwimmingIdle() {
        return new PlayerSwimmingIdleState(context, this);
    }
    public PlayerBaseState Floating() {
        return new PlayerFloatingState(context, this);
    }
    public PlayerBaseState FloatingIdle() {
        return new PlayerFloatingIdleState(context, this);
    }
    // add floating and swimming states
    // maybe swimming idle?
    // or we can have a boolean that tells us if we're the land guy or the water guy and that can constrain movement within the same states
}
