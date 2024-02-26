using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsButton : MonoBehaviour
{
    private ScreenLoadAnim screenLoadScript;
    private ShakeController optionsShakeScript;
    private OptionsSlideController optionsSlideScript;

    private Transform shadowTransform;

    private AudioSource clickSound, selectSound, explosionSound;

    private float pointingLerp;

    [SerializeField] private bool doesntHaveShadow;
    private bool pointed;

    void Start()
    {
        if(!doesntHaveShadow) shadowTransform = transform.GetChild(0).transform;

        clickSound = GameObject.Find("UIsounds/ClickSound").GetComponent<AudioSource>();
        selectSound = GameObject.Find("UIsounds/SelectSound").GetComponent<AudioSource>();
        explosionSound = GameObject.Find("UIsounds/ExplosionSound").GetComponent<AudioSource>();
        screenLoadScript = GameObject.Find("CanvasOptions/ScreenLoadUnloadOptions").GetComponent<ScreenLoadAnim>();
        optionsShakeScript = GameObject.Find("CanvasOptions/Options").GetComponent<ShakeController>();
        optionsSlideScript = GameObject.Find("CanvasOptions/Options").GetComponent<OptionsSlideController>();
    }

    void Update()
    {
        ChangePointedLerp();
        ChangeChildStats();
    }

    void ChangePointedLerp()
    {
        if (pointed && pointingLerp < 1) pointingLerp += Time.unscaledDeltaTime * 18;
        if (!pointed && pointingLerp > 0) pointingLerp -= Time.unscaledDeltaTime * 18;

        if (pointingLerp < 0) pointingLerp = 0;
        else if (pointingLerp > 1) pointingLerp = 1;
    }

    void ChangeChildStats()
    {
        transform.localScale = Vector2.one * Mathf.Lerp(1f, 1.15f, pointingLerp);
        if (!doesntHaveShadow) shadowTransform.localPosition = new Vector2(Mathf.Lerp(-6, -12, pointingLerp), Mathf.Lerp(-6, -12, pointingLerp));
    }

    public void Pointed()
    {
        pointed = true;
        PlayPitchSound(selectSound);
    }

    public void Unpointed()
    {
        pointed = false;
    }

    public void BackPressed()
    {
        PlayPitchSound(clickSound);
        screenLoadScript.LoadScene("Options", "Options");
        GameObject.Find("OptionsInitializer").GetComponent<OptionsInitializer>().StartEverything();
    }

    public void ShakePressed()
    {
        optionsShakeScript.SetDefinitiveMaxRadius(PlayerPrefs.GetFloat("ScreenshakeValue"));
        optionsShakeScript.Shake();
        PlayPitchSound(explosionSound);
    }

    public void MoreOptionsButton()
    {
        optionsSlideScript.GoToMoreOptions();
        PlayPitchSound(clickSound);
    }

    public void LessOptionsButton()
    {
        optionsSlideScript.ComeFromMoreOptions();
        PlayPitchSound(clickSound);
    }

    public void PlayPitchSound(AudioSource sound)
    {
        sound.pitch = Random.Range(0.9f, 1.1f);
        sound.Play();
    }
}
