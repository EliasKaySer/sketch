using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

////�������� ��������� Rigidbody
//[RequireComponent(typeof(Rigidbody))]
//�������� ��������� Rigidbody
[RequireComponent(typeof(Collider))]
public class Die : MonoBehaviour {
    [SerializeField] private string checkingTag = "Player";
    [SerializeField] private GameObject scoreObject;
    private Text scoreText;

    private void Start() {
        scoreText = scoreObject.GetComponent<Text>();
    }

    //void OnCollisionEnter(Collision collision) {
    //    Debug.Log(collision.gameObject.name);
    //    // ���� �� ������ ���������, � ��� �� �����������,
    //    // ��, ��� �������, ������ ������������ ����, � �� �����
    //    if (collision.gameObject.tag.Equals(checkingTag)) {
    //        // ���������� ������������
    //        gameObject.SetActive(false);
    //    }
    //}
    void OnTriggerEnter(Collider collider) {
        // ���� ����� �������� �� �������
        if (collider.gameObject.tag.Equals(checkingTag)) {
            // � ����������� ���� �������� ���� �����
            //GameController.Score++;
            scoreText.text = (Convert.ToInt32(scoreText.text) + 10).ToString();
            // �� ��������� Destroy(this); ��������� this - ���
            // ��������� ������� � ������� �������, �� �����������
            // this.gameObject ��� ������ gameObject, �����
            // ���������� ������
            Destroy(gameObject);
        }
    }
}
