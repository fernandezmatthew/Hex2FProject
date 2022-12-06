// Code gotten from: https://forum.unity.com/threads/how-to-stop-music-after-scene-change.1185973/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalBgm : MonoBehaviour {
    private void Awake() {
        GameObject[] musicObj = GameObject.FindGameObjectsWithTag("GameMusic");
        if (musicObj.Length > 1) {
            Destroy(this.gameObject);
        }
        else {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
