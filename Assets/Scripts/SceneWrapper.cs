using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneWrapper : MonoBehaviour
{
    private Scene activeScene;

    void Awake()
    {
        activeScene = SceneManager.GetActiveScene();
    }

    void Update()
    {
        ReloadScene();
    }

    private void ReloadScene() {
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(activeScene.buildIndex);
        }
    }
}
