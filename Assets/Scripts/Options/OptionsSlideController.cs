using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsSlideController : MonoBehaviour
{
    private Transform gameCameraTransform, advancedOptionsTransform;
    private HudController hudScript;
    private Animator cameraAnim;

    [SerializeField] private float enterExitSpeed;
    private float safeDistanceFromCamera, lerpProgress, realCameraRotation;

    private bool entering = true, advancedOptionsEntering, imInGameScene;

    void Start()
    {
        safeDistanceFromCamera = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x * 4f;

        // Avoiding transparent area between the two options menus when lerping:
        Transform emptyColorFillTransform = transform.Find("EmptyColorFill");
        emptyColorFillTransform.position = new Vector3(safeDistanceFromCamera / 2, emptyColorFillTransform.position.y, emptyColorFillTransform.position.z);

        advancedOptionsTransform = transform.Find("AdvancedOptions");
        advancedOptionsTransform.position = new Vector3(safeDistanceFromCamera, advancedOptionsTransform.position.y, advancedOptionsTransform.position.z);

        if (GameObject.Find("Camera/CameraRiser/Main Camera")) imInGameScene = true;
            
        if (imInGameScene) gameCameraTransform = GameObject.Find("Camera/CameraRiser/Main Camera").transform;
        else gameCameraTransform = GameObject.Find("Camera/Main Camera").transform;

        GameObject.Find("CanvasOptions").GetComponent<Canvas>().worldCamera = gameCameraTransform.GetComponent<Camera>();
        realCameraRotation = gameCameraTransform.eulerAngles.z;

        if (imInGameScene) cameraAnim = GameObject.Find("Camera").GetComponent<Animator>();
        else cameraAnim = gameCameraTransform.parent.GetComponent<Animator>();

        if (imInGameScene)
        {
            hudScript = GameObject.Find("________________Canvas________________").GetComponent<HudController>();
            hudScript.SetIsInOptions = true;
        }
    }

    void Update()
    {
        LerpProgressAdjuster();
        PositionLerp();

        transform.localEulerAngles = new Vector3(0, 0, - realCameraRotation + gameCameraTransform.eulerAngles.z);

        if (Input.GetButtonUp("Pause"))
        {
            if (advancedOptionsEntering) ComeFromAdvancedOptions();
            else OptionsExit();
        }
    }

    private void LerpProgressAdjuster()
    {
        // To Understand the mathematical functions, copypaste them on www.geogebra.org/classic without the +0.001. I don't want to explain.
        // +0.001 in all of them is for the menu to get a little bit faster to the destination position and, therefore, stop when it seems to be stopped for
        // prevent it from keep adding very little useless decimals forever, and also to make it easier to debug at the end of the positioning, you're welcome.

        // Default Options

        if (lerpProgress > 0 && !entering) { LerpingOptionsOut(); return; }

        if (lerpProgress < 0.5f && !advancedOptionsEntering) LerpingOptionsIn();

        // Advanced Options

        if (advancedOptionsEntering && lerpProgress < 1) LerpingAdvancedOptionsIn();

        if (!advancedOptionsEntering && lerpProgress > 0.5f) LerpingAdvancedOptionsOut();
    }

    // Lerping Processes -->

    private void LerpingOptionsIn()
    {
        float smoothingFactor = -lerpProgress * 2 + 1 + 0.001f; // lerpProgress duplied for it to cover a half of 0 - 1, the possible range of values for lerpProgress.
        lerpProgress += Time.unscaledDeltaTime * smoothingFactor * enterExitSpeed;
        // My brain is a fucking meme. In smoothingFactor, negative lerpProgress is because we want to invert the function; more lerpProgress, less speed.
        // If you really want to know how all these simple functions work just copypaste them on www.geogebra.org/classic, you'll understand instantly.

        if (lerpProgress > 1)
        {
            lerpProgress = 1;
        }
    }

    private void LerpingAdvancedOptionsIn()
    {
        float smoothingFactor = -lerpProgress * 2 + 2 + 0.001f;
        lerpProgress += Time.unscaledDeltaTime * smoothingFactor * enterExitSpeed;

        if (lerpProgress > 1)
        {
            lerpProgress = 1;
        }
    }

    private void LerpingAdvancedOptionsOut()
    {
        float smoothingFactor = lerpProgress * 2 - 1 + 0.001f;
        lerpProgress -= Time.unscaledDeltaTime * smoothingFactor * enterExitSpeed;

        if (lerpProgress < 0.5f)
        {
            lerpProgress = 0.5f;
        }
    }

    private void LerpingOptionsOut()
    {
        float smoothingFactor = lerpProgress * 2 + 0.001f;
        lerpProgress -= Time.unscaledDeltaTime * smoothingFactor * enterExitSpeed;

        if (lerpProgress <= 0)
        {
            lerpProgress = 0;
            SceneManager.UnloadSceneAsync("Options");
        }
    }

    // <-- Lerping Processes

    private void PositionLerp()
    {
        float yDifferenceFromCamera = Mathf.Tan(gameCameraTransform.eulerAngles.z * Mathf.Deg2Rad);

        transform.position = new Vector3(Mathf.Lerp(safeDistanceFromCamera, -safeDistanceFromCamera, lerpProgress), Mathf.Lerp(safeDistanceFromCamera * yDifferenceFromCamera, -safeDistanceFromCamera * yDifferenceFromCamera, lerpProgress), transform.position.z);

        // For some rason, advanced options changes its position, so it's repositioned:
        advancedOptionsTransform.position = new Vector3(transform.position.x + safeDistanceFromCamera, Mathf.Lerp(safeDistanceFromCamera * yDifferenceFromCamera, -safeDistanceFromCamera * yDifferenceFromCamera, lerpProgress) + safeDistanceFromCamera * yDifferenceFromCamera, advancedOptionsTransform.position.z);
    }

    // Functs called from buttons -->

    public void GoToAdvancedOptions()
    {
        advancedOptionsEntering = true;
        CameraSlide(true);
    }

    public void ComeFromAdvancedOptions()
    {
        advancedOptionsEntering = false;
        CameraSlide(false);
    }

    public void OptionsExit()
    {
        entering = false;
        if (imInGameScene) hudScript.SetIsInOptions = false;
    }

    private void CameraSlide(bool isRight)
    {
        if (isRight) cameraAnim.SetTrigger("SlideRight");
        else cameraAnim.SetTrigger("SlideLeft");
    }
}