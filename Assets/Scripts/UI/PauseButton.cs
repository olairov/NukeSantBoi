using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseButton : MonoBehaviour
{
    private HudController hudScript;
    
    private Transform childTransform;

    private float pointingLerp, myYpos;

    private bool pointed;

    void Start()
    {
        childTransform = transform.GetChild(0).transform;

        hudScript = GameObject.Find("________________Canvas________________").GetComponent<HudController>();

        myYpos = transform.position.y;
    }

    void Update()
    {
        ChangePointedLerp();
        ChangeChildStats();
    }

    void ChangePointedLerp()
    {
        if (pointed && pointingLerp < 1) pointingLerp += Time.unscaledDeltaTime * 8;
        if (!pointed && pointingLerp > 0) pointingLerp -= Time.unscaledDeltaTime * 8;

        if (pointingLerp < 0) pointingLerp = 0;
        else if (pointingLerp > 1) pointingLerp = 1;
    }

    void ChangeChildStats()
    {
        childTransform.localScale = new Vector2(Mathf.Lerp(1f, 1.2f, pointingLerp), Mathf.Lerp(1f, 1.3f, pointingLerp));
        childTransform.localPosition = new Vector2(0, Mathf.Lerp(myYpos, myYpos + 6, pointingLerp));
    }

    public void Pointed()
    {
        pointed = true;
    }

    public void Unpointed()
    {
        pointed = false;
    }

    public void ContinuePressed()
    {
        hudScript.Continue();
    }

    public void MenuPressed()
    {
        Time.timeScale = 1;
        hudScript.SetIsPaused = false;
        SceneManager.LoadScene("Menu");
    }

    public void RetryPressed()
    {
        SceneManager.LoadScene("Game");
    }

    public void OptionsPressed()
    {
        hudScript.SetInOptions = true;
    }

    public void BackPressed()
    {
        hudScript.SetInOptions = false;
    }
}