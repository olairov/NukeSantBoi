using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsScreenScroller : MonoBehaviour
{
    private RectTransform myRectTransform;

    [SerializeField] float firstLevelPos, distanceBetweenLevels;
    // Done this way because multiple other scripts need those values, and they're hard to calculate on each script.
    static public float leftEdgePosition, rightEdgePosition;
    public float size;

    void Awake() // Done in the awake because other objects' start need to know the size or origin position of this object.
    {
        myRectTransform = GetComponent<RectTransform>();

        // This part just adjusts the general size of the level buttons so that they occupy all the height of the screen once the real screen's resolution
        // is lower than the X reference resolution of the canvas.

        Vector2 referenceResolution = GameObject.Find("CanvasLevelSelection").GetComponent<CanvasScaler>().referenceResolution;

        size = 1;
        float screenProportion = (float)Screen.width / Screen.height;
        if (screenProportion < referenceResolution.x / referenceResolution.y)
        {
            size = ChangeXSize(screenProportion);
            myRectTransform.localScale = Vector2.one * size;
            transform.position = new Vector3(UpdatePos(), transform.position.y, transform.position.z);
        }

        SetLevelsPositions();

        leftEdgePosition = transform.position.x;
        rightEdgePosition = GetRightEdgePosition();

        if (PlayerPrefs.HasKey("LevelSelectionScrollInitialPosition")) 
            transform.position = new Vector3(PlayerPrefs.GetFloat("LevelSelectionScrollInitialPosition"), transform.position.y, transform.position.z);
    }

    float ChangeXSize(float proportion)
    {
        float chosenSize = (-proportion + 3) * 0.9f;
        if (chosenSize < 1) chosenSize = 1;

        return chosenSize;
        // Apparently, for my uncomprehensible mind this is the easiest way of doing: If proportion is 2, size is 1, and if proportion is 1, size is 2, and so on.
    }

    float UpdatePos()
    {
        float distanceBetweenLevels = transform.GetChild(1).position.x - transform.GetChild(0).position.x;
        float firstLevelDistanceFromZero = transform.GetChild(0).position.x - Camera.main.ScreenToWorldPoint(Vector3.zero).x;

        return transform.position.x + (distanceBetweenLevels - firstLevelDistanceFromZero);
    }

    void SetLevelsPositions()
    {
        Vector2 referenceResolution = GameObject.Find("CanvasLevelSelection").GetComponent<CanvasScaler>().referenceResolution;
        firstLevelPos += (Screen.width - Screen.height * (int)(referenceResolution.x / referenceResolution.y)) / 7;
        
        for (int idx = 0; idx < transform.childCount; idx++)
            transform.GetChild(idx).GetComponent<RectTransform>().anchoredPosition = new Vector2(firstLevelPos + distanceBetweenLevels * idx, 0);
    }

    float GetRightEdgePosition()
    {
        float distanceBetweenLevels = transform.GetChild(1).position.x - transform.GetChild(0).position.x;

        // The position of the right / left endings of the levels object rectangle
        float rightEdgeOfLevels = transform.GetChild(transform.childCount - 1).position.x + distanceBetweenLevels;
        float leftEdgeOfLevels = transform.GetChild(0).position.x - distanceBetweenLevels;

        // I substract it, because leftEdfeOfLevels is supposed to be always negative. The last substraction is because the edge was too far away.
        float levelsLength = rightEdgeOfLevels - leftEdgeOfLevels;
        float screenWidthInWorldUnits = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x * 2; // The ZERO point is always in the middle.
        
        return transform.position.x - (levelsLength - screenWidthInWorldUnits);
    }

    public void SetScrollInitialPosition()
    {
        PlayerPrefs.SetFloat("LevelSelectionScrollInitialPosition", transform.position.x);
    }
}
