using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    private Transform imageChildTransform, textChildTransform;
    
    private Image childImage;

    private float pointingLerp;
    
    private bool pointed;

    void Start()
    {
        imageChildTransform = transform.GetChild(0).transform;
        textChildTransform = transform.GetChild(1).transform;

        childImage = transform.GetChild(0).GetComponent<Image>();
    }

    void Update()
    {
        ChangePointedLerp();
        ChangeChildStats();
    }

    void ChangePointedLerp()
    {
        if (pointed && pointingLerp < 1) pointingLerp += Time.unscaledDeltaTime * 5;
        if (!pointed && pointingLerp > 0) pointingLerp -= Time.unscaledDeltaTime * 5;

        if (pointingLerp < 0) pointingLerp = 0;
        else if (pointingLerp > 1) pointingLerp = 1;
    }

    void ChangeChildStats()
    {
        imageChildTransform.localScale = new Vector2(1, Mathf.Lerp(1f, 1.3f, pointingLerp));
        textChildTransform.localScale = new Vector2(Mathf.Lerp(1f, 1.8f, pointingLerp), Mathf.Lerp(1f, 1.8f, pointingLerp));
        childImage.color = new Color(childImage.color.r, childImage.color.g, childImage.color.b, Mathf.Lerp(0.6f, 0.9f, pointingLerp));
    }

    public void Pointed()
    {
        pointed = true;
    }

    public void Unpointed()
    {
        pointed = false;
    }

    public void PlayPressed()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExitPressed()
    {
        Application.Quit();
    }
}
