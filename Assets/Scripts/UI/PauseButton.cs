using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButton : MonoBehaviour
{
    private HudController hudScript;
    private ScreenLoadAnim screenLoadScript;

    void Start()
    {
        screenLoadScript = GameObject.Find("Canvas/ScreenLoadUnload").GetComponent<ScreenLoadAnim>();
        hudScript = GameObject.Find("________________Canvas________________").GetComponent<HudController>();
    }

    public void ContinuePressed()
    {
        hudScript.Continue();
    }

    public void MenuPressed()
    {
        Time.timeScale = 1;
        hudScript.IsPaused = false;
        screenLoadScript.LoadScene("Menu", "Game");
        hudScript.SetChangingScene = true;
    }

    public void RetryPressed()
    {
        screenLoadScript.LoadScene("Game", "Game");
        hudScript.SetChangingScene = true;
    }

    public void OptionsPressed()
    {
        if (SceneManager.sceneCount > 1) SceneManager.UnloadSceneAsync("Options");
        SceneManager.LoadScene("Options", LoadSceneMode.Additive);
    }
}