using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsSlideController : MonoBehaviour
{
    private Transform gameCameraTransform, moreOptionsTransform;
    private HudController hudScript;
    private Animator cameraAnim;
    private AudioSource menuEnterSound, menuExitSound;

    [SerializeField] private float enterExitSpeed;
    private float safeDistanceFromCamera, lerpProgress, realCameraRotation;

    private bool entering = true, advancedOptionsEntering, imInGameScene, notOtherSceneActive = true;

    void Start()
    {
        if (SceneManager.sceneCount > 1)
        {
            GameObject.Find("OptionsCamera").SetActive(false);
            if (GameObject.Find("Camera/CameraRiser/Main Camera")) imInGameScene = true;

            notOtherSceneActive = false;
            GameObject.Find("EventSystemOptions").SetActive(false);
        }

        safeDistanceFromCamera = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x * 4f;

        // Avoiding transparent area between the two options menus when lerping:
        Transform emptyColorFillTransform = transform.Find("EmptyColorFill");
        emptyColorFillTransform.position = new Vector3(safeDistanceFromCamera / 2, emptyColorFillTransform.position.y, emptyColorFillTransform.position.z);

        // Store and initialize Advanced Options
        moreOptionsTransform = transform.Find("MoreOptions");
        moreOptionsTransform.position = new Vector3(safeDistanceFromCamera, moreOptionsTransform.position.y, moreOptionsTransform.position.z);

        // Get the camera transform
        if (imInGameScene)
        {
            gameCameraTransform = GameObject.Find("Camera/CameraRiser/Main Camera").transform;
            GameObject.Find("________________Canvas________________").GetComponent<HudController>().PretendsToBePaused = true;
        }
        else if (!notOtherSceneActive) gameCameraTransform = GameObject.Find("Camera/Main Camera").transform;
        else gameCameraTransform = GameObject.Find("OptionsCamera/Main Camera").transform;

        // Initialize the canvas to adapt it to the camera and get standard camera's rotation to substract it from the actual camera rotation when it changes.
        GameObject.Find("CanvasOptions").GetComponent<Canvas>().worldCamera = gameCameraTransform.GetComponent<Camera>();
        realCameraRotation = gameCameraTransform.eulerAngles.z;

        // Store the animator of the camera to be able to send the animation activation signal.
        if (imInGameScene) cameraAnim = GameObject.Find("Camera").GetComponent<Animator>();
        else cameraAnim = gameCameraTransform.parent.GetComponent<Animator>();

        // Store Sounds for entering and exitting from the menu.
        menuEnterSound = GameObject.Find("UIsoundsOptions/MenuInSound").GetComponent<AudioSource>();
        menuExitSound = GameObject.Find("UIsoundsOptions/MenuOutSound").GetComponent<AudioSource>();

        if (imInGameScene)
        {
            hudScript = GameObject.Find("________________Canvas________________").GetComponent<HudController>();
            hudScript.SetIsInOptions = true;
        }
    }

    void LateUpdate()
    {
        LerpProgressAdjuster();
        PositionLerp();

        transform.localEulerAngles = new Vector3(0, 0, - realCameraRotation + gameCameraTransform.eulerAngles.z);

        if (Input.GetButtonDown("Pause"))
        {
            if (advancedOptionsEntering) ComeFromAdvancedOptions();
            else OptionsExit();
        }

        moreOptionsTransform.position = new Vector3(moreOptionsTransform.position.x, 0, moreOptionsTransform.position.z);
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
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
        }
    }

    // <-- Lerping Processes

    private void PositionLerp()
    {
        float ySafeDistanceFromCamera = Mathf.Tan(gameCameraTransform.eulerAngles.z * Mathf.Deg2Rad) * safeDistanceFromCamera;

        transform.position = new Vector3(Mathf.Lerp(safeDistanceFromCamera, -safeDistanceFromCamera, lerpProgress), Mathf.Lerp(ySafeDistanceFromCamera, -ySafeDistanceFromCamera, lerpProgress), transform.position.z);

        // For some rason, advanced options changes its position, so it's repositioned:
        moreOptionsTransform.position = new Vector3(transform.position.x + safeDistanceFromCamera, 0, moreOptionsTransform.position.z);
    }

    // Functs called from buttons -->

    public void GoToAdvancedOptions()
    {
        advancedOptionsEntering = true;
        CameraSlide(true);

        menuEnterSound.Play();
    }

    public void ComeFromAdvancedOptions()
    {
        advancedOptionsEntering = false;
        CameraSlide(false);

        menuExitSound.Play();
    }

    public void OptionsExit()
    {
        entering = false;
        if (imInGameScene) hudScript.SetIsInOptions = false;

        menuExitSound.Play();
    }

    private void CameraSlide(bool isRight)
    {
        if (isRight) cameraAnim.SetTrigger("SlideRight");
        else cameraAnim.SetTrigger("SlideLeft");
    }
}