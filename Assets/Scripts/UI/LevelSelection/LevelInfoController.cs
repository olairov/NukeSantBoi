using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfoController : MonoBehaviour
{
    Transform infoPlaceTransform;

    Animator myInfoAnimator;

    AudioSource clickedSound;

    AnyButton myButtonScript;
    LevelButtonResizer myLevelResizerScript;

    bool infoEnabled;

    void Start()
    {
        myInfoAnimator = transform.parent.GetChild(0).Find("Image/InfoFill").GetComponent<Animator>();
        infoPlaceTransform = transform.parent.GetChild(0).Find("Image/infoPlace").transform;
        clickedSound = GameObject.Find("UIsounds/SliderSound").GetComponent<AudioSource>();
        myButtonScript = transform.parent.GetChild(0).GetComponent<AnyButton>();
        myLevelResizerScript = transform.parent.GetComponent<LevelButtonResizer>();
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
        myButtonScript.AbleToGrowWhenInPhoneDevice = infoEnabled;

        clickedSound.Play();
    }

    public void InfoSelected()
    {
        if (!TouchControllersManager.isUsingPhone)
        {
            infoPlaceTransform.localScale = Vector2.one * 1.2f;
        }
        if (!(myLevelResizerScript.IsShrinking && !infoEnabled)) myButtonScript.SetPointed = true;
    }

    public void InfoDisselected()
    {
        infoPlaceTransform.localScale = Vector2.one;
        myButtonScript.StopSelectedSound();

        myButtonScript.SetPointed = false;
    }
}
