using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsButton : MonoBehaviour
{
    private ScreenLoadAnim screenLoadScript;

    private Transform shadowTransform;

    private AudioSource clickSound, selectSound;

    private float pointingLerp;

    private bool pointed;

    void Start()
    {
        shadowTransform = transform.GetChild(0).transform;

        clickSound = GameObject.Find("UIsounds/ClickSound").GetComponent<AudioSource>();
        selectSound = GameObject.Find("UIsounds/SelectSound").GetComponent<AudioSource>();
        screenLoadScript = GameObject.Find("Canvas/ScreenLoadUnloadOptions").GetComponent<ScreenLoadAnim>();
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
        shadowTransform.localPosition = new Vector2(Mathf.Lerp(-6, -12, pointingLerp), Mathf.Lerp(-6, -12, pointingLerp));
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
    }

    private void PlayPitchSound(AudioSource sound)
    {
        sound.pitch = Random.Range(0.9f, 1.1f);
        sound.Play();
    }
}
