using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnyButton : MonoBehaviour
{
    private Transform shadowTransform, resizerTransform;

    private AudioSource selectSound;

    [SerializeField] private float resizingSpeed;
    private float pointingLerp = 0.5f;

    [SerializeField] private bool doesntHaveShadow, isToggle;
    private bool pointed, clicked, reallyClicked;

    void Start()
    {
        if (!doesntHaveShadow) shadowTransform = transform.GetChild(0).transform;

        selectSound = GameObject.Find("UIsounds/SelectSound").GetComponent<AudioSource>();

        if (isToggle) resizerTransform = transform.Find("Background");
        else resizerTransform = transform.Find("Image");
    }

    void Update()
    {
        if (Input.GetButtonUp("Fire1") && !clicked && reallyClicked) reallyClicked = false;

        ChangePointedLerp();
        ChangeChildStats();
    }

    void ChangePointedLerp()
    {
        if ((!clicked && !pointed && pointingLerp < 0.5f) || (pointed && !clicked && pointingLerp < 1)) Growing();

        if (!clicked && !pointed && pointingLerp > 0.5f) Shrinking(1);
        if (clicked && pointed && pointingLerp > 0) Shrinking(5);
    }

    // Lerping Size --->

    private void Growing()
    {
        pointingLerp += Time.unscaledDeltaTime * resizingSpeed;

        if (!pointed)
        {
            if (pointingLerp > 0.5f) pointingLerp = 0.5f;
        }
        else if (pointingLerp > 1) pointingLerp = 1;
    }

    private void Shrinking(float multiplier)
    {
        pointingLerp -= Time.unscaledDeltaTime * resizingSpeed * multiplier;

        if (clicked)
        {
            if (pointingLerp < 0) pointingLerp = 0;
        }
        else if (pointingLerp < 0.5f) pointingLerp = 0.5f;
    }

    // <--- Lerping Size

    void ChangeChildStats()
    {
        resizerTransform.localScale = Vector2.one * Mathf.Lerp(0.85f, 1.15f, pointingLerp);
        if (!doesntHaveShadow)
        {
            shadowTransform.localPosition = new Vector2(Mathf.Lerp(0, -12, pointingLerp), Mathf.Lerp(0, -12, pointingLerp));
            shadowTransform.localScale = Vector2.one * Mathf.Lerp(0.85f, 1.15f, pointingLerp);
        }
    }

    // Simple Mouse Actions --->

    public void Pointed()
    {
        pointed = true;
        if (reallyClicked) clicked = true;
        PlayPitchSound(selectSound);
    }

    public void Unpointed()
    {
        pointed = false;
        clicked = false;
    }

    public void ClickedDown()
    {
        clicked = true;
        reallyClicked = true;
    }

    public void ClickedUp()
    {
        clicked = false;
        reallyClicked = false;
    }

    public void PlayPitchSound(AudioSource sound)
    {
        sound.pitch = Random.Range(0.9f, 1.1f);
        sound.Play();
    }
}
