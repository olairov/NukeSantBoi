using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfoController : MonoBehaviour
{
    Transform infoPlaceTransform;

    Animator myInfoAnimator;

    AudioSource clickedSound;

    AnyButton myButtonScript;

    bool infoEnabled;

    void Start()
    {
        myInfoAnimator = transform.parent.GetChild(0).Find("Image/InfoFill").GetComponent<Animator>();
        infoPlaceTransform = transform.parent.GetChild(0).Find("Image/infoPlace").transform;
        clickedSound = GameObject.Find("UIsounds/SliderSound").GetComponent<AudioSource>();
        myButtonScript = transform.parent.GetChild(0).GetComponent<AnyButton>();
    }

    private void Update()
    {
        transform.position = infoPlaceTransform.position;
    }

    public void InfoPressed()
    {
        infoEnabled = !infoEnabled;

        myInfoAnimator.SetBool("enabled", infoEnabled);

        clickedSound.Play();
    }

    public void InfoSelected()
    {
        if (!TouchControllersManager.isUsingPhone)
        {
            infoPlaceTransform.localScale = Vector2.one * 1.2f;
            myButtonScript.Pointed();
        }
    }

    public void InfoDisselected()
    {
        infoPlaceTransform.localScale = Vector2.one;
    }
}
