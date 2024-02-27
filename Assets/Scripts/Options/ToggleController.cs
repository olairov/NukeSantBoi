using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{
    private Image insideTickImage;

    [SerializeField] private bool toggleEnabled;
    private bool toggledInitially;

    void Start()
    {
        insideTickImage = transform.Find("Background/Checkmark/Interior").GetComponent<Image>();

        InitializeValues();
    }

    private void InitializeValues()
    {
        if (!PlayerPrefs.HasKey("FullWindow")) PlayerPrefs.SetInt("FullWindow", 1);
        insideTickImage.enabled = false;

        if (PlayerPrefs.GetInt("FullWindow") >= 1)
        {
            toggleEnabled = true;
            toggledInitially = true;
            transform.GetComponent<Toggle>().isOn = true;
            insideTickImage.enabled = true;
        }
    }

    public void EnableDisable()
    {
        if (toggledInitially)
        {
            toggledInitially = false;
            return;
        }

        toggleEnabled = !toggleEnabled;
        Screen.fullScreen = toggleEnabled;

        PlayerPrefs.SetInt("FullWindow", toggleEnabled ? 1 : 0);
        insideTickImage.enabled = toggleEnabled;
    }
}
