using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizMenuAppear : MonoBehaviour
{
    private void Start()
    {
        transform.Find("surveyMenu").gameObject.SetActive(false);
    }

    public void QuizMenuStartAnim()
    {
        transform.Find("surveyMenu").gameObject.SetActive(true);
        GetComponent<Animator>().SetBool("MenuIn", true);
    }

    public void Collision()
    {
        GameObject.Find("Camera/CameraRiser/Main Camera").GetComponent<ShakeController>().Shake();
        GetComponent<AudioSource>().Play();
    }
}
