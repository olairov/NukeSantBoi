using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowFPS : MonoBehaviour
{
    TMP_Text myText;
    float deltaTime;
    [SerializeField] float changeSpeed;

    private void Start()
    {
        myText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        //deltaTime += (Time.unscaledDeltaTime - deltaTime) * changeSpeed;
        myText.text = Mathf.Round(1 / Time.unscaledDeltaTime).ToString();
    }
}
