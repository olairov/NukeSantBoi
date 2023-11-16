using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowHighscore : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.HasKey("Highscore")) GetComponent<TMP_Text>().text = PlayerPrefs.GetInt("Highscore").ToString();
    }
}
