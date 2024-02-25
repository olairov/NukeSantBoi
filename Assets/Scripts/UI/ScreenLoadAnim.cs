using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenLoadAnim : MonoBehaviour
{
    private HudController hudScript;

    private Transform cameraTransform;

    private float safeDistanceFromCamera, timeSinceLevelLoad = -0.7f, timeSinceStartOfSceneQuit = 1;

    private string sceneToLoad, originScene;

    [SerializeField] private bool isFromMenu;
    private bool alreadyCalledLoader, alreadyFinishedEntering;

    void Start()
    {
        safeDistanceFromCamera = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x * 2.5f;
        if (!isFromMenu)
        {
            hudScript = GameObject.Find("________________Canvas________________").GetComponent<HudController>();
            cameraTransform = cameraTransform = GameObject.Find("Camera").transform.GetChild(0);
        }
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    private void Update()
    {
        if (timeSinceLevelLoad < 1 && timeSinceStartOfSceneQuit >= 1) ExitFromScene();

        if (timeSinceStartOfSceneQuit < 1 && !alreadyFinishedEntering) EnterOnScene();
    }

    void ExitFromScene()
    {
        timeSinceLevelLoad += Time.unscaledDeltaTime * 5;

        if (timeSinceLevelLoad < 0) return;

        float positionLerp = Mathf.Pow(timeSinceLevelLoad, 2);
        float yDifferenceFromCamera = 0;
        if (!isFromMenu) yDifferenceFromCamera = Mathf.Tan(cameraTransform.eulerAngles.z * Mathf.Deg2Rad);

        transform.position = new Vector3(Mathf.Lerp(0, -safeDistanceFromCamera, positionLerp), Mathf.Lerp(0, -safeDistanceFromCamera * yDifferenceFromCamera, positionLerp), transform.position.z);
    }

    void EnterOnScene()
    {
        timeSinceStartOfSceneQuit -= Time.unscaledDeltaTime * 3;

        float positionLerp = Mathf.Pow(timeSinceStartOfSceneQuit, 2) - 0.015f;
        float yDifferenceFromCamera = 0;
        if (!isFromMenu) yDifferenceFromCamera = Mathf.Tan(cameraTransform.eulerAngles.z * Mathf.Deg2Rad);

        transform.position = new Vector3(Mathf.Lerp(0, safeDistanceFromCamera, positionLerp), Mathf.Lerp(0, safeDistanceFromCamera * yDifferenceFromCamera, positionLerp), transform.position.z);

        if (timeSinceStartOfSceneQuit < 0.5f && !alreadyCalledLoader)
        {
            alreadyCalledLoader = true;
            StartCoroutine(EffectsAndLoadScene());
        }

        if (timeSinceStartOfSceneQuit < 0)
        {
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
            alreadyFinishedEntering = true;
        }
    }

    IEnumerator EffectsAndLoadScene()
    {
        transform.GetComponent<AudioSource>().Play();
        transform.GetChild(0).GetComponent<ShakeController>().Shake();

        yield return new WaitForSecondsRealtime(1.5f);

        RestartStats();

        if (sceneToLoad == "Exit") Application.Quit();
        else if (sceneToLoad == "Options" && originScene != "Options") SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
        else if (sceneToLoad == "Options" && originScene == "Options")
        {
            GameObject.Find("Canvas/ScreenLoadUnload").GetComponent<ScreenLoadAnim>().RestartStats();
            SceneManager.UnloadSceneAsync("Options");

            // Options from options means that from options, we want to exit this scene, and as anotherone might be enabled, I just disable options.
        }
        else SceneManager.LoadScene(sceneToLoad);
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
