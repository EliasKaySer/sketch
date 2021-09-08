using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCamera : MonoBehaviour {
    [SerializeField] private int speed = 100;
    [SerializeField] private int borderSize = 15;
    [SerializeField] private float speedScalce = 0.1f;
    private float defaultHeight = 0f;
    private int defaultSpeed = 0;
    [SerializeField]
    CameraMoveRestriction restriction
        = new CameraMoveRestriction() {
            Use = false,
            Left = 50,
            Top = 50,
            Right = 0,
            Bottom = 0,
        };

    void Update() {
        if (restriction.Use) {
            if (restriction.Left <= transform.position.x && (Input.mousePosition.x > 0 && Input.mousePosition.x < borderSize)) {
                transform.position -= transform.right * Time.deltaTime * speed;
            }
            if (restriction.Right >= transform.position.x && (Input.mousePosition.x > Screen.width - borderSize && Input.mousePosition.x < Screen.width)) {
                transform.position += transform.right * Time.deltaTime * speed;
            }
            if (restriction.Bottom <= transform.position.z && (Input.mousePosition.y > 0 && Input.mousePosition.y < borderSize)) {
                transform.position -= transform.forward * Time.deltaTime * speed;
            }
            if (restriction.Top >= transform.position.z && (Input.mousePosition.y > Screen.height - borderSize && Input.mousePosition.y <= Screen.height)) {
                transform.position += transform.forward * Time.deltaTime * speed;
            }
        } else {
            if ((Input.mousePosition.x > 0 && Input.mousePosition.x < borderSize)) {
                transform.position -= transform.right * Time.deltaTime * speed;
            }
            if ((Input.mousePosition.x > Screen.width - borderSize && Input.mousePosition.x < Screen.width)) {
                transform.position += transform.right * Time.deltaTime * speed;
            }
            if ((Input.mousePosition.y > 0 && Input.mousePosition.y < borderSize)) {
                transform.position -= transform.forward * Time.deltaTime * speed;
            }
            if ((Input.mousePosition.y > Screen.height - borderSize && Input.mousePosition.y <= Screen.height)) {
                transform.position += transform.forward * Time.deltaTime * speed;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0f) {
            transform.position -= new Vector3(0f, transform.position.y * Input.GetAxis("Mouse ScrollWheel"), 0f);
            ChangeSpeed();
        }
    }
    public void Start() {
        defaultHeight = transform.position.y;
        defaultSpeed = speed;
    }
    private void ChangeSpeed() {
        //speed = (int)(defaultHeight > transform.position.y ? defaultSpeed - Mathf.Abs(speedScalce * transform.position.y)
        //    : defaultHeight < transform.position.y ? defaultSpeed + Mathf.Abs(speedScalce * transform.position.y)
        //    : defaultSpeed);
        speed = defaultSpeed
            + (int)(defaultHeight > transform.position.y ? -1 * Mathf.Abs(speedScalce * transform.position.y)
            : defaultHeight < transform.position.y ? Mathf.Abs(speedScalce * transform.position.y)
            : defaultSpeed);
        speed = speed < 25 ? 25
            : speed > 225 ? 225
            : speed;
    }


    [Serializable]
    public struct CameraMoveRestriction {

        [SerializeField] bool use;
        public bool Use { get { return use; } set { use = value; } }

        [SerializeField] float left;
        public float Left { get { return left; } set { left = value; } }

        [SerializeField] float top;
        public float Top { get { return top; } set { top = value; } }

        [SerializeField] float right;
        public float Right { get { return right; } set { right = value; } }

        [SerializeField] float bottom;
        public float Bottom { get { return bottom; } set { bottom = value; } }
    }
}
