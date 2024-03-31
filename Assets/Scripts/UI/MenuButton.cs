using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    private ScreenLoadAnim screenLoadScript;

    void Start()
    {
        screenLoadScript = GameObject.Find("Canvas/ScreenLoadUnloadMenu").GetComponent<ScreenLoadAnim>();
    }

    public void PlayPressed()
    {
        screenLoadScript.LoadScene("Game", "Menu");
    }

    public void ExitPressed()
    {
        screenLoadScript.LoadScene("Exit", "Menu");
    }

    public void OptionsPressed()
    {
        if (SceneManager.sceneCount > 1) SceneManager.UnloadSceneAsync("Options");
        SceneManager.LoadScene("Options", LoadSceneMode.Additive);
    }
}
