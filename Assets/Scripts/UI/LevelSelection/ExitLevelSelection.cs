using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevelSelection : MonoBehaviour
{
    bool lastAnyInfoPressedState;

    private void Update()
    {
        if (Input.GetButtonDown("Pause") && !lastAnyInfoPressedState && !LevelButtonResizer.anyInfoPressed) GetComponent<Animator>().SetTrigger("exit");

        lastAnyInfoPressedState = LevelButtonResizer.anyInfoPressed; // Wait one frame so that the LevelButtonResizer has time to set anyInfoPressed to false
    }

    public void Exit()
    {
        SceneManager.UnloadSceneAsync("LevelSelection");
    }

    public void PlayMenuInSound()
    {
        GameObject.Find("UIsounds/MenuInSound").GetComponent<AudioSource>().Play();
    }

    public void PlayMenuOutSound()
    {
       GameObject.Find("UIsounds/MenuOutSound").GetComponent<AudioSource>().Play();
    }

    public void SetLevelButtonsLayerToDefault()
    {
        LevelButtonResizer.anyInfoPressedAlready = false;
    }
}
