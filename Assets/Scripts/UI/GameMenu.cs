using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour {
    [SerializeField] private GUISkin mainGUISkin;
    [SerializeField, Min(1)] private int numDepth = 1;
    private bool pause = false;

    private Rect fullScreen;
    private Rect menuRect;

    private void Awake() {
        fullScreen = new Rect(0, 0, Screen.width, Screen.height);
        menuRect = new Rect((Screen.width - 150) / 2, (Screen.height - 150) / 2, 150, 150);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ToggleMenu();
        }
    }

    public void ToggleMenu() {
        pause = !pause;
    }

    private void DrawMenu() {
        GUI.depth = numDepth;
        GUI.skin = mainGUISkin;
        GUI.Box(fullScreen, "", mainGUISkin.GetStyle("MenuBackground"));

        GUI.BeginGroup(menuRect);

        GUI.Label(new Rect(25, 30, 100, 30), "Pause", GUI.skin.label);
        if (GUI.Button(new Rect(0, 50, 150, 30), "Resume")) {
            ToggleMenu();
        }
        if (GUI.Button(new Rect(0, 90, 150, 30), "Exit")) {
            Application.Quit();
            Debug.Log("Exit");
        }

        GUI.EndGroup();
    }

    private void OnGUI() {
        if (pause) {
            DrawMenu();
        }
    }
}
