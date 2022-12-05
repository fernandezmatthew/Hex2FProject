using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour {
    Scene scene;

    public void Start() {
        
    }
    public void LoadByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void RestartLevel() {
        scene = SceneManager.GetActiveScene();
        LoadByIndex(scene.buildIndex);
    }
}
