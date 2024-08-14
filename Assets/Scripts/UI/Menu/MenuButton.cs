using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    private ScreenLoadAnim screenLoadScript;

    private static float timeForEnabled;
    private static int objectsWithThisScript = 4;

    private bool inMenuScene;

    void Start()
    {
        inMenuScene = GameObject.Find("Canvas");

        if (inMenuScene) screenLoadScript = GameObject.Find("Canvas/ScreenLoadUnloadMenu").GetComponent<ScreenLoadAnim>();
    }

    private void Update()
    {
        if (timeForEnabled > 0) timeForEnabled -= Time.unscaledDeltaTime / objectsWithThisScript;
    }

    public void PlayPressed()
    {
        if (timeForEnabled > 0 || !inMenuScene) return;

        timeForEnabled = 1f;
        screenLoadScript.LoadScene("Game", "Menu");
    }

    public void ExitPressed()
    {
        if (timeForEnabled > 0) return;

        timeForEnabled = 1f;
        screenLoadScript.LoadScene("Exit", "Menu");
    }

    public void OptionsPressed()
    {
        if (timeForEnabled > 0) return;

        timeForEnabled = 0.1f;
        if (SceneManager.sceneCount > 1) SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
        SceneManager.LoadScene("Options", LoadSceneMode.Additive);
    }

    public void ShopPressed()
    {
        if (timeForEnabled > 0) return;

        timeForEnabled = 1f;
        if (SceneManager.sceneCount > 1) SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
        SceneManager.LoadScene("Shop", LoadSceneMode.Additive);
    }

    public void LevelSelectionPressed()
    {
        if (timeForEnabled > 0) return;

        timeForEnabled = 0.1f;
        if (SceneManager.sceneCount > 1) SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
        SceneManager.LoadScene("LevelSelection", LoadSceneMode.Additive);
    }
}
