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

        MoveUISingleElement(transform.parent.Find("Charge").GetComponent<RectTransform>(), 0.18f, 0);
        MoveUISingleElement(transform.parent.Find("PointsCounter").GetComponent<RectTransform>(), 0.18f, 0);
        MoveUISingleElement(transform.parent.Find("MorePointsContainer").GetComponent<RectTransform>(), 0.18f, -40f);
    }

    void MoveUISingleElement(RectTransform element, float anchorX, float anchoredXpos)
    {
        element.anchorMin = new Vector2(anchorX, element.anchorMin.y);
        element.anchorMax = new Vector2(anchorX, element.anchorMax.y);
        element.anchoredPosition = new Vector2(anchoredXpos, element.anchoredPosition.y);
    }
}
