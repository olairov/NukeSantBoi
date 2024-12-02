using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonResizer : MonoBehaviour
{
    private RectTransform myRectTransform;
    private AnyButton myButtonScript;
    private LevelButton myLevelButtonScript;
    private LevelInfoController myLevelInfoScript;
    private Canvas myImageCanvas;
    private Image myButtonClickerImage, myInfoButtonImage;

    private static Image actualClickedButtonImage;

    [SerializeField] private float growingSpeed;
    private float growingLerp, xOriginalPos, originalXSize;
    public bool IsGrown
    {
        get { return growingLerp > 0; }
    }

    public static bool anyInfoPressedAlready, anyInfoPressed, exittingScene;
    private bool infoPressed, levelButtonIsSelected;
    public bool InfoPressed
    {
        set
        {
            infoPressed = value;
            anyInfoPressed = value;
            anyInfoPressedAlready = true;

            if (infoPressed)
            {
                actualClickedButtonImage = myButtonClickerImage;
                //levelButtonIsSelected = myLevelButtonScript.transform.localScale.x > 1;
                if (levelButtonIsSelected) myLevelButtonScript.SetScale(1f);
            }
            else
            {
                growingLerp = 1.5f; // Give time to the animation to quit the informative text, which looks bad when the button is shrinking.
                actualClickedButtonImage = null;
                myButtonScript.Unpointed();
                if (levelButtonIsSelected) myLevelButtonScript.SetScale(1.1f);
            }
        }
        get { return infoPressed; }
    }
    public bool LevelButtonIsSelected
    {
        set { levelButtonIsSelected = value; }
    }

    void Start()
    {
        myRectTransform = GetComponent<RectTransform>();
        myButtonScript = transform.GetChild(0).GetComponent<AnyButton>();
        myLevelButtonScript = transform.GetChild(0).GetComponent<LevelButton>();
        myLevelInfoScript = transform.Find("InfoClicker").GetComponent<LevelInfoController>();
        myImageCanvas = transform.GetChild(0).GetChild(0).GetComponent<Canvas>();
        myButtonClickerImage = transform.GetChild(0).Find("Clicker").GetComponent<Image>();
        myInfoButtonImage = transform.Find("InfoClicker").GetComponent<Image>();

        originalXSize = myRectTransform.sizeDelta.x;
        xOriginalPos = myRectTransform.position.x;

        actualClickedButtonImage = null;
        anyInfoPressedAlready = false;
        anyInfoPressed = false;
        exittingScene = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause") && infoPressed)
        {
            myLevelInfoScript.InfoPressed();
        }

        // Making the button grow or shrink when the info button pressed.
        if (infoPressed && growingLerp < 1 && !exittingScene)
        {
            growingLerp += Time.deltaTime * growingSpeed;
            ChangeSize(Mathf.Clamp01(growingLerp));
        }
        if (!infoPressed && growingLerp > 0 || exittingScene && infoPressed)
        {
            growingLerp -= Time.deltaTime * growingSpeed;
            ChangeSize(Mathf.Clamp01(growingLerp));
        }

        if (infoPressed) // Make sure that while info is being shown, it stays in big.
        {
            myButtonScript.Pointed();
        }

        if (actualClickedButtonImage != myButtonClickerImage && anyInfoPressed) // Make sure that while info is being shown, the OTHER buttons don't work.
        {
            myButtonClickerImage.enabled = false;
            myInfoButtonImage.enabled = false;
        }
        else // When there's no button showing info or if it is this one, the buttons are active.
        {
            myButtonClickerImage.enabled = true;
            myInfoButtonImage.enabled = true;
        }

        // When the button is pressed, it's shown over the other ones.
        if (anyInfoPressedAlready)
        {
            if (growingLerp > 0) myImageCanvas.sortingOrder = 6;
            else myImageCanvas.sortingOrder = 5;
        }
        else myImageCanvas.sortingOrder = 3; // In the beginning, the buttons have to pass behind the border, so their layer has to be normal.
    }

    void ChangeSize(float lerp)
    {
        myRectTransform.sizeDelta = new Vector2(Mathf.Lerp(originalXSize, 850, lerp), myRectTransform.sizeDelta.y);
        myRectTransform.position = new Vector2(Mathf.Lerp(xOriginalPos, 0 / 2, lerp), 0);
    }
}
