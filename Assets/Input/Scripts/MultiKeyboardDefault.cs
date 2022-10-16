using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiKeyboardDefault : MonoBehaviour {
    PlayerInputManager playerInputManager;
    void Awake() {
        playerInputManager = GetComponent<PlayerInputManager>();
    }

    // Update is called once per frame
    void Start() {
        var p1 = PlayerInput.Instantiate(playerInputManager.playerPrefab, controlScheme: "Keyboard0", pairWithDevice: Keyboard.current);
        var p2 = PlayerInput.Instantiate(playerInputManager.playerPrefab, controlScheme: "Keyboard1", pairWithDevice: Keyboard.current);
    }
}
