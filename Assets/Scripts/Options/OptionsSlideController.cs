using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsSlideController : MonoBehaviour
{
    [SerializeField] private float EnterExitSpeed;
    private float safeDistanceFromCamera, lerpProgress, moreOptionsDistance, moreOptionsLerpProgress;

    private bool entering = true, moreOptionsEntering;

    void Start()
    {
        safeDistanceFromCamera = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x * 3f;
        moreOptionsDistance = transform.Find("VolumeOptions").position.x;
    }

    void Update()
    {
        LerpProgressAdjuster();
        PositionLerp();

        if (entering && lerpProgress > 0.9f)
        {
            OptionsLerpProgressAdjuster();
            MoreOptionsLerp();
        }
    }

    private void LerpProgressAdjuster()
    {
        if (lerpProgress < 1 && entering)
        {
            lerpProgress += Time.unscaledDeltaTime * (1 - (lerpProgress + 0.05f)) * EnterExitSpeed;
            if (lerpProgress > 1) lerpProgress = 1;
        }

        if (lerpProgress > 0 && !entering)
        {
            lerpProgress -= Time.unscaledDeltaTime * (lerpProgress + 0.05f) * EnterExitSpeed;
            if (lerpProgress <= 0)
            {
                lerpProgress = 0;
                SceneManager.UnloadSceneAsync("Options");
            }
        }

        Debug.Log(lerpProgress);
    }

    private void OptionsLerpProgressAdjuster()
    {

    }

    private void PositionLerp()
    {
        transform.position = new Vector3(Mathf.Lerp(safeDistanceFromCamera, 0, lerpProgress), transform.position.y, transform.position.z);
    }

    private void MoreOptionsLerp()
    {
        transform.position = new Vector3(Mathf.Lerp(0, -moreOptionsDistance, moreOptionsLerpProgress), transform.position.y, transform.position.z);
    }

    public void GoToMoreOptions()
    {
        moreOptionsEntering = true;
    }

    public void ComeFromMoreOptions()
    {
        moreOptionsEntering = false;
    }

    public void OptionsExit()
    {
        entering = false;
    }
}
