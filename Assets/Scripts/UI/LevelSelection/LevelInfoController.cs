using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfoController : MonoBehaviour
{
    Transform infoPlaceTransform, infoSelectedTransform;

    Animator myInfoAnimator, infoSelectedAnimator;

    AudioSource clickedSound;

    AnyButton myButtonScript;
    LevelButtonResizer myLevelResizerScript;
    ScrollerController scrollerScript;

    public bool infoClikedDown;
    bool infoEnabled;

    void Start()
    {
        infoPlaceTransform = transform.parent.Find("LevelButton/Image/infoPlace").transform;
        infoSelectedTransform = GameObject.Find("CanvasLevelSelection/MainScreen/infoGlow").transform;

        myInfoAnimator = transform.parent.Find("LevelButton/Image/InfoFill").GetComponent<Animator>();
        infoSelectedAnimator = infoSelectedTransform.GetComponent<Animator>();

        clickedSound = GameObject.Find("UIsounds/SliderSound").GetComponent<AudioSource>();
        myButtonScript = transform.parent.Find("LevelButton").GetComponent<AnyButton>();
        myLevelResizerScript = transform.parent.GetComponent<LevelButtonResizer>();
        scrollerScript = GameObject.Find("CanvasLevelSelection/MainScreen/Scroller").GetComponent<ScrollerController>();
    }

    private void Update()
    {
        transform.position = infoPlaceTransform.position;
        if (LevelButtonResizer.exittingScene)
        {
            myInfoAnimator.SetBool("enabled", false);
            myInfoAnimator.speed = 2; // Do the animation faster so that the text doesn't look squeezed becouse of the instant button's shrinking.
        }
    }

    public void InfoPressed()
    {
        infoEnabled = !infoEnabled;

        myInfoAnimator.SetBool("enabled", infoEnabled);
        myLevelResizerScript.InfoPressed = infoEnabled;
        // When in phone, AnyButton isn't allowed to grow because there's no pointer. But since here it's part of the animation, I allow it to grow:
        myButtonScript.AbleToGrowWhenInPhoneDevice = infoEnabled;

        infoSelectedTransform.position = new Vector3(transform.position.x, transform.position.y, infoSelectedTransform.position.z);
        infoSelectedAnimator.SetTrigger("selected");

        clickedSound.Play();
    }

    public void InfoSelected()
    {
        if (!TouchControllersManager.isUsingPhone)
        {
            infoPlaceTransform.localScale = Vector2.one * 1.2f;
        }

        if (!(myLevelResizerScript.IsGrown && !infoEnabled) && !TouchControllersManager.isUsingPhone) myButtonScript.SetPointed = true;
    }

    public void InfoDisselected()
    {
        infoPlaceTransform.localScale = Vector2.one;
        myButtonScript.StopSelectedSound();

        myButtonScript.SetPointed = false;
        infoClikedDown = false;
    }

    // Just for detect scrolling

    public void ClickedDown()
    {
        scrollerScript.PointerDown();
        infoClikedDown = true;
    }

    public void ClickedUp()
    {
        scrollerScript.PointerUp();
        if (infoClikedDown)
        {
            InfoPressed();
            infoClikedDown = false;
        }
    }
}
