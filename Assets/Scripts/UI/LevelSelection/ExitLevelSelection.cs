using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevelSelection : MonoBehaviour
{
    bool lastAnyInfoPressedState;

    float timeInScene = 0;

    private void Update()
    {
        timeInScene += Time.unscaledDeltaTime;
        if (Input.GetButtonDown("Pause") && !lastAnyInfoPressedState && !LevelButtonResizer.anyInfoPressed && timeInScene > 0.4f) StartExittingScene();

        lastAnyInfoPressedState = LevelButtonResizer.anyInfoPressed; // Wait one frame so that the LevelButtonResizer has time to set anyInfoPressed to false
    }

    public void StartExittingScene()
    {
        GetComponent<Animator>().SetTrigger("exit");

        for (int idx = 0; idx < transform.Find("Levels").childCount; idx++)
        {
            transform.Find("Levels").GetChild(idx).GetComponent<LevelButtonResizer>().ExitScene();
        }

        transform.Find("LeftButton").GetComponent<ScrollingButton>().ExitScene();
        transform.Find("RightButton").GetComponent<ScrollingButton>().ExitScene();

        transform.Find("Levels").GetComponent<ButtonsScreenScroller>().SetScrollInitialPosition();
        transform.Find("Scroller").GetComponent<ScrollerController>().ExittingScene = true;
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
