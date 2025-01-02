using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{
    private Image insideTickImage;

    private bool toggleEnabled, toggledInitially;

    [SerializeField] private int myAction = 0;

    void Start()
    {
        insideTickImage = transform.Find("Background/Image/Checkmark/Interior").GetComponent<Image>();

        if (myAction < 1) InitializeValuesFullWindow();
        else InitializeValuesThreeDAudio();
    }

    private void InitializeValuesFullWindow()
    {
        if (!PlayerPrefs.HasKey("FullWindow")) PlayerPrefs.SetInt("FullWindow", 1);
        insideTickImage.enabled = false;

        if (PlayerPrefs.GetInt("FullWindow") >= 1)
        {
            toggleEnabled = true;
            toggledInitially = true;
            insideTickImage.enabled = true;
            GetComponent<Toggle>().isOn = true;
        }
    }

    private void InitializeValuesThreeDAudio()
    {
        if (!PlayerPrefs.HasKey("ThreeDAudio")) PlayerPrefs.SetInt("ThreeDAudio", 1);
        insideTickImage.enabled = false;

        if (PlayerPrefs.GetInt("ThreeDAudio") >= 1)
        {
            toggleEnabled = true;
            toggledInitially = true;
            insideTickImage.enabled = true;
            GetComponent<Toggle>().isOn = true;
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
        insideTickImage.enabled = toggleEnabled;

        if (myAction < 1)
        {
            PlayerPrefs.SetInt("FullWindow", toggleEnabled ? 1 : 0);
            Screen.fullScreen = toggleEnabled;
        }
        else
        {
            PlayerPrefs.SetInt("ThreeDAudio", toggleEnabled ? 1 : 0);
            InGameSound.threeDsound = toggleEnabled;
        }

    }
}
