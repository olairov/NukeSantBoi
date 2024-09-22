using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButton : MonoBehaviour
{
    private HudController hudScript;
    private ScreenLoadAnim screenLoadScript;

    private float timeForEnabled;
    private static int objectsWithThisScript = 3;

    void Start()
    {
        screenLoadScript = GameObject.Find("Canvas/ScreenLoadUnload").GetComponent<ScreenLoadAnim>();
        hudScript = GameObject.Find("________________Canvas________________").GetComponent<HudController>();
    }

    private void Update()
    {
        if (timeForEnabled > 0) timeForEnabled -= Time.unscaledDeltaTime / objectsWithThisScript;
    }

    public void ContinuePressed()
    {
        hudScript.Continue();
    }

    public void MenuPressed()
    {
        if (timeForEnabled > 0) return;

        timeForEnabled = 1f;
        Time.timeScale = 1;
        hudScript.IsPaused = false;
        screenLoadScript.LoadScene("Menu", "Game");
        hudScript.SetChangingScene = true;
    }

    public void RetryPressed()
    {
        if (timeForEnabled > 0) return;

        timeForEnabled = 1f;
        screenLoadScript.LoadScene("Game", "Game");
        hudScript.SetChangingScene = true;
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

    public void QuizPressed()
    {
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLScLkIE7z1QZ3Eamu1pyRzbTftLfLW580QghEMYpYtIbu9KDVA/viewform?usp=sf_link");
    }

    public void QuizOutPressed()
    {
        transform.parent.parent.GetComponent<Animator>().SetBool("MenuIn", false);
    }
}