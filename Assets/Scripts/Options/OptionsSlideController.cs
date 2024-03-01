using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsSlideController : MonoBehaviour
{
    private Transform gameCameraTransform;

    [SerializeField] private float enterExitSpeed;
    private float safeDistanceFromCamera, lerpProgress, moreOptionsLerpProgress, yRealDifferenceFromCamera, realCameraRotation;

    private bool entering = true, moreOptionsEntering;

    void Start()
    {
        Debug.Log("MyPos: " + transform.position.x + " MoreOptionsPos: " + transform.Find("VolumeOptions").position.x);
        safeDistanceFromCamera = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x * 4f;
        Debug.Log("SafeDistanceFromCamera: " + safeDistanceFromCamera);

        Transform emptyColorFillTransform = transform.Find("EmptyColorFill");
        emptyColorFillTransform.position = new Vector3(safeDistanceFromCamera / 2, emptyColorFillTransform.position.y, emptyColorFillTransform.position.z);

        Transform moreOptionsTransform = transform.Find("VolumeOptions");
        moreOptionsTransform.position = new Vector3(safeDistanceFromCamera, moreOptionsTransform.position.y, moreOptionsTransform.position.z);
        gameCameraTransform = GameObject.Find("Camera/CameraRiser/Main Camera").transform;

        GameObject.Find("CanvasOptions").GetComponent<Canvas>().worldCamera = gameCameraTransform.GetComponent<Camera>();
        realCameraRotation = gameCameraTransform.eulerAngles.z;
        yRealDifferenceFromCamera = Mathf.Tan(realCameraRotation * Mathf.Deg2Rad) * safeDistanceFromCamera;

        // no se por que no sé calcular la distancia en X de las opciones default a las opciones de volumen.
    }

    void Update()
    {
        LerpProgressAdjuster();
        PositionLerp();

        OptionsLerpProgressAdjuster();
        if (entering && lerpProgress > 0.95f && moreOptionsLerpProgress > 0)
        {
            //MoreOptionsLerp();
        }

        transform.localEulerAngles = new Vector3(0, 0, - realCameraRotation + gameCameraTransform.eulerAngles.z);
    }

    /*private void LerpProgressAdjuster()
    {
        if (lerpProgress < 1 && entering)
        {
            lerpProgress += Time.unscaledDeltaTime * (1 - (lerpProgress - 0.001f)) * enterExitSpeed;
            if (lerpProgress > 1)
            {
                lerpProgress = 1;
                Debug.Log("MyPos: " + transform.position.x + " MoreOptionsPos: " + transform.Find("VolumeOptions").position.x);
            }
        }

        if (lerpProgress > 0 && !entering)
        {
            lerpProgress -= Time.unscaledDeltaTime * (lerpProgress + 0.001f) * enterExitSpeed;
            if (lerpProgress <= 0)
            {
                lerpProgress = 0;
                Debug.Log("MyPos: " + transform.position.x + " MoreOptionsPos: " + transform.Find("VolumeOptions").position.x);
                SceneManager.UnloadSceneAsync("Options");
            }
        }
    }*/

    private void LerpProgressAdjuster()
    {
        if (lerpProgress > 0 && !entering)
        {
            float smoothingFactor = lerpProgress + 0.001f;
            lerpProgress -= Time.unscaledDeltaTime * smoothingFactor * enterExitSpeed;
            // The substraction of time multiplied by the slide speed is also multiplied by smoothingFactor for it to be slower when closer to the target,
            // but 0.001 is added to it to it to arrive to the target rapidly and not getting closer and closer but never there.
            if (lerpProgress <= 0)
            {
                lerpProgress = 0;
                SceneManager.UnloadSceneAsync("Options");
            }
            return;
        }

        if (lerpProgress < 0.5f)
        {
            lerpProgress += Time.unscaledDeltaTime * (1 - (lerpProgress - 0.001f)) * enterExitSpeed;
            if (lerpProgress > 1)
            {
                lerpProgress = 1;
            }
        }
    }

    private void OptionsLerpProgressAdjuster()
    {
        if (moreOptionsLerpProgress < 1 && moreOptionsEntering)
        {
            moreOptionsLerpProgress += Time.unscaledDeltaTime * (1 - (moreOptionsLerpProgress - 0.001f)) * enterExitSpeed;
            if (moreOptionsLerpProgress > 1)
            {
                moreOptionsLerpProgress = 1;
            }
        }

        if (moreOptionsLerpProgress > 0 && !moreOptionsEntering)
        {
            moreOptionsLerpProgress -= Time.unscaledDeltaTime * (moreOptionsLerpProgress + 0.001f) * enterExitSpeed;
            if (moreOptionsLerpProgress <= 0)
            {
                moreOptionsLerpProgress = 0;
            }
        }
    }

    private void PositionLerp()
    {
        float yDifferenceFromCamera = Mathf.Tan(gameCameraTransform.eulerAngles.z * Mathf.Deg2Rad);

        transform.position = new Vector3(Mathf.Lerp(safeDistanceFromCamera, 0, lerpProgress), Mathf.Lerp(0, -safeDistanceFromCamera * yDifferenceFromCamera, lerpProgress) + yRealDifferenceFromCamera, transform.position.z);
    }

    /*private void MoreOptionsLerp()
    {
        float yDifferenceFromCamera = Mathf.Tan(gameCameraTransform.eulerAngles.z * Mathf.Deg2Rad);

        transform.position = new Vector3(Mathf.Lerp(0, -safeDistanceFromCamera, moreOptionsLerpProgress), Mathf.Lerp(0, -safeDistanceFromCamera * yDifferenceFromCamera, moreOptionsLerpProgress), transform.position.z);
    }*/

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
