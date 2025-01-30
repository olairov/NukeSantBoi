using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{
    private Image insideTickImage;

    private bool toggleEnabled, toggledInitially;

    [SerializeField] private int myAction = 0; // 0 = FullWindow,  1 = ThreeDAudio

    void Start()
    {
        insideTickImage = transform.Find("Background/Image/Checkmark/Interior").GetComponent<Image>();

        string function = "";

        switch (myAction)
        {
            case 0:
                function = "FullWindow";
                break;
            case 1:
                function = "ThreeDAudio";
                break;
            default:
                Debug.LogError("'myAction' value is not valid");
                break;
        }

        if (function.Length > 0) InitializeValues(function);
    }

    private void InitializeValues(string toggleFunction)
    {
        if (!PlayerPrefs.HasKey(toggleFunction)) PlayerPrefs.SetInt(toggleFunction, 1);
        insideTickImage.enabled = false;

        if (PlayerPrefs.GetInt(toggleFunction) >= 1)
        {
            toggleEnabled = true;
            toggledInitially = true;
            insideTickImage.enabled = true;
            GetComponent<Toggle>().isOn = true;
        }
        else
        {
            toggleEnabled = false;
            toggledInitially = false;
            insideTickImage.enabled = false;
            GetComponent<Toggle>().isOn = false;
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
