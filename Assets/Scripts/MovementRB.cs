using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// get from <see href='https://ru.stackoverflow.com/a/936027'>ru.stackoverflow.com</see>
/// </summary>
//тебуемый компонент Rigidbody
[RequireComponent(typeof(Rigidbody))]
public class MovementRB : MonoBehaviour {
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpForce = 300f;
    //[SerializeField] private string GroundTag = "Ground";

    //что бы эта переменная работала добавьте тэг "Ground" на вашу поверхность земли
    private bool isGrounded = true;
    private Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    // физикой необходимо обрабатывать в FixedUpdate, не в Update
    void FixedUpdate() {
        MovementLogic();
        JumpLogic();
    }

    private void MovementLogic() {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        rb.AddForce(movement * speed);
    }

    private void JumpLogic() {
        if (isGrounded) {
            if (Input.GetAxis("Jump") > 0) {
                //Vector3.up - absolute top, despite on rotation
                rb.AddForce(Vector3.up * jumpForce);
            }
        }
    }

    void OnCollisionEnter(Collision collision) {
        IsGroundedUpate(collision, true);
    }

    void OnCollisionExit(Collision collision) {
        IsGroundedUpate(collision, false);
    }

    private void IsGroundedUpate(Collision collision, bool value) {
        //if (collision.gameObject.tag.Equals(GroundTag)) {
            isGrounded = value;
        //}
    }
}
