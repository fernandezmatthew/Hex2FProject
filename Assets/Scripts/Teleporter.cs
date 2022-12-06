using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform target;

    private void OnTriggerEnter(Collider collision) {
        if (collision.tag == "Player") {
            PlayerStateMachineBase collidingPlayer = collision.gameObject.GetComponent<PlayerStateMachineBase>();
            collidingPlayer.Teleport(target.transform.position);
        }
    }
}
