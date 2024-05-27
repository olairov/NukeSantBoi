using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ShopLoader : MonoBehaviour
{
    private GameObject ShopObj;

    // Start is called before the first frame update
    void Start()
    {
        ShopObj = transform.parent.Find("MainScreen").gameObject;
        ShopObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableShop()
    {
        ShopObj.SetActive(true);
    }

    public void DisableShop()
    {
        ShopObj.SetActive(false);
    }

    public void ExitShopScene()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));
    }
}
