using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenLoadAnim : MonoBehaviour
{
    private HudController hudScript;
    private SceneLoadGatePart gatePartLeft, gatePartRight;

    private Transform cameraTransform;

    private float safeDistanceFromCamera, timeSinceLevelLoad = -0.3f, timeSinceStartOfSceneQuit = 1;

    static private string sceneToLoad, originScene;

    [SerializeField] private bool isFromMenu;
    private bool alreadyCalledLoader, alreadyFinishedEntering, allowedToExitFromScene;

    void Start()
    {
        gatePartLeft = transform.Find("LeftGate").GetComponent<SceneLoadGatePart>();
        gatePartRight = transform.Find("RightGate").GetComponent<SceneLoadGatePart>();

        if (!isFromMenu)
        {
            hudScript = GameObject.Find("________________Canvas________________").GetComponent<HudController>();
            cameraTransform = cameraTransform = GameObject.Find("Camera").transform.GetChild(0);
        }
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        allowedToExitFromScene = true;
    }

    private void Update()
    {
        if (timeSinceLevelLoad < 1 && timeSinceStartOfSceneQuit >= 1 && allowedToExitFromScene) ExitFromScene(); // When the scene just started, it exits from it.

        if (timeSinceStartOfSceneQuit < 1 && !alreadyFinishedEntering) EnterOnScene(); // When the scene is preparing to change, it enters on it.
    }

    void ExitFromScene()
    {
        timeSinceLevelLoad += Time.unscaledDeltaTime * 5;

        if (timeSinceLevelLoad < 0) return;

        float positionLerp = Mathf.Pow(timeSinceLevelLoad, 2);
        float yDifferenceFromCamera = 0;
        if (!isFromMenu) yDifferenceFromCamera = Mathf.Tan(cameraTransform.eulerAngles.z * Mathf.Deg2Rad);

        gatePartLeft.Move(positionLerp, yDifferenceFromCamera);
        gatePartRight.Move(positionLerp, yDifferenceFromCamera);
    }

    void EnterOnScene()
    {
        timeSinceStartOfSceneQuit -= Time.unscaledDeltaTime * 3;

        float positionLerp = Mathf.Pow(timeSinceStartOfSceneQuit, 2) - 0.01f;
        float yDifferenceFromCamera = 0;
        if (!isFromMenu) yDifferenceFromCamera = Mathf.Tan(cameraTransform.eulerAngles.z * Mathf.Deg2Rad);

        gatePartLeft.Move(positionLerp, yDifferenceFromCamera);
        gatePartRight.Move(positionLerp, yDifferenceFromCamera);

        if (timeSinceStartOfSceneQuit < 0.5f && !alreadyCalledLoader)
        {
            alreadyCalledLoader = true;
            StartCoroutine(EffectsAndLoadScene());
        }

        if (timeSinceStartOfSceneQuit < 0)
        {
            alreadyFinishedEntering = true;
            transform.position = new Vector3(0, 0, transform.position.z);

            gatePartLeft.PosZero();
            gatePartRight.PosZero();
        }
    }

    IEnumerator EffectsAndLoadScene()
    {
        transform.GetComponent<AudioSource>().Play();
        transform.GetComponent<ShakeController>().Shake();
        transform.Find("RightGate/Logo").GetComponent<Animator>().SetTrigger("Glow");

        yield return new WaitForSecondsRealtime(1.5f);

        RestartStats();

        if (sceneToLoad == "Exit") Application.Quit();
        else
        {
            StartCoroutine(StartOfLoadingScene());
            allowedToExitFromScene = false;
        }
    }

    IEnumerator StartOfLoadingScene() // Make sure the animation doesn't start until the scene has fully loaded.
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad); // Start scene loading
        asyncLoad.allowSceneActivation = false; // Prevent the scene from being activated until loaded

        while (!asyncLoad.isDone)
        {
            // Maybe put a Loading Bar

            if (asyncLoad.progress >= 0.9f) // The scene is almost ready
            {
                asyncLoad.allowSceneActivation = true;  // Activate the scene
            }

            yield return null;
        }

        /*while (true)
        {
            // Maybe put a Loading Bar

            if (asyncLoad.isDone)
            {
                asyncLoad.allowSceneActivation = true;  // Activate the scene
                break;
            }

            yield return null;
        }*/
    }

    public void LoadScene(string scene, string sceneFromCalled)
    {
        timeSinceStartOfSceneQuit -= Time.unscaledDeltaTime;
        transform.position = new Vector3(safeDistanceFromCamera, transform.position.y, transform.position.z);
        sceneToLoad = scene;
        originScene = sceneFromCalled;
    }

    public void RestartStats()
    {
        timeSinceStartOfSceneQuit = 1;
        timeSinceLevelLoad = 0;
        alreadyCalledLoader = false;
        alreadyFinishedEntering = false;
        if (!isFromMenu)
        {
            hudScript.SetChangingScene = false;
        }
    }
}
