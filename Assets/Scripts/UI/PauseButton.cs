using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButton : MonoBehaviour
{
    private HudController hudScript;
    private ScreenLoadAnim screenLoadScript;

    private Transform cameraTransform;

    private AudioSource clickSound, selectSound;

    private Transform childTransform;

    private float pointingLerp, myYpos, cameraZpos;

    private bool pointed;

    void Start()
    {
        childTransform = transform.GetChild(0).transform;

        hudScript = GameObject.Find("________________Canvas________________").GetComponent<HudController>();
        clickSound = GameObject.Find("UIsounds/ClickSound").GetComponent<AudioSource>();
        selectSound = GameObject.Find("UIsounds/SelectSound").GetComponent<AudioSource>();
        screenLoadScript = GameObject.Find("Canvas/ScreenLoadUnload").GetComponent<ScreenLoadAnim>();
        cameraTransform = Camera.main.transform;
        cameraZpos = cameraTransform.position.z;

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
        if (SceneManager.sceneCount > 1) SceneManager.UnloadSceneAsync("Options");
        SceneManager.LoadScene("Options", LoadSceneMode.Additive);
        // PlayPitchSound(clickSound);   ---> For some reason, the options menu already makes this sound when it starts and I don't know how to erase it.
        // It played twice at the same time, so erasing this line leaves only the other sound from unknown source and change isn't noticeable.
    }

    private void PlayPitchSound(AudioSource sound)
    {
        sound.pitch = Random.Range(0.9f, 1.1f);
        sound.Play();
    }
}