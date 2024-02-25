using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    private HudController hudScript;
    private ScreenLoadAnim screenLoadScript;

    private AudioSource clickSound, selectSound;

    private Transform childTransform;

    private float pointingLerp, myYpos;

    private bool pointed;

    void Start()
    {
        childTransform = transform.GetChild(0).transform;

        hudScript = GameObject.Find("________________Canvas________________").GetComponent<HudController>();
        clickSound = GameObject.Find("UIsounds/ClickSound").GetComponent<AudioSource>();
        selectSound = GameObject.Find("UIsounds/SelectSound").GetComponent<AudioSource>();
        screenLoadScript = GameObject.Find("Canvas/ScreenLoadUnload").GetComponent<ScreenLoadAnim>();

        myYpos = transform.position.y;
    }

    void Update()
    {
        ChangePointedLerp();
        ChangeChildStats();
    }

    void ChangePointedLerp()
    {
        if (pointed && pointingLerp < 1) pointingLerp += Time.unscaledDeltaTime * 20;
        if (!pointed && pointingLerp > 0) pointingLerp -= Time.unscaledDeltaTime * 20;

        if (pointingLerp < 0) pointingLerp = 0;
        else if (pointingLerp > 1) pointingLerp = 1;
    }

    void ChangeChildStats()
    {
        childTransform.localScale = new Vector2(Mathf.Lerp(1f, 1.1f, pointingLerp), Mathf.Lerp(1f, 1.1f, pointingLerp));
        childTransform.localPosition = new Vector2(0, Mathf.Lerp(myYpos, myYpos + 3, pointingLerp));
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

    public void ContinuePressed()
    {
        hudScript.Continue();
        PlayPitchSound(clickSound);
    }

    public void MenuPressed()
    {
        Time.timeScale = 1;
        hudScript.SetIsPaused = false;
        screenLoadScript.LoadScene("Menu", "Game");
        PlayPitchSound(clickSound);
        hudScript.SetChangingScene = true;
    }

    public void RetryPressed()
    {
        screenLoadScript.LoadScene("Game", "Game");
        PlayPitchSound(clickSound);
        hudScript.SetChangingScene = true;
    }

    public void OptionsPressed()
    {
        hudScript.SetInOptions = true;
        PlayPitchSound(clickSound);
    }

    public void BackPressed()
    {
        hudScript.SetInOptions = false;
        PlayPitchSound(clickSound);
    }

    private void PlayPitchSound(AudioSource sound)
    {
        sound.pitch = Random.Range(0.9f, 1.1f);
        sound.Play();
    }
}