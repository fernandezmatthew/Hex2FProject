using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameLimiter : MonoBehaviour
{
    
    [SerializeField] public bool limit = true;
    [SerializeField] public int FPS = 144;
    
    void Awake() {
        if (limit) {
            Application.targetFrameRate = FPS;
        }
        else {
            Application.targetFrameRate = -1;
        }
    }
}
