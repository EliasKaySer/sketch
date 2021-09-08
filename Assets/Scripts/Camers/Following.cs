using UnityEngine;
using System.Collections;

public class Following : MonoBehaviour {

    [SerializeField] private GameObject player; // ������ ������
    [SerializeField] private GameObject follower; // ������ ������
    private Vector3 offset;

    void Start() {
        offset = follower.transform.position - player.transform.position;
    }

    void LateUpdate() {
        follower.transform.position = player.transform.position + offset;
    }
}
