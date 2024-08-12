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

    [SerializeField] private float resizingSpeed, amountOfSliderSoundPlaces;
    private float pointingLerp = 0.5f, lastSliderValue, timeSinceLastSliderSound;

    [SerializeField] private bool doesntHaveShadow, imSlider;
    private bool pointed, clicked, reallyClicked;

    void Start()
    {
        if (!doesntHaveShadow) shadowTransform = transform.Find("Image/Shadow").transform;

        if (GameObject.Find("UIsounds"))
        {
            selectSound = GameObject.Find("UIsounds/SelectSound").GetComponent<AudioSource>();
            clickSound = GameObject.Find("UIsounds/ClickSound").GetComponent<AudioSource>();
            sliderSound = GameObject.Find("UIsounds/SliderSound").GetComponent<AudioSource>();
        }
        else
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
        if (Input.GetButtonUp("Fire1") && !clicked && reallyClicked) reallyClicked = false;
        if (Input.GetButtonUp("Fire1") && imSlider && clicked)
        {
            pointed = false;
            ClickedUp();
        }

        ChangePointedLerp();
        ChangeChildStats();

        if (imSlider)
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
    }

    void ChangePointedLerp() // Decision-making based in pointingLerp value and the pointer state in relation to the button.
    {
        if ((!clicked && !pointed && pointingLerp < 0.5f) || (pointed && !clicked && pointingLerp < 1 && !TouchControllersManager.isUsingPhone)) Growing();

        if (!clicked && !pointed && pointingLerp > 0.5f) Shrinking(1);
        if (clicked && pointed && pointingLerp > 0) Shrinking(5); // Increase the speed of the shrinking to create a most powerful effect.
    }

    // Lerping Size --->

    private void Growing()
    {
        pointingLerp += Time.unscaledDeltaTime * resizingSpeed;

        if (!pointed || TouchControllersManager.isUsingPhone)
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

        if (!clicked && !TouchControllersManager.isUsingPhone) PlayPitchSound(selectSound);
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
}
