using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

////тебуемый компонент Rigidbody
//[RequireComponent(typeof(Rigidbody))]
//тебуемый компонент Rigidbody
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
    //    // Если вы хотите проверять, с кем вы столкнулись,
    //    // то, как правило, должны использовать теги, а не имена
    //    if (collision.gameObject.tag.Equals(checkingTag)) {
    //        // Отыгрываем столкновение
    //        gameObject.SetActive(false);
    //    }
    //}
    void OnTriggerEnter(Collider collider) {
        // Если игрок попадает на триггер
        if (collider.gameObject.tag.Equals(checkingTag)) {
            // О контроллере игры расскажу чуть позже
            //GameController.Score++;
            scoreText.text = (Convert.ToInt32(scoreText.text) + 10).ToString();
            // Не вызывайте Destroy(this); поскольку this - это
            // компонент скрипта в игровом объекте, вы используете
            // this.gameObject или просто gameObject, чтобы
            // уничтожить объект
            Destroy(gameObject);
        }
    }
}
