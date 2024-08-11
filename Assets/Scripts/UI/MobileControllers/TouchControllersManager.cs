using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControllersManager : MonoBehaviour
{
    static public bool isUsingPhone;

    private GameObject controllersGO;

    void Start()
    {
        controllersGO = transform.GetChild(0).gameObject;

        if (!isUsingPhone) controllersGO.SetActive(false);
        else EnableTouchControls();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && !isUsingPhone)
        {
            EnableTouchControls();
        }

        if (PlayerController.dead) controllersGO.SetActive(false);
    }

    void EnableTouchControls()
    {
        controllersGO.SetActive(true);
        isUsingPhone = true;

        transform.parent.Find("Charge").GetComponent<RectTransform>().anchoredPosition = new Vector2(230, 120);
        transform.parent.Find("PointsCounter").GetComponent<RectTransform>().anchoredPosition = new Vector2(230, -80);
        transform.parent.Find("MorePointsContainer").GetComponent<RectTransform>().anchoredPosition = new Vector2(200, -55);
    }
}
