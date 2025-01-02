using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollerController : MonoBehaviour
{
    private Transform levelsTransform;

    [SerializeField] private float speedLoss, timeBounceLasts;
    private float xTouchStartPos, lastFrameXTouchPos, momentum, leftEdgePosition, rightEdgePosition,
        startBouncePos, timeSinceBounceStart, scrollingHarshness;
    public float Momentum
    {
        set { momentum = value; bouncing = false; }
    }

    private bool scrollingPressed, lvlButtonCanBeClicked = true, surpassingLeftEdge, surpassingRightEdge, bouncing, usingButtons;
    public bool UsingButtons
    {
        set { usingButtons = value; }
    }

    private UnityEngine.InputSystem.EnhancedTouch.Touch actualTouch;

    void Start()
    {
        levelsTransform = GameObject.Find("CanvasLevelSelection/MainScreen/Levels").transform;

        leftEdgePosition = levelsTransform.position.x; // The position LevelsTransform has to be to be considered surpassing the left edge (the one by default)
        rightEdgePosition = ButtonsScreenScroller.rightEdgePosition;
    }

    void LateUpdate()
    {
        surpassingLeftEdge = levelsTransform.position.x > leftEdgePosition;

        surpassingRightEdge = levelsTransform.position.x < rightEdgePosition;

        if (usingButtons) return;

        if (surpassingLeftEdge != surpassingRightEdge) OutOfBoundsProcess();
        Move();
    }

    void Move()
    {
        if (scrollingPressed && momentum == 0)
        {
            ScrollingProcess(xTouchStartPos, GetTouchPos(), lastFrameXTouchPos);
            lastFrameXTouchPos = GetTouchPos();
        }
        else
        {
            momentum /= 1 + Time.deltaTime * (surpassingLeftEdge || surpassingRightEdge ? speedLoss * 4 : speedLoss);
            if (momentum > 0 && momentum < 0.5f || momentum > -0.5f && momentum < 0) momentum = 0;
            levelsTransform.position += new Vector3(momentum * Time.deltaTime, 0, 0);
        }
    }

    void OutOfBoundsProcess()
    {
        if (momentum == 0 && !scrollingPressed) BounceBackToEdge();

        if (surpassingLeftEdge)
        {
            if (lastFrameXTouchPos < GetTouchPos()) scrollingHarshness = Mathf.Clamp01((levelsTransform.position.x - leftEdgePosition) / 10);
            else scrollingHarshness = 0;
        }

        if (surpassingRightEdge)
        {
            if (lastFrameXTouchPos > GetTouchPos()) scrollingHarshness = Mathf.Clamp01((rightEdgePosition - levelsTransform.position.x) / 10);
            else scrollingHarshness = 0;
        }
    }

    public void PointerDown()
    {
        if (LevelButtonResizer.anyInfoPressed) return;

        scrollingPressed = true;
        bouncing = false;
        momentum = 0;
        lastFrameXTouchPos = GetTouchPos();

        if (TouchControllersManager.isUsingPhone)
        {
            actualTouch = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count - 1];
            xTouchStartPos = Camera.main.ScreenToWorldPoint(actualTouch.screenPosition).x;
        }
        else
        {
            xTouchStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        }
    }

    public void PointerUp()
    {
        lvlButtonCanBeClicked = true;

        if (!scrollingPressed) return;
        scrollingPressed = false;
        momentum = (GetTouchPos() - lastFrameXTouchPos) * (1 - scrollingHarshness) / Time.deltaTime;
    }

    private void ScrollingProcess(float xOriginalTouchPos, float xActualTouchPos, float xLastTouchPos)
    {
        float displacement = (xActualTouchPos - xLastTouchPos) * (1 - scrollingHarshness);

        levelsTransform.position += new Vector3(displacement, 0, 0);

        if (lvlButtonCanBeClicked && Mathf.Abs(xOriginalTouchPos - xActualTouchPos) > 0.5f)
        {
            lvlButtonCanBeClicked = false;

            for (int idx = 0; idx < levelsTransform.childCount; idx++)
            {
                levelsTransform.GetChild(idx).GetChild(0).GetComponent<LevelButton>().ClickedDown(false);
                levelsTransform.GetChild(idx).GetChild(1).GetComponent<LevelInfoController>().infoClikedDown = false;
            }
        }
    }

    private void BounceBackToEdge()
    {
        if (!bouncing)
        {
            startBouncePos = levelsTransform.position.x;

            timeSinceBounceStart = 0;
            bouncing = true;
        }

        if (timeSinceBounceStart < timeBounceLasts)
        {
            timeSinceBounceStart += Time.deltaTime;
            timeSinceBounceStart = Mathf.Clamp01(timeSinceBounceStart);
        }
        else bouncing = false;

        // Put this equation in geogebra.org/classic and you will understand how it smooths the lerp:
        float smoothedLerp = Mathf.Cos(timeSinceBounceStart / timeBounceLasts * Mathf.PI + Mathf.PI) / 2 + 0.5f;
        levelsTransform.position = new Vector3(Mathf.Lerp(startBouncePos, surpassingLeftEdge ? leftEdgePosition : rightEdgePosition, smoothedLerp),
            levelsTransform.position.y, levelsTransform.position.z);
    }

    private float GetTouchPos()
    {
        return TouchControllersManager.isUsingPhone ? Camera.main.ScreenToWorldPoint(actualTouch.screenPosition).x : Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
    }
}
