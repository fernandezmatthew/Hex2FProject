using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerStateMachineBase : MonoBehaviour {
    //Setup Default State within the Lowest Level of the Context

    public bool died = false; // make this protected once we have death conditions
    public bool movementInputEnabled = true;
    //References
    [SerializeField] protected LayerMask groundedLayers;
    [SerializeField] protected LayerMask enemyLayers;
    [SerializeField] protected Transform cam;
    protected CharacterController controller;
    protected Animator anim;

    //VISIBLE
    /********************************************************************/

    [SerializeField] protected int playerIndex = 0;
    [SerializeField] protected bool isSwimmer = false;
    [SerializeField] protected float basePlayerSpeed = 12f;
    [SerializeField] protected float playerSwimSpeed = 12f;
    [SerializeField] protected float fallingGravityMultiplier = 2f; //Strength of gravity on this object
    [SerializeField] protected bool holdJump = false;
    [SerializeField] protected float maxJumpHeight = 8f;
    [SerializeField] protected float minJumpHeight = 4f; //Only used if holdJump is true. If holdJump is true but you always reach maxHeight, try reducing the minJumpHoldTime
    [SerializeField] protected float maxJumpDuration = 1f;
    [SerializeField] protected float minJumpHoldTime = .1f;
    [SerializeField] protected float swimJumpScalar = 1f; //Does not accurately multiply the height of the original jump
    [SerializeField] protected float airSpeedRatio = .75f; //Decreases player speed in the air
    [SerializeField] protected float crouchSpeedRatio = .75f; //Decreases player speed while crouched
    [SerializeField] protected int extraJumps = 0;
    [SerializeField] protected float jumpBufferedCounterMax = .05f;
    [SerializeField] protected float terminalVelocity = 10000f; //Caps our fall speed. //Eventually make this not visible from editor

    //PRIVATES
    /********************************************************************/

    //Input
    protected Vector2 inputVector;
    protected bool inputJumpButtonPressed;
    protected bool inputJumpButtonHeld;

    //Movement
    protected Vector3 playerVelocity; //vector used for vertical physics
    protected Vector3 unchangedMove;
    protected Vector3 move;
    protected float currentPlayerSpeed;
    protected float turnSmoothTime; //Smooths the object's turning
    protected float turnSmoothVelocity; //Also smooths the object's turning
    protected float runThreshold;

    //Enables
    //EDIT: PUTBACK protected bool movementInputEnabled;

    //Booleans
    protected bool isFacingRight;
    //EDIT: PUTBACK protected bool died;

    //Gravities
    protected float groundedGravity; // Gravity while grounded
    protected float jumpingGravityButtonHeld; // Gravity while jumping and holding the jump button
    protected float jumpingGravityButtonReleased; // Gravity while jumping with jump button released
    protected float fallingGravity;
    protected float currentGravityValue; // the current gravity (Will be one of the predefined values)

    //Jumping
    protected float initialJumpVelocity;
    protected float originalStepOffset; //Helps us not jitter when jumping onto a ledge;
    protected float jumpRate; //Stops player from being able to accidentally double jump when spamming jump key. Measured in Hz.
    protected float nextJumpTime; //Works with previous variable
    protected int extraJumpsLeft; //Keeps track of how many more jumps we can use before touching the ground again
    protected float jumpButtonHoldTimer;
    protected float jumpBufferedCounter;
    protected float isGroundedCounter;
    protected float isGroundedCounterMax;
    protected bool jumpButtonReleased;

    //State Variables
    protected PlayerBaseState currentPlayerState;
    protected PlayerStateFactory playerStates;
    protected EPlayerState ePlayerState;

    //GETTERS AND SETTERS
    /********************************************************************/

    //References
    public CharacterController Controller { get { return controller; } set { controller = value; } }

    //State Machine
    public PlayerBaseState CurrentPlayerState { get { return currentPlayerState; } set { currentPlayerState = value; } }
    public EPlayerState EPlayerState { get { return ePlayerState; } set { ePlayerState = value; } }

    //Input
    public Vector2 InputVector { get { return inputVector; } set { inputVector = value; } }
    public bool InputJumpButtonPressed { get { return inputJumpButtonPressed; } set { inputJumpButtonPressed = value; } }
    public bool InputJumpButtonHeld { get { return inputJumpButtonHeld; } set { inputJumpButtonHeld = value; } }
    public int PlayerIndex { get { return playerIndex; } set { playerIndex = value; } }

    //Movement
    public Vector3 PlayerVelocity { get { return playerVelocity; } set { playerVelocity = value; } }
    public Vector3 Move { get { return move; } set { move = value; } }
    public Vector3 UnchangedMove { get { return unchangedMove; } set { unchangedMove = value; } }
    public float BasePlayerSpeed { get { return basePlayerSpeed; } set { basePlayerSpeed = value; } }
    public float CurrentPlayerSpeed { get { return currentPlayerSpeed; } set { currentPlayerSpeed = value; } }
    public float PlayerSwimSpeed { get { return playerSwimSpeed; } set { playerSwimSpeed = value; } }
    public float JumpBufferedCounter { get { return jumpBufferedCounter; } set { jumpBufferedCounter = value; } }
    public float JumpBufferedCounterMax { get { return jumpBufferedCounterMax; } set { jumpBufferedCounterMax = value; } }
    public float AirSpeedRatio { get { return airSpeedRatio; } set { airSpeedRatio = value; } }
    public float RunThreshold { get { return runThreshold; } set { runThreshold = value; } }

    //Enables
    public bool MovementInputEnabled { get { return movementInputEnabled; } set { movementInputEnabled = value; } }

    //Booleans
    public bool IsSwimmer { get { return isSwimmer; } set { isSwimmer = value; } }
    public bool IsFacingRight { get { return isFacingRight; } set { isFacingRight = value; } }
    public bool Died { get { return died; } set { died = value; } }

    //Gravities
    public float GroundedGravity { get { return groundedGravity; } set { groundedGravity = value; } }
    public float JumpingGravityButtonHeld { get { return jumpingGravityButtonHeld; } set { jumpingGravityButtonHeld = value;} }
    public float JumpingGravityButtonReleased { get { return jumpingGravityButtonReleased; } set { jumpingGravityButtonReleased = value; } }
    public float FallingGravity { get { return fallingGravity; } set { fallingGravity = value; } }
    public float CurrentGravityValue { get { return currentGravityValue; } set { currentGravityValue = value; } }
    public float TerminalVelocity { get { return terminalVelocity; } set { terminalVelocity = value; } }

    //Jumping
    public float InitialJumpVelocity { get { return initialJumpVelocity; } set { initialJumpVelocity = value; } }
    public int ExtraJumpsLeft { get { return extraJumpsLeft; } set { extraJumpsLeft = value; } }
    public int ExtraJumps { get { return extraJumps; } set { extraJumps = value; } }
    public float OriginalStepOffset { get { return originalStepOffset; } set { originalStepOffset = value; } }
    public float NextJumpTime { get { return nextJumpTime; } set { nextJumpTime = value; } }
    public float MaxJumpHeight { get { return maxJumpHeight; } set { maxJumpHeight = value; } }
    public float MinJumpHeight { get { return minJumpHeight; } set { minJumpHeight = value; } }
    public float MaxJumpDuration { get { return maxJumpDuration; } set { maxJumpDuration = value; } }
    public float MinJumpHoldTime { get { return minJumpHoldTime; } set { minJumpHoldTime = value; } }
    public float SwimJumpScalar { get { return swimJumpScalar; } set { swimJumpScalar = value; } }
    public float JumpRate { get { return jumpRate; } set { jumpRate = value; } }
    public float JumpButtonHoldTimer { get { return jumpButtonHoldTimer; } set { jumpButtonHoldTimer = value; } }
    public bool HoldJump { get { return holdJump; } set { holdJump = value; } }
    public bool JumpButtonReleased { get { return jumpButtonReleased; } set { jumpButtonReleased = value; } }

    protected virtual void Awake() {
        //INITIALIZE VARIABLES // EDIT: A lot of variables havent been initialized yet
        /********************************************************************/

        //References
        controller = gameObject.GetComponent<CharacterController>(); //would like to disable the capsule connected to this
        anim = GetComponentInChildren<Animator>();
        /*anim = GetComponent<Animator>();
        if (anim == null) {
            anim = GetComponentInChildren<Animator>();
        }*/

        //State Machine
        ePlayerState = EPlayerState.Default;

        //Input
        inputVector = Vector2.zero;
        inputJumpButtonPressed = false;

        //Movement
        movementInputEnabled = true;
        playerVelocity = Vector3.zero;
        move = Vector3.zero;
        currentPlayerSpeed = basePlayerSpeed;
        turnSmoothTime = 0.1f;
        runThreshold = .85f;

        //Enables

        //Booleans
        isFacingRight = true;
        died = false;

        //Gravities
        groundedGravity = -9.8f;
        currentGravityValue = groundedGravity;

        //Jumping
        jumpRate = 8f;
        nextJumpTime = 0f;
        extraJumpsLeft = extraJumps;
        originalStepOffset = controller.stepOffset;
        jumpBufferedCounter = 0f;
        isGroundedCounter = 0f;
        isGroundedCounterMax = .05f;
        jumpButtonReleased = true;
        InitializeJumpVariables();
    }

    public virtual bool IsGrounded() {
        float heightThreshold = .2f + controller.skinWidth;
        if (playerVelocity.y > 0) {
            return false;
        }

        //Cast 9 rays to check groundedness. isGrounded if any are true
        bool[] hits = new bool[9];
        bool hit = false;

        if (groundedLayers != LayerMask.GetMask("Nothing")) {
            //Center
            hits[0] = Physics.Raycast(controller.bounds.center, Vector3.down, controller.bounds.extents.y + heightThreshold, groundedLayers);
            //Center of sides
            hits[1] = Physics.Raycast(controller.bounds.center + new Vector3(controller.bounds.extents.x, 0, 0), Vector3.down, controller.bounds.extents.y + heightThreshold, groundedLayers);
            hits[2] = Physics.Raycast(controller.bounds.center - new Vector3(controller.bounds.extents.x, 0, 0), Vector3.down, controller.bounds.extents.y + heightThreshold, groundedLayers);
            hits[3] = Physics.Raycast(controller.bounds.center + new Vector3(0, 0, controller.bounds.extents.z), Vector3.down, controller.bounds.extents.y + heightThreshold, groundedLayers);
            hits[4] = Physics.Raycast(controller.bounds.center - new Vector3(0, 0, controller.bounds.extents.z), Vector3.down, controller.bounds.extents.y + heightThreshold, groundedLayers);
            //Corners
            hits[5] = Physics.Raycast(controller.bounds.center + new Vector3(controller.bounds.extents.x, 0, controller.bounds.extents.z), Vector3.down, controller.bounds.extents.y + heightThreshold, groundedLayers);
            hits[6] = Physics.Raycast(controller.bounds.center + new Vector3(controller.bounds.extents.x, 0, -1f * controller.bounds.extents.z), Vector3.down, controller.bounds.extents.y + heightThreshold, groundedLayers);
            hits[7] = Physics.Raycast(controller.bounds.center + new Vector3(-1f * controller.bounds.extents.x, 0, controller.bounds.extents.z), Vector3.down, controller.bounds.extents.y + heightThreshold, groundedLayers);
            hits[8] = Physics.Raycast(controller.bounds.center + new Vector3(-1f * controller.bounds.extents.x, 0, -1f * controller.bounds.extents.z), Vector3.down, controller.bounds.extents.y + heightThreshold, groundedLayers);
        }

        else {
            //Center
            hits[0] = Physics.Raycast(controller.bounds.center, Vector3.down, controller.bounds.extents.y + heightThreshold);
            //Center of sides
            hits[1] = Physics.Raycast(controller.bounds.center + new Vector3(controller.bounds.extents.x, 0, 0), Vector3.down, controller.bounds.extents.y + heightThreshold);
            hits[2] = Physics.Raycast(controller.bounds.center - new Vector3(controller.bounds.extents.x, 0, 0), Vector3.down, controller.bounds.extents.y + heightThreshold);
            hits[3] = Physics.Raycast(controller.bounds.center + new Vector3(0, 0, controller.bounds.extents.z), Vector3.down, controller.bounds.extents.y + heightThreshold);
            hits[4] = Physics.Raycast(controller.bounds.center - new Vector3(0, 0, controller.bounds.extents.z), Vector3.down, controller.bounds.extents.y + heightThreshold);
            //Corners
            hits[5] = Physics.Raycast(controller.bounds.center + new Vector3(controller.bounds.extents.x, 0, controller.bounds.extents.z), Vector3.down, controller.bounds.extents.y + heightThreshold);
            hits[6] = Physics.Raycast(controller.bounds.center + new Vector3(controller.bounds.extents.x, 0, -1f * controller.bounds.extents.z), Vector3.down, controller.bounds.extents.y + heightThreshold);
            hits[7] = Physics.Raycast(controller.bounds.center + new Vector3(-1f * controller.bounds.extents.x, 0, controller.bounds.extents.z), Vector3.down, controller.bounds.extents.y + heightThreshold);
            hits[8] = Physics.Raycast(controller.bounds.center + new Vector3(-1f * controller.bounds.extents.x, 0, -1f * controller.bounds.extents.z), Vector3.down, controller.bounds.extents.y + heightThreshold);
        }

        bool anyHits = false;
        for (int i = 0; i < hits.Length; i++) {
            if (hits[i]) {
                anyHits = true;
                break;
            }
        }

        /*//Debugging //Uncomment to see the 9 rays that check groundedness
        Color rayColor;
        if (hits[0]) {
            rayColor = Color.green;
        }
        else {
            rayColor = Color.red;
        }
        Debug.DrawRay(controller.bounds.center, Vector3.down * (controller.bounds.extents.y + heightThreshold), rayColor);

        if (hits[1]) {
            rayColor = Color.green;
        }
        else {
            rayColor = Color.red;
        }
        Debug.DrawRay(controller.bounds.center + new Vector3(controller.bounds.extents.x, 0, 0), Vector3.down * (controller.bounds.extents.y + heightThreshold), rayColor);

        if (hits[2]) {
            rayColor = Color.green;
        }
        else {
            rayColor = Color.red;
        }
        Debug.DrawRay(controller.bounds.center - new Vector3(controller.bounds.extents.x, 0, 0), Vector3.down * (controller.bounds.extents.y + heightThreshold), rayColor);

        if (hits[3]) {
            rayColor = Color.green;
        }
        else {
            rayColor = Color.red;
        }
        Debug.DrawRay(controller.bounds.center + new Vector3(0, 0, controller.bounds.extents.z), Vector3.down * (controller.bounds.extents.y + heightThreshold), rayColor);

        if (hits[4]) {
            rayColor = Color.green;
        }
        else {
            rayColor = Color.red;
        }
        Debug.DrawRay(controller.bounds.center - new Vector3(0, 0, controller.bounds.extents.z), Vector3.down * (controller.bounds.extents.y + heightThreshold), rayColor);

        if (hits[5]) {
            rayColor = Color.green;
        }
        else {
            rayColor = Color.red;
        }
        Debug.DrawRay(controller.bounds.center + new Vector3(controller.bounds.extents.x, 0, controller.bounds.extents.z), Vector3.down * (controller.bounds.extents.y + heightThreshold), rayColor);

        if (hits[6]) {
            rayColor = Color.green;
        }
        else {
            rayColor = Color.red;
        }
        Debug.DrawRay(controller.bounds.center + new Vector3(controller.bounds.extents.x, 0, -1f * controller.bounds.extents.z), Vector3.down * (controller.bounds.extents.y + heightThreshold), rayColor);

        if (hits[7]) {
            rayColor = Color.green;
        }
        else {
            rayColor = Color.red;
        }
        Debug.DrawRay(controller.bounds.center + new Vector3(-1f * controller.bounds.extents.x, 0, controller.bounds.extents.z), Vector3.down * (controller.bounds.extents.y + heightThreshold), rayColor);
        if (hits[8]) {
            rayColor = Color.green;
        }
        else {
            rayColor = Color.red;
        }
        Debug.DrawRay(controller.bounds.center + new Vector3(-1f * controller.bounds.extents.x, 0, -1f * controller.bounds.extents.z), Vector3.down * (controller.bounds.extents.y + heightThreshold), rayColor);
        //Endof Debugging*/

        if (anyHits) {
            controller.enableOverlapRecovery = true;
        }

        return anyHits;
    }

    public virtual bool bumpingHead() {
        float heightThreshold = .2f + controller.skinWidth;

        //Cast 9 rays to check if we bumped
        bool[] hits = new bool[9];
        bool hit = false;

        if (groundedLayers != LayerMask.GetMask("Nothing")) {
            //Center
            hits[0] = Physics.Raycast(controller.bounds.center, Vector3.up, controller.bounds.extents.y + heightThreshold, groundedLayers);
            //Center of sides
            hits[1] = Physics.Raycast(controller.bounds.center + new Vector3(controller.bounds.extents.x, 0, 0), Vector3.up, controller.bounds.extents.y + heightThreshold, groundedLayers);
            hits[2] = Physics.Raycast(controller.bounds.center - new Vector3(controller.bounds.extents.x, 0, 0), Vector3.up, controller.bounds.extents.y + heightThreshold, groundedLayers);
            hits[3] = Physics.Raycast(controller.bounds.center + new Vector3(0, 0, controller.bounds.extents.z), Vector3.up, controller.bounds.extents.y + heightThreshold, groundedLayers);
            hits[4] = Physics.Raycast(controller.bounds.center - new Vector3(0, 0, controller.bounds.extents.z), Vector3.up, controller.bounds.extents.y + heightThreshold, groundedLayers);
            //Corners
            hits[5] = Physics.Raycast(controller.bounds.center + new Vector3(controller.bounds.extents.x, 0, controller.bounds.extents.z), Vector3.up, controller.bounds.extents.y + heightThreshold, groundedLayers);
            hits[6] = Physics.Raycast(controller.bounds.center + new Vector3(controller.bounds.extents.x, 0, -1f * controller.bounds.extents.z), Vector3.up, controller.bounds.extents.y + heightThreshold, groundedLayers);
            hits[7] = Physics.Raycast(controller.bounds.center + new Vector3(-1f * controller.bounds.extents.x, 0, controller.bounds.extents.z), Vector3.up, controller.bounds.extents.y + heightThreshold, groundedLayers);
            hits[8] = Physics.Raycast(controller.bounds.center + new Vector3(-1f * controller.bounds.extents.x, 0, -1f * controller.bounds.extents.z), Vector3.up, controller.bounds.extents.y + heightThreshold, groundedLayers);
        }

        else {
            //Center
            hits[0] = Physics.Raycast(controller.bounds.center, Vector3.down, controller.bounds.extents.y + heightThreshold);
            //Center of sides
            hits[1] = Physics.Raycast(controller.bounds.center + new Vector3(controller.bounds.extents.x, 0, 0), Vector3.up, controller.bounds.extents.y + heightThreshold);
            hits[2] = Physics.Raycast(controller.bounds.center - new Vector3(controller.bounds.extents.x, 0, 0), Vector3.up, controller.bounds.extents.y + heightThreshold);
            hits[3] = Physics.Raycast(controller.bounds.center + new Vector3(0, 0, controller.bounds.extents.z), Vector3.up, controller.bounds.extents.y + heightThreshold);
            hits[4] = Physics.Raycast(controller.bounds.center - new Vector3(0, 0, controller.bounds.extents.z), Vector3.up, controller.bounds.extents.y + heightThreshold);
            //Corners
            hits[5] = Physics.Raycast(controller.bounds.center + new Vector3(controller.bounds.extents.x, 0, controller.bounds.extents.z), Vector3.up, controller.bounds.extents.y + heightThreshold);
            hits[6] = Physics.Raycast(controller.bounds.center + new Vector3(controller.bounds.extents.x, 0, -1f * controller.bounds.extents.z), Vector3.up, controller.bounds.extents.y + heightThreshold);
            hits[7] = Physics.Raycast(controller.bounds.center + new Vector3(-1f * controller.bounds.extents.x, 0, controller.bounds.extents.z), Vector3.up, controller.bounds.extents.y + heightThreshold);
            hits[8] = Physics.Raycast(controller.bounds.center + new Vector3(-1f * controller.bounds.extents.x, 0, -1f * controller.bounds.extents.z), Vector3.up, controller.bounds.extents.y + heightThreshold);
        }

        bool anyHits = false;
        for (int i = 0; i < hits.Length; i++) {
            if (hits[i]) {
                anyHits = true;
                break;
            }
        }

        /*//Debugging //Uncomment to see the 9 rays that check if bumped
        Color rayColor;
        if (hits[0]) {
            rayColor = Color.green;
        }
        else {
            rayColor = Color.red;
        }
        Debug.DrawRay(controller.bounds.center, Vector3.up * (controller.bounds.extents.y + heightThreshold), rayColor);

        if (hits[1]) {
            rayColor = Color.green;
        }
        else {
            rayColor = Color.red;
        }
        Debug.DrawRay(controller.bounds.center + new Vector3(controller.bounds.extents.x, 0, 0), Vector3.up * (controller.bounds.extents.y + heightThreshold), rayColor);

        if (hits[2]) {
            rayColor = Color.green;
        }
        else {
            rayColor = Color.red;
        }
        Debug.DrawRay(controller.bounds.center - new Vector3(controller.bounds.extents.x, 0, 0), Vector3.up * (controller.bounds.extents.y + heightThreshold), rayColor);

        if (hits[3]) {
            rayColor = Color.green;
        }
        else {
            rayColor = Color.red;
        }
        Debug.DrawRay(controller.bounds.center + new Vector3(0, 0, controller.bounds.extents.z), Vector3.up * (controller.bounds.extents.y + heightThreshold), rayColor);

        if (hits[4]) {
            rayColor = Color.green;
        }
        else {
            rayColor = Color.red;
        }
        Debug.DrawRay(controller.bounds.center - new Vector3(0, 0, controller.bounds.extents.z), Vector3.up * (controller.bounds.extents.y + heightThreshold), rayColor);

        if (hits[5]) {
            rayColor = Color.green;
        }
        else {
            rayColor = Color.red;
        }
        Debug.DrawRay(controller.bounds.center + new Vector3(controller.bounds.extents.x, 0, controller.bounds.extents.z), Vector3.up * (controller.bounds.extents.y + heightThreshold), rayColor);

        if (hits[6]) {
            rayColor = Color.green;
        }
        else {
            rayColor = Color.red;
        }
        Debug.DrawRay(controller.bounds.center + new Vector3(controller.bounds.extents.x, 0, -1f * controller.bounds.extents.z), Vector3.up * (controller.bounds.extents.y + heightThreshold), rayColor);

        if (hits[7]) {
            rayColor = Color.green;
        }
        else {
            rayColor = Color.red;
        }
        Debug.DrawRay(controller.bounds.center + new Vector3(-1f * controller.bounds.extents.x, 0, controller.bounds.extents.z), Vector3.up * (controller.bounds.extents.y + heightThreshold), rayColor);
        if (hits[8]) {
            rayColor = Color.green;
        }
        else {
            rayColor = Color.red;
        }
        Debug.DrawRay(controller.bounds.center + new Vector3(-1f * controller.bounds.extents.x, 0, -1f * controller.bounds.extents.z), Vector3.up * (controller.bounds.extents.y + heightThreshold), rayColor);
        //Endof Debugging*/

        return anyHits;
    }

    public virtual bool BelowSurface() {
        float heightThreshold = .2f + controller.skinWidth;

        if (Physics.Raycast(controller.bounds.center + controller.bounds.extents, Vector3.up, controller.bounds.extents.y + heightThreshold, LayerMask.GetMask("WaterSurface"))) {
            return true;
        }
        else {
            return false;
        }
    }

    public virtual bool AboveSurface() {
        float heightThreshold = .2f + controller.skinWidth;

        if (Physics.Raycast(controller.bounds.center, Vector3.down, controller.bounds.extents.y + heightThreshold, LayerMask.GetMask("WaterSurface2"))) {
            return true;
        }
        else {
            return false;
        }
    }

    public Vector3 SetMoveRelativeToCamera() {
        Matrix4x4 matrix;
        if (cam != null) {
            matrix = Matrix4x4.Rotate(Quaternion.Euler(0, cam.eulerAngles.y, 0));
        }
        else {
            matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 0));
        }
        Vector3 move = matrix.MultiplyPoint3x4(unchangedMove);
        return move;
    }

    public Vector3 GetMoveInput() {
        if (movementInputEnabled) {
            return new Vector3(inputVector.x, inputVector.y, 0);
        }
        else
            return Vector3.zero;
    }

    public void UpdateRotation() {
        //update rotation if 3d
        //Rotate our 3D object based on input direction
        if (move != Vector3.zero) {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }

    public void InitializeJumpVariables() {
        // for maxheight
        float timeToMaxApex = maxJumpDuration / 2;
        jumpingGravityButtonHeld = (-2f * maxJumpHeight) / Mathf.Pow(timeToMaxApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToMaxApex;

        // fallingGravity
        fallingGravity = jumpingGravityButtonHeld * fallingGravityMultiplier;

        // for minHeight
        // find how high we should be after minJumpHoldTime has elapsed
        // if that height is higher than maxjumpHieght, set jumpingGravityButtonReleased to jumpingGravityButtonHeld
        // otherwise, find out what deceleration is required to get us to peak at minJumpHeight
        float positionAfterMinJumpTime = initialJumpVelocity * minJumpHoldTime + (.5f * jumpingGravityButtonHeld * Mathf.Pow(minJumpHoldTime, 2));
        float velocityAfterMinJumpTime = initialJumpVelocity + (jumpingGravityButtonHeld * minJumpHoldTime);
        jumpingGravityButtonReleased = -Mathf.Pow(velocityAfterMinJumpTime, 2) / (2f * (minJumpHeight - positionAfterMinJumpTime));
        if (jumpingGravityButtonReleased > 0f) {
            jumpingGravityButtonReleased = jumpingGravityButtonHeld;
        }

        // Debug.Log("Initial jump velocity: " + initialJumpVelocity);
        // Debug.Log("Velocity after minimumButtonHeldTime: " + velocityAfterMinJumpTime);
        // Debug.Log("Gravity when button held: " + jumpingGravityButtonHeld);
        // Debug.Log("Gravity when button released: " + jumpingGravityButtonReleased);
    }

    public void UpdateRotation2D() {
        if (isFacingRight) {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }


    // could just use getters and setters for these boolean changing functions but I feel like this is cleaner when we're accessing
    // from a hazard or manager rather than from a state
    public void Die() {
        died = true;
    }

    public void DisableMovementInput() {
        movementInputEnabled = false;
    }

    public void EnableMovementInput() {
        movementInputEnabled = true;
    }
}
