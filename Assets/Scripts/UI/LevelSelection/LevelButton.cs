using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelButton : MonoBehaviour
{
    private static RectTransform lastLevelButtonPressedImage;
    private RectTransform myImage;

    private static GameObject lastSelectedBorder;
    private GameObject selectedBorder;

    private LevelButtonResizer myLevelButtonResizerScript;
    private AnyButton myAnyButtonScript;

    private float originSize = 1, targetSize = 1, sizeLerpProgress = 1;

    public int level;

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
        transform.Find("Image/NumberSpace/Number").GetComponent<TMP_Text>().text = level.ToString();

        myImage = GetComponent<RectTransform>();
        selectedBorder = transform.Find("Image/SelectedBorder").gameObject;
        myLevelButtonResizerScript = transform.parent.GetComponent<LevelButtonResizer>();
        myAnyButtonScript = GetComponent<AnyButton>();

        if (!PlayerPrefs.HasKey("Level")) PlayerPrefs.SetInt("Level", 1);

        selectedBorder.SetActive(false);
        if (PlayerPrefs.GetInt("Level") == level) // If the saved level from befor is the level of this button, it sets itself as selcted.
        {
            SetAsSelected();
        }
    }

    private void Update()
    {
        if (sizeLerpProgress < 1) SizeLerp();
    }

    void SizeLerp()
    {
        sizeLerpProgress += Time.unscaledDeltaTime * 5;
        sizeLerpProgress = Mathf.Clamp01(sizeLerpProgress);
        myAnyButtonScript.SetDefaultSize = Vector2.one * Mathf.Lerp(originSize, targetSize, sizeLerpProgress);
    }

    public void Clicked()
    {
        if (!clickedDown || !pointed) return;
        clickedDown = false;

        PlayerPrefs.SetInt("Level", level);

        lastLevelButtonPressedImage.GetComponent<LevelButton>().SetScale(1);
        lastSelectedBorder.SetActive(false);

        SetAsSelected();
    }

    void SetAsSelected()
    {
        if (lastLevelButtonPressedImage != null) lastLevelButtonPressedImage.parent.GetComponent<LevelButtonResizer>().LevelButtonIsSelected = false;
        lastLevelButtonPressedImage = myImage;

        if (!myLevelButtonResizerScript.InfoPressed) SetScale(1.1f);
        myLevelButtonResizerScript.LevelButtonIsSelected = true;

        lastSelectedBorder = selectedBorder;
        selectedBorder.SetActive(true);
    }

    public void SetScale(float scale)
    {
        originSize = targetSize; // The original size is the last TargetSize that was assigned.
        targetSize = scale;
        sizeLerpProgress = 0;
    }
}
