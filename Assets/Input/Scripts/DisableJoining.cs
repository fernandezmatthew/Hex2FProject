using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DisableJoining : MonoBehaviour
{
    PlayerInputManager playerInputManager;
    int maxPlayers;
    void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        maxPlayers = playerInputManager.maxPlayerCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInputManager.playerCount == maxPlayers) {
            playerInputManager.DisableJoining();
            this.enabled = false;
        }
    }
}
