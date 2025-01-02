using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelHighscoreSetter : MonoBehaviour
{
    [SerializeField] private int infoToShow;
    private int myLevel;

    void Start()
    {
        myLevel = FindParentWithComponent<LevelButton>().level; // Unfortunately, this is not a joke.
       
        ShowHighscore();
    }

    void ShowHighscore()
    {
        if (PlayerPrefs.HasKey("HighscoreLevel" + myLevel) && infoToShow == 0) GetComponent<TMP_Text>().text = PlayerPrefs.GetInt("HighscoreLevel" + myLevel).ToString();
        if (PlayerPrefs.HasKey("TotalPointsLevel" + myLevel) && infoToShow == 1) GetComponent<TMP_Text>().text = PlayerPrefs.GetInt("TotalPointsLevel" + myLevel).ToString();
        if (PlayerPrefs.HasKey("TotalGamesLevel" + myLevel) && infoToShow == 2) GetComponent<TMP_Text>().text = PlayerPrefs.GetInt("TotalGamesLevel" + myLevel).ToString();
    }

    T FindParentWithComponent<T>() where T : Component
    {
        Transform current = transform.parent;

        while (current != null)
        {
            T component = current.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
            current = current.parent;
        }

        Debug.LogWarning("No desired component in any parent found!");
        return null;
    }
}
