using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int value = 1;
    protected LevelManager levelManager;
    
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.tag == "Player") {
            PlayerStateMachineBase collidingPlayer = collision.gameObject.GetComponent<PlayerStateMachineBase>();
            int playerIndex = collidingPlayer.PlayerIndex;
            if (levelManager != null) {
                levelManager.CollectJunk(playerIndex, value);
            }

            Destroy(gameObject);
        }
    }

}
