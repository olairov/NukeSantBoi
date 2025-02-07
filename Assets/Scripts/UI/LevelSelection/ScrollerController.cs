using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollerController : MonoBehaviour
{
    private Transform levelsTransform;

    [SerializeField] private float speedLoss, timeBounceLasts;
    private float xTouchStartPos, lastFrameXTouchPos, lastLastFrameXTouchPos, momentum, leftEdgePosition, rightEdgePosition,
        startBouncePos, timeSinceBounceStart, scrollingHarshness;
    public float Momentum
    {
        set { momentum = value; bouncing = false; }
    }

    private bool scrollingPressed, lvlButtonCanBeClicked = true, surpassingLeftEdge, surpassingRightEdge, bouncing, usingButtons, exittingScene;
    public bool UsingButtons
    {
        set { usingButtons = value; }
    }
    public bool ExittingScene
    {
        set { exittingScene = value; }
    }

    private int actualTouchID = -1;

    void Start()
    {
        levelsTransform = GameObject.Find("CanvasLevelSelection/MainScreen/Levels").transform;

        // The position LevelsTransform has to be to be considered surpassing the left / right edge
        leftEdgePosition = ButtonsScreenScroller.leftEdgePosition;
        rightEdgePosition = ButtonsScreenScroller.rightEdgePosition;
    }

    void LateUpdate()
    {
        surpassingLeftEdge = levelsTransform.position.x > leftEdgePosition;

        surpassingRightEdge = levelsTransform.position.x < rightEdgePosition;

        if (usingButtons || exittingScene) return;

        if (surpassingLeftEdge != surpassingRightEdge) OutOfBoundsProcess();
        Move();
    }

    void Move()
    {
        if (scrollingPressed && momentum == 0)
        {
            ScrollingProcess(xTouchStartPos, GetTouchPos(), lastFrameXTouchPos);
            lastLastFrameXTouchPos = lastFrameXTouchPos;
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
        if (!scrollingPressed)
        {
            if (momentum == 0) BounceBackToEdge();
            return;
        }

        // In case the touch continues when surpassing an edge, the speed of the scroll will decrement more the further away you are from the limit.
        // If the player starts scrolling away from the edge, the speed of the scroll will be as usual.

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

        if (TouchControllersManager.isUsingPhone)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began) actualTouchID = touch.fingerId;
            }

            xTouchStartPos = GetTouchPos();
        }
        else
        {
            xTouchStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        }

        lastFrameXTouchPos = GetTouchPos();
        lastLastFrameXTouchPos = lastFrameXTouchPos;
    }

    public void PointerUp()
    {
        lvlButtonCanBeClicked = true;

        if (!scrollingPressed) return;
        scrollingPressed = false;

        // For some reason I absolutely ignore, when in phone, lastFrameXTouchPos is equal to GetTouchPos(), but not in PC. The weird conditional fixes that.
        momentum = (GetTouchPos() - (TouchControllersManager.isUsingPhone ? lastLastFrameXTouchPos : lastFrameXTouchPos)) * (1 - scrollingHarshness) / Time.deltaTime;
        actualTouchID = -1;
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
        if (TouchControllersManager.isUsingPhone)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.fingerId == actualTouchID) return Camera.main.ScreenToWorldPoint(touch.position).x;
            }

            Debug.LogWarning("No touch found with ID " + actualTouchID);
            return 0; // In case the Foreach doesn't find any matching touch (It shouldn't happen).
        }
        else return Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
    }
}
