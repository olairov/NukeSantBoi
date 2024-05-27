using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainShopController : MonoBehaviour
{
    private bool imInGameScene, notOtherSceneActive = true;

    void Start()
    {
        if (SceneManager.sceneCount > 1)
        {
            GameObject.Find("ShopCamera").SetActive(false);
            if (GameObject.Find("Camera/CameraRiser/Main Camera")) imInGameScene = true;

            notOtherSceneActive = false;
            GameObject.Find("EventSystemShop").SetActive(false);
        }

        if (imInGameScene) transform.parent.GetComponent<Canvas>().worldCamera = GameObject.Find("Camera/CameraRiser/Main Camera").GetComponent<Camera>();
        else if (!notOtherSceneActive) transform.parent.GetComponent<Canvas>().worldCamera = GameObject.Find("Camera/Main Camera").GetComponent<Camera>();
    }

    void Update()
    {
        
    }
}
