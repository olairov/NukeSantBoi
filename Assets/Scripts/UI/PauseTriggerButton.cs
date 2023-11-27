using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseTriggerButton : MonoBehaviour
{
    private HudController hudScript;
    private PlayerController playerScript;

    private float pointingLerp;

    private bool pointed;

    void Start()
    {
        hudScript = GameObject.Find("________________Canvas________________").GetComponent<HudController>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        ChangePointedLerp();
        ChangeStats();
    }

    void ChangePointedLerp()
    {
        if (pointed && pointingLerp < 1) pointingLerp += Time.unscaledDeltaTime * 8;
        if (!pointed && pointingLerp > 0) pointingLerp -= Time.unscaledDeltaTime * 8;

        if (pointingLerp < 0) pointingLerp = 0;
        else if (pointingLerp > 1) pointingLerp = 1;
    }

    void ChangeStats()
    {
        transform.localScale = new Vector2(Mathf.Lerp(1f, 1.2f, pointingLerp), Mathf.Lerp(1f, 1.3f, pointingLerp));
    }

    public void Pointed()
    {
        pointed = true;
        playerScript.SetMouseIsUnaccessible = true;
    }

    public void Unpointed()
    {
        pointed = false;
        playerScript.SetMouseIsUnaccessible = false;
    }

    public void Pressed()
    {
        hudScript.Pause();
    }
}
