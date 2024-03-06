using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnyButton : MonoBehaviour
{
    private ShakeController myShake;

    [SerializeField] private Transform resizerTransform; // Leave blank if it's a default button.
    private Transform shadowTransform;

    private AudioSource selectSound, clickSound, pressSound;

    [SerializeField] private float resizingSpeed;
    private float pointingLerp = 0.5f;

    [SerializeField] private bool doesntHaveShadow, imSlider;
    private bool pointed, clicked, reallyClicked;

    void Start()
    {
        if (!doesntHaveShadow) shadowTransform = transform.Find("Image/Shadow").transform;

        selectSound = GameObject.Find("UIsounds/SelectSound").GetComponent<AudioSource>();
        clickSound = GameObject.Find("UIsounds/ClickSound").GetComponent<AudioSource>();
        pressSound = GameObject.Find("UIsounds/PressSound").GetComponent<AudioSource>();

        if (resizerTransform == null) resizerTransform = transform.Find("Image");
        if (!imSlider) myShake = resizerTransform.GetComponent<ShakeController>();
    }

    void Update()
    {
        if (Input.GetButtonUp("Fire1") && !clicked && reallyClicked) reallyClicked = false;
        if (Input.GetButtonUp("Fire1") && imSlider && clicked)
        {
            pointed = false;
            ClickedUp();
        }

        ChangePointedLerp();
        ChangeChildStats();
    }

    void ChangePointedLerp() // Decision-making based in pointingLerp value and the pointer state in relation to the button.
    {
        if ((!clicked && !pointed && pointingLerp < 0.5f) || (pointed && !clicked && pointingLerp < 1)) Growing();

        if (!clicked && !pointed && pointingLerp > 0.5f) Shrinking(1);
        if (clicked && pointed && pointingLerp > 0) Shrinking(5); // Increase the speed of the shrinking to create a better effect of strength.
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

        if (!doesntHaveShadow) shadowTransform.localPosition = new Vector2(Mathf.Lerp(0, -12, pointingLerp), Mathf.Lerp(0, -12, pointingLerp));
    }

    // Simple Mouse Actions --->

    public void Pointed()
    {
        pointed = true;

        if (!clicked) PlayPitchSound(selectSound);
        if (reallyClicked) clicked = true;
    }

    public void Unpointed()
    {
        if (imSlider && reallyClicked) return;

        pointed = false;
        clicked = false;
    }

    public void ClickedDown()
    {
        clicked = true;
        reallyClicked = true;

        if (!imSlider) myShake.Shake();
        PlayPitchSound(clickSound);
    }

    public void ClickedUp()
    {
        clicked = false;
        reallyClicked = false;
    }

    // Sound when clicked --->

    public void PlayPitchSound(AudioSource sound)
    {
        sound.pitch = Random.Range(0.9f, 1.1f);
        sound.Play();
    }
}
