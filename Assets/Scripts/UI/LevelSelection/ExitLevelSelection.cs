using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevelSelection : MonoBehaviour
{
    public void Exit()
    {
        SceneManager.UnloadSceneAsync("LevelSelection");
    }
}
