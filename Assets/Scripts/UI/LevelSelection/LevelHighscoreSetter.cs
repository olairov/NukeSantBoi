using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelHighscoreSetter : MonoBehaviour
{
    [SerializeField] private int myLevel, infoToShow;

    void Start()
    {
        ShowHighscore();
    }

    void ShowHighscore()
    {
        if (PlayerPrefs.HasKey("HighscoreLevel" + myLevel) && infoToShow == 0) GetComponent<TMP_Text>().text = PlayerPrefs.GetInt("HighscoreLevel" + myLevel).ToString();
        if (PlayerPrefs.HasKey("TotalPointsLevel" + myLevel) && infoToShow == 1) GetComponent<TMP_Text>().text = PlayerPrefs.GetInt("TotalPointsLevel" + myLevel).ToString();
        if (PlayerPrefs.HasKey("TotalGamesLevel" + myLevel) && infoToShow == 2) GetComponent<TMP_Text>().text = PlayerPrefs.GetInt("TotalGamesLevel" + myLevel).ToString();
    }
}
