using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerStateMachineBase : MonoBehaviour {
    //Setup Default State within the Lowest Level of the Context

    //References
    [SerializeField] protected LayerMask groundedLayers;
    [SerializeField] protected LayerMask enemyLayers;
    [SerializeField] protected Transform cam;
    protected CharacterController controller;
    protected Animator anim;

    //Visible Variables (Movement)
    [SerializeField] protected float basePlayerSpeed = 12f;
    [SerializeField] protected float gravityScale = 8f; //Strength of gravity on this object
    [SerializeField] protected bool holdJump = false;
    [SerializeField] protected float jumpHeight = 8f;
    [SerializeField] protected float airSpeedRatio = .75f; //Decreases player speed in the air
    [SerializeField] protected float crouchSpeedRatio = .75f; //Decreases player speed while crouched
    [SerializeField] protected int extraJumps = 0;
    [SerializeField] protected float maxJumpHoldTime = 0.2f;
    [SerializeField] protected float jumpBufferedCounterMax = .05f;
    [SerializeField] protected float terminalVelocity = 10000f; //Caps our fall speed. //Eventually make this not visible from editor
    //Visible Variables (Hero)
    /*[SerializeField] protected int maxHealth = 100;
    [SerializeField] protected int baseDamageOutput = 20;
    [SerializeField] protected float attackRange = 1f;
    [SerializeField] protected float attackRate = 3f; //Stops player from being able to attack infintely fast, in Hz
    [SerializeField] protected float abilityARate = 3f;
    [SerializeField] protected float abilityDRate = 3f;*/

    //Private Variables (Movement)
    //A lot of these booleans can potentially be done away with because of the state machine, will comment them out without deleting for now
    protected Vector3 playerVelocity; //vector used for vertical physics
    protected Vector3 unchangedMove;
    protected Vector3 move;
    protected bool movementInputEnabled;
    protected bool abilityInputEnabled;
    //protected bool isGrounded;
    //protected bool isMoving;
    protected bool facingRight;
    //protected bool sprintPressed; //Make sure to implement once inputs are setup
    protected float baseGravityValue; //Personal gravitational acceleration
    protected float currentGravityValue;
    protected float currentPlayerSpeed;
    protected float turnSmoothTime; //Smooths the object's turning
    protected float turnSmoothVelocity; //Also smooths the object's turning
    protected float originalStepOffset; //Helps us not jitter when jumping onto a ledge;
    protected float jumpRate; //Stops player from being able to accidentally double jump when spamming jump key. Measured in Hz.
    protected float nextJumpTime; //Works with previous variable
    protected int extraJumpsLeft; //Keeps track of how many more jumps we can use before touching the ground again
    protected float jumpHoldTimeCounter;
    protected float jumpBufferedCounter;
    protected float isGroundedCounter;
    protected float isGroundedCounterMax;
    protected float runThreshold;
    //protected float frozenZ; //Freezes Z position
    //private Variables (Hero)
    /*protected int currentHealth;
    protected int currentDamageOutput;
    protected float nextAttackTime; //Works with attackRate
    protected float nextAbilityATime;
    protected float nextAbilityDTime;
    protected float attackAnimationTime; //time it takes to do attack animation
    protected float attackAnimationTimeLeft; //time left in current attack animation
    protected float caughtAnimationTime; //dont change the speed of this animation or wont work right unfortunately
    protected bool attackedLastFrame;
    protected Dictionary<string, Collider> enemiesHitOnCurrentAttack; //stores all enemies hit on currentattack so we dont register twice
    protected bool inAbilityA;
    protected bool inAbilityD;*/

    //State Variables
    protected PlayerBaseState currentPlayerState;
    protected PlayerStateFactory playerStates;
    protected EPlayerState ePlayerState;

    //Getters and Setters to our private variables
    public PlayerBaseState CurrentPlayerState { get { return currentPlayerState; } set { currentPlayerState = value; } }
    public EPlayerState EPlayerState { get { return ePlayerState; } set { ePlayerState = value; } }
    public float JumpBufferedCounter { get { return jumpBufferedCounter; } set { jumpBufferedCounter = value; } }
    public float JumpBufferedCounterMax { get { return jumpBufferedCounterMax; } set { jumpBufferedCounterMax = value; } }
    public Vector3 Move { get { return move; } set { move = value; } }
    public Vector3 UnchangedMove { get { return unchangedMove; } set { unchangedMove = value; } }
    public CharacterController Controller { get { return controller; } set { controller = value; } }
    public float CurrentPlayerSpeed { get { return currentPlayerSpeed; } set { currentPlayerSpeed = value; } }
    public float AirSpeedRatio { get { return airSpeedRatio; } set { airSpeedRatio = value; } }
    public Vector3 PlayerVelocity { get { return playerVelocity; } set { playerVelocity = value; } }
    public int ExtraJumpsLeft { get { return extraJumpsLeft; } set { extraJumpsLeft = value; } }
    public int ExtraJumps { get { return extraJumps; } set { extraJumps = value; } }
    public float OriginalStepOffset { get { return originalStepOffset; } set { originalStepOffset = value; } }
    public bool MovementInputEnabled { get { return movementInputEnabled; } set { movementInputEnabled = value; } }
    public bool AbilityInputEnabled { get { return abilityInputEnabled; } set { abilityInputEnabled = value; } }
    public float NextJumpTime { get { return nextJumpTime; } set { nextJumpTime = value; } }
    public float JumpHeight { get { return jumpHeight; } set { jumpHeight = value; } }
    public float BaseGravityValue { get { return baseGravityValue; } set { baseGravityValue = value; } }
    public float CurrentGravityValue { get { return currentGravityValue; } set { currentGravityValue = value; } }
    public float JumpRate { get { return jumpRate; } set { jumpRate = value; } }
    public float JumpHoldTimeCounter { get { return jumpHoldTimeCounter; } set { jumpHoldTimeCounter = value; } }
    public float MaxJumpHoldTime { get { return maxJumpHoldTime; } set { maxJumpHoldTime = value; } }
    public bool HoldJump { get { return holdJump; } set { holdJump = value; } }
    public float RunThreshold { get { return runThreshold; } set { runThreshold = value; } }

    protected virtual void Awake() {
        //Grab References
        controller = gameObject.GetComponent<CharacterController>(); //would like to disable the capsule connected to this
        anim = GetComponent<Animator>();

        //Set variables (Movement)
        baseGravityValue = gravityScale * -9.81f; //Sets personal gravity based on gravity scale
        currentGravityValue = baseGravityValue;
        ePlayerState = EPlayerState.Default;
        movementInputEnabled = true;
        abilityInputEnabled = true;
        //isMoving = false;
        //isGrounded = false;
        //crouchPressed = false;
        //sprintPressed = false;
        //isCrouched = false;
        playerVelocity = Vector3.zero;
        move = Vector3.zero;
        currentPlayerSpeed = basePlayerSpeed;
        turnSmoothTime = 0.1f;
        jumpRate = 8f;
        nextJumpTime = 0f;
        extraJumpsLeft = extraJumps;
        originalStepOffset = controller.stepOffset;
        //isJumping = false;
        jumpHoldTimeCounter = maxJumpHoldTime;
        jumpBufferedCounter = 0f;
        isGroundedCounter = 0f;
        isGroundedCounterMax = .05f;
        runThreshold = .85f;
        //isInAbilityA = false;
        //isInAbilityD = false;

        //Set variables (Hero)
        /*currentHealth = maxHealth;
        currentDamageOutput = baseDamageOutput;
        nextAttackTime = 0f;
        nextAbilityATime = 0f;
        nextAbilityDTime = 0f;
        attackedLastFrame = false;
        enemiesHitOnCurrentAttack = new Dictionary<string, Collider>();
        attackAnimationTimeLeft = 0f;
        inAbilityA = false;
        inAbilityD = false;*/

        //will eventually have to copy-paste the animationClip[] thing i have, but 0 for now
        /*attackAnimationTime = 0;
        caughtAnimationTime = 0;*/
    }

    public virtual bool IsGrounded() {
        float heightThreshold = 1.6f + controller.skinWidth;
        if (playerVelocity.y > 0) {
            return false;
        }

        //Cast 9 rays to check groundedness. isGrounded if any are true
        bool[] hits = new bool[9];
        bool hit = false;

        if (groundedLayers != LayerMask.GetMask("Nothing")) {
            hit =
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

    public void SetParent() {

    }

    public void UpdateGravity() {
        //Implementing a terminal velocity cuz i think itll feel better
        if (playerVelocity.y > -1f * terminalVelocity) {
            playerVelocity.y += currentGravityValue * Time.deltaTime;
        }
        controller.Move(playerVelocity * Time.deltaTime);
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
        return new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
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
}
