using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AnyButton : MonoBehaviour
{
    private ShakeController myShake;

    [SerializeField] private Transform resizerTransform; // Leave blank if it's a default button.
    private Transform shadowTransform;

    private AudioSource selectSound, clickSound, sliderSound;

    private Slider mySlider;

    private Vector2 defaultSize = Vector2.one;
    public Vector2 SetDefaultSize
    {
        set { defaultSize = value; }
    }

    [SerializeField] private float resizingSpeed, amountOfSliderSoundPlaces;
    private float pointingLerp, clickedLerp = 1, lastSliderValue, timeSinceLastSliderSound;
    // PointigLerp is 0 when not pointed, ClickedLerp is 1 when not clicked.

    [SerializeField] private bool doesntHaveShadow, imSlider;
    private bool pointed, clicked, reallyClicked, cantMakeSelectedSound, ableToGrowWhenInPhoneDevice;
    public static bool canBePressed = true;
    public bool AbleToGrowWhenInPhoneDevice
    {
        set { ableToGrowWhenInPhoneDevice = value; }
    }
    public bool SetPointed
    {
        set { pointed = value; }
    }

    void Start()
    {
        if (!doesntHaveShadow) shadowTransform = transform.Find("Image/Shadow").transform;

        if (GameObject.Find("UIsounds"))
        {
            selectSound = GameObject.Find("UIsounds/SelectSound").GetComponent<AudioSource>();
            clickSound = GameObject.Find("UIsounds/ClickSound").GetComponent<AudioSource>();
            sliderSound = GameObject.Find("UIsounds/SliderSound").GetComponent<AudioSource>();
        }
        else if (GameObject.Find("UIsoundsOptions"))
        {
            selectSound = GameObject.Find("UIsoundsOptions/SelectSound").GetComponent<AudioSource>();
            clickSound = GameObject.Find("UIsoundsOptions/ClickSound").GetComponent<AudioSource>();
            sliderSound = GameObject.Find("UIsoundsOptions/SliderSound").GetComponent<AudioSource>();
        }

        if (resizerTransform == null) resizerTransform = transform.Find("Image");
        if (!imSlider) myShake = resizerTransform.GetComponent<ShakeController>();
        else
        {
            mySlider = transform.GetComponent<Slider>();

            lastSliderValue = GetCastedSliderValue();
        }
    }

    void Update()
    {
        if (!canBePressed) return;

        if (Input.GetButtonUp("Fire1") && !clicked && reallyClicked) reallyClicked = false;
        if (Input.GetButtonUp("Fire1") && clicked)
        {
            if (imSlider) pointed = false;
            ClickedUp();
        }

        ChangePointedLerp();
        ChangeChildStats();

        if (imSlider) ManageSliderSound();

        cantMakeSelectedSound = false;
    }

    void ManageSliderSound()
    {
        if (timeSinceLastSliderSound > 0.07f)
        {
            if (GetCastedSliderValue() != lastSliderValue)
            {
                sliderSound.pitch = mySlider.value / 2 + 0.75f;
                sliderSound.Play();

                timeSinceLastSliderSound = 0;
                lastSliderValue = GetCastedSliderValue();
            }
        }
        else timeSinceLastSliderSound += Time.unscaledDeltaTime;
    }

    void ChangePointedLerp() // Decision-making based in pointingLerp / clickedLerp value and the pointer state in relation to the button.
    {
        if (!clicked) clickedLerp = 1;

        if (!clicked && !pointed)
        {
            if (clickedLerp < 1) clickedLerp = Growing(clickedLerp);
            else if (pointingLerp > 0) pointingLerp = Shrinking(pointingLerp, 1);
        }

        if (pointed)
        {
            if (!clicked && pointingLerp < 1) pointingLerp = Growing(pointingLerp);
            else if (clicked && clickedLerp > 0) clickedLerp = pointingLerp = Shrinking(clickedLerp, 5);
        }
    }

    // Lerping Size --->

    private float Growing(float valueToGrow)
    {
        valueToGrow += Time.unscaledDeltaTime * resizingSpeed;

        if (valueToGrow > 1) valueToGrow = 1;

        return valueToGrow;
    }

    private float Shrinking(float valueToShrink, float multiplier)
    {
        valueToShrink -= Time.unscaledDeltaTime * resizingSpeed * multiplier;

        if (valueToShrink < 0) valueToShrink = 0;

        return valueToShrink;
    }

    // <--- Lerping Size

    void ChangeChildStats()
    {
        resizerTransform.localScale = Vector2.Lerp(defaultSize, defaultSize * 1.15f, pointingLerp);
        if (clickedLerp < 1) resizerTransform.localScale = Vector2.Lerp(defaultSize * 0.85f, defaultSize * 1.15f, clickedLerp);

        if (!doesntHaveShadow) shadowTransform.localPosition = new Vector2(Mathf.Lerp(-6, -12, pointingLerp), Mathf.Lerp(-6, -12, pointingLerp));
    }

    // Simple Mouse Actions --->

    public void Pointed()
    {
        if (TouchControllersManager.isUsingPhone && !ableToGrowWhenInPhoneDevice) return;
        pointed = true;

        if (!clicked && !TouchControllersManager.isUsingPhone && !cantMakeSelectedSound && !selectSound.isPlaying && !ableToGrowWhenInPhoneDevice) PlayPitchSound(selectSound);
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

        if (!imSlider && pointed) myShake.Shake();
        PlayPitchSound(clickSound);
    }

    public void ClickedUp()
    {
        clicked = false;
        reallyClicked = false;
    }

    // <--- Simple Mouse Actions

    private float GetCastedSliderValue()
    {
        return (float)(((int)(mySlider.value * amountOfSliderSoundPlaces)) / amountOfSliderSoundPlaces);
    }

    // Sound when clicked --->

    public void PlayPitchSound(AudioSource sound)
    {
        sound.pitch = Random.Range(0.9f, 1.1f);
        sound.Play();
    }

    public void StopSelectedSound()
    {
        cantMakeSelectedSound = true;
    }
}
