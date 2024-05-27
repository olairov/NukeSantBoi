using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitShop : MonoBehaviour
{
    public void ExitPressed()
    {
        GameObject.Find("CanvasShop/ShopLoaderUnloader").GetComponent<Animator>().SetTrigger("Exit");
    }
}
