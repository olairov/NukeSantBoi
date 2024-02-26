using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{
    private Image insideTickImage;

    [SerializeField] private bool toggleEnabled;

    void Start()
    {
        insideTickImage = transform.Find("Background/Checkmark/Interior").GetComponent<Image>();

        InitializeValues();
    }

    private void InitializeValues()
    {
        if (!PlayerPrefs.HasKey("FullWindow")) PlayerPrefs.SetInt("FullWindow", 1);

        if (PlayerPrefs.GetInt("FullWindow") >= 1)
        {
            toggleEnabled = true;
            transform.GetComponent<Toggle>().isOn = true;
            insideTickImage.enabled = true;
        }
    }

    public void EnableDisable()
    {
        toggleEnabled = !toggleEnabled;
        PlayerPrefs.SetInt("FullWindow", toggleEnabled ? 1 : 0);

        insideTickImage.enabled = toggleEnabled;
    }
}
