using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputReader : MonoBehaviour
{
    protected PlayerControls playerControls;
    protected PlayerInput playerInput;
    protected PlayerStateMachineBase movement;

    void Awake() {

        playerInput = GetComponent<PlayerInput>();

        // Find all movement scripts in scene
        PlayerStateMachineBase[] movements = FindObjectsOfType<PlayerStateMachineBase>();
        int index = playerInput.playerIndex;
        movement = movements.FirstOrDefault(movements => movements.GetPlayerIndex() == index);
        Debug.Log("Player Index " + index + " instantiated!");

        // Set our callbacks // try this if I cant figure out jump with new input system
        playerInput.actions["Jump"].started += OnJump;
        playerInput.actions["Jump"].canceled += OnJumpReleased;
    }

    public void OnMove(CallbackContext ctx) {
        if (movement != null) {
            movement.InputVector = ctx.ReadValue<Vector2>();
        }
    }

    public void OnJump(CallbackContext ctx) {
        if (movement != null) {
            movement.InputJumpButtonPressed = true;
            movement.InputJumpButtonHeld = true;
        }
    }

    public void OnJumpReleased(CallbackContext ctx) {
        movement.InputJumpButtonHeld = false;
    }
}
