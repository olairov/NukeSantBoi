using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsSlideController : MonoBehaviour
{
    private Transform GameCameraTransform;

    [SerializeField] private float EnterExitSpeed;
    private float safeDistanceFromCamera, lerpProgress, moreOptionsDistance, moreOptionsLerpProgress, yRealDifferenceFromCamera, realCameraRotation;

    private bool entering = true, moreOptionsEntering;

    void Start()
    {
        safeDistanceFromCamera = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x * 4.5f;
        moreOptionsDistance = transform.Find("VolumeOptions").position.x;
        GameCameraTransform = GameObject.Find("Camera/CameraRiser/Main Camera").transform;

        GameObject.Find("CanvasOptions").GetComponent<Canvas>().worldCamera = GameCameraTransform.GetComponent<Camera>();
        yRealDifferenceFromCamera = Mathf.Tan(GameCameraTransform.eulerAngles.z * Mathf.Deg2Rad) * safeDistanceFromCamera;
        realCameraRotation = GameCameraTransform.eulerAngles.z;
    }

    void Update()
    {
        LerpProgressAdjuster();
        PositionLerp();

        if (entering && lerpProgress > 0.99f && moreOptionsEntering)
        {
            OptionsLerpProgressAdjuster();
            MoreOptionsLerp();
        }

        //transform.eulerAngles = new Vector3(0, 0, GameCameraTransform.eulerAngles.z);
        //Busca otra manera, esto no va
    }

    private void LerpProgressAdjuster()
    {
        if (lerpProgress < 1 && entering)
        {
            lerpProgress += Time.unscaledDeltaTime * (1 - (lerpProgress + 0.005f)) * EnterExitSpeed;
            if (lerpProgress > 1) lerpProgress = 1;
        }

        if (lerpProgress > 0 && !entering)
        {
            lerpProgress -= Time.unscaledDeltaTime * (lerpProgress + 0.005f) * EnterExitSpeed;
            if (lerpProgress <= 0)
            {
                lerpProgress = 0;
                SceneManager.UnloadSceneAsync("Options");
            }
        }
    }

    private void OptionsLerpProgressAdjuster()
    {

    }

    private void PositionLerp()
    {
        float yDifferenceFromCamera = Mathf.Tan(GameCameraTransform.eulerAngles.z * Mathf.Deg2Rad);

        transform.position = new Vector3(Mathf.Lerp(safeDistanceFromCamera, 0, lerpProgress), Mathf.Lerp(0, -safeDistanceFromCamera * yDifferenceFromCamera, lerpProgress) + yRealDifferenceFromCamera, transform.position.z);
        // Mathf.Lerp(0, -safeDistanceFromCamera * yDifferenceFromCamera, lerpProgress)
    }

    private void MoreOptionsLerp()
    {
        float yDifferenceFromCamera = Mathf.Tan(GameCameraTransform.eulerAngles.z * Mathf.Deg2Rad);

        transform.position = new Vector3(Mathf.Lerp(0, -moreOptionsDistance, moreOptionsLerpProgress), Mathf.Lerp(0, -safeDistanceFromCamera * yDifferenceFromCamera, moreOptionsLerpProgress), transform.position.z);
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
