using UnityEngine;

/// <summary>
/// get from <see href='https://docs.unity3d.com/ScriptReference/CharacterController.Move.html'>docs.unity3d.com</see>
/// </summary>
//тебуемый компонент CharacterController
[RequireComponent(typeof(CharacterController))]
public class MovementCC : MonoBehaviour {
    //[SerializeField] private float Speed = 10f;
    //[SerializeField] private float JumpForce = 300f;
    ////[SerializeField] private string GroundTag = "Ground";

    ////что бы эта переменная работала добавьте тэг "Ground" на вашу поверхность земли
    //private bool isGrounded = true;
    //private Rigidbody rb;

    [SerializeField] private float speed = 10.0f;
    [SerializeField] private float jumpHeight = 3.0f;
    private float gravityValue = -9.81f;
    private Vector3 playerVelocity;

    private bool isGrounded;

    private CharacterController cc;

    private void Start() {
        cc = GetComponent<CharacterController>();
    }

    // физикой необходимо обрабатывать в FixedUpdate, не в Update
    private void FixedUpdate() {

        isGrounded = cc.isGrounded;
        if (isGrounded && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
        }

        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        cc.Move(movement * speed * Time.deltaTime);

        if (movement != Vector3.zero) {
            gameObject.transform.forward = movement;
        }

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && isGrounded) {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        cc.Move(playerVelocity * Time.deltaTime);
    }
}
