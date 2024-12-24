using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsScreenScroller : MonoBehaviour
{
    private RectTransform myRectTransform;

    public float size;

    void Awake()
    {
        myRectTransform = transform.GetComponent<RectTransform>();

        // This part just adjusts the general size of the level buttons so that they occupy all the height of the screen once the real screen's resolution
        // is lower than the X reference resolution of the canvas.

        size = 1;
        float screenProportion = (float)Screen.width / Screen.height;
        if (screenProportion < 2)
        {
            size = ChangeXSize(screenProportion);
            myRectTransform.localScale = Vector2.one * size;
        }
    }

    float ChangeXSize(float proportion)
    {
        float chosenSize = (-proportion + 3) * 0.9f;
        if (chosenSize < 1) chosenSize = 1;

        return chosenSize;
        // Apparently, for my uncomprehensible mind this is the easiest way of doing: If proportion is 2, size is 1, and if proportion is 1, size is 2, and so on.
    }
}
