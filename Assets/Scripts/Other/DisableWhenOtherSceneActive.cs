using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisableWhenOtherSceneActive : MonoBehaviour
{
    private void Awake()
    {
        if (SceneManager.sceneCount > 1) Destroy(gameObject);
    }
}
