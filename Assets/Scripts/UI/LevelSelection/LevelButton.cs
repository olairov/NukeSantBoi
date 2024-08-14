using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    private static RectTransform lastLevelButtonPressedImage;
    private RectTransform myImage;

    private static GameObject lastSelectedBorder;
    private GameObject selectedBorder;

    [SerializeField] private int level;

    private void Start()
    {
        myImage = GetComponent<RectTransform>();
        selectedBorder = transform.Find("Image/SelectedBorder").gameObject;

        if (!PlayerPrefs.HasKey("Level")) PlayerPrefs.SetInt("Level", 0);

        selectedBorder.SetActive(false);
        if (PlayerPrefs.GetInt("Level") == level)
        {
            SetAsSelected();
        }
    }

    public void Clicked()
    {
        PlayerPrefs.SetInt("Level", level);

        lastLevelButtonPressedImage.localScale = Vector2.one;
        lastSelectedBorder.SetActive(false);

        SetAsSelected();
    }

    void SetAsSelected()
    {
        lastLevelButtonPressedImage = myImage;
        lastLevelButtonPressedImage.localScale = new Vector2(1.1f, 1.1f);

        lastSelectedBorder = selectedBorder;
        selectedBorder.SetActive(true);
    }
}
