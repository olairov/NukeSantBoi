using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenLoadAnim : MonoBehaviour
{
    private HudController hudScript;
    private SceneLoadGatePart gatePartLeft, gatePartRight;

    private Transform cameraTransform;

    private float safeDistanceFromCamera, timeSinceLevelLoad, timeSinceStartOfSceneQuit = 1;

    static private string sceneToLoad, originScene;

    [SerializeField] private bool isFromMenu;
    private bool alreadyCalledLoader, alreadyFinishedEntering, justStarted = true, allowedToExitFromScene;

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

        //if (sceneToLoad != null) StartCoroutine(StartOfLoadingScene());
        //else allowedToExitFromScene = true;
        allowedToExitFromScene = true;
    }

    private void Update()
    {
        if (timeSinceLevelLoad < 1 && timeSinceStartOfSceneQuit >= 1 && allowedToExitFromScene) ExitFromScene(); // When the scene just started, it exits from it.

        if (timeSinceStartOfSceneQuit < 1 && !alreadyFinishedEntering) EnterOnScene(); // When the scene is preparing to change, it enters on it.
    }

    void ExitFromScene()
    {
        if (sceneToLoad == "Game" && originScene == "Menu" && justStarted)
        {
            timeSinceLevelLoad -= 0.7f;
            justStarted = false;
        }

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
        else if (sceneToLoad == "Options" && originScene != "Options") SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
        else if (sceneToLoad == "Options" && originScene == "Options")
        {
            GameObject.Find("Canvas/ScreenLoadUnload").GetComponent<ScreenLoadAnim>().RestartStats();
            SceneManager.UnloadSceneAsync("Options");

            // Options from options means that from options, we want to exit that scene, and as anotherone might be enabled, I just disable options.
        }
        else
        {
            StartCoroutine(StartOfLoadingScene());
            allowedToExitFromScene = false;
        }
    }

    IEnumerator StartOfLoadingScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            // You can update a loading bar or do other things here if needed

            if (asyncLoad.progress >= 0.9f)
            {
                // The scene is almost ready
                asyncLoad.allowSceneActivation = true;  // Activate the scene
                //allowedToExitFromScene = true;
                SceneManager.UnloadSceneAsync(originScene);
            }

            yield return null;
        }

        /*while (true)
        {
            if (asyncLoad.isDone)
            {
                asyncLoad.allowSceneActivation = true;
                allowedToExitFromScene = true;
                SceneManager.UnloadSceneAsync(originScene);

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
