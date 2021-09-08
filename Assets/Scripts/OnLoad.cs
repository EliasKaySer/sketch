using System.Collections.Generic;
using UnityEngine;

public class OnLoad : MonoBehaviour {
    [SerializeField] private GameObject[] gameObjects;
    private void Awake() {
        foreach (var gameObject in gameObjects) {
            gameObject.gameObject.SetActive(true);
        }
    }
}
