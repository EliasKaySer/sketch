using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewSelectObject : MonoBehaviour {

    [SerializeField] private GameObject viewSelectObject;
    private RawImage viewSelectObjectRect;
    [SerializeField] private Texture2D pictureDefault;
    private Texture2D pictureSelectObject;
    public Texture2D PictureSelectObject {
        get { return pictureSelectObject; }
        set { pictureSelectObject = value; }
    }

    private void Start() {
        viewSelectObjectRect = viewSelectObject.GetComponent<RawImage>();
        viewSelectObjectRect.texture = pictureDefault;
    }

    private void OnGUI() {
        if (pictureSelectObject != null) {
            viewSelectObjectRect.texture = pictureSelectObject;
        } else {
            viewSelectObjectRect.texture = pictureDefault;
        }
    }
}
