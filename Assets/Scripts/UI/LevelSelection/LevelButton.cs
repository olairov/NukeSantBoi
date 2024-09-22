using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    private static RectTransform lastLevelButtonPressedImage;
    private RectTransform myImage;

    private static GameObject lastSelectedBorder;
    private GameObject selectedBorder;

    private LevelButtonResizer myLevelButtonResizerScript;

    [SerializeField] private int level;

    private bool clickedDown, pointed;
    public bool ClickedDown
    {
        set { clickedDown = value; }
    }
    public bool IsPointed
    {
        set { pointed = value; }
    }

    private void Start()
    {
        myImage = GetComponent<RectTransform>();
        selectedBorder = transform.Find("Image/SelectedBorder").gameObject;
        myLevelButtonResizerScript = transform.parent.GetComponent<LevelButtonResizer>();

        if (!PlayerPrefs.HasKey("Level")) PlayerPrefs.SetInt("Level", 1);

        selectedBorder.SetActive(false);
        if (PlayerPrefs.GetInt("Level") == level)
        {
            SetAsSelected();
        }
    }

    public void Clicked()
    {
        if (!clickedDown || !pointed) return;
        clickedDown = false;

        PlayerPrefs.SetInt("Level", level);

        lastLevelButtonPressedImage.localScale = Vector2.one;
        lastSelectedBorder.SetActive(false);

        SetAsSelected();
    }

    void SetAsSelected()
    {
        lastLevelButtonPressedImage = myImage;
        if (!myLevelButtonResizerScript.InfoPressed) SetScale(1.1f);
        else myLevelButtonResizerScript.LevelButtonIsSelected = true;

        lastSelectedBorder = selectedBorder;
        selectedBorder.SetActive(true);
    }

    public void SetScale(float scale)
    {
        myImage.localScale = new Vector2(scale, scale);
    }
}
