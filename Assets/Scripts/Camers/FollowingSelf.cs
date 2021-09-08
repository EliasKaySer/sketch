using UnityEngine;


public class FollowingSelf : MonoBehaviour {

    [SerializeField] private GameObject player; // Объект игрока
    private Vector3 offset;

    void Start() {
        offset = transform.position - player.transform.position;
    }

    void LateUpdate() {
        transform.position = new Vector3(player.transform.position.x + offset.x, transform.position.y, player.transform.position.z + offset.z);
        if (Input.GetAxis("Mouse ScrollWheel") != 0f) {
            transform.position -= new Vector3(transform.position.x, transform.position.y * Input.GetAxis("Mouse ScrollWheel"), transform.position.z);
        }
    }
}
