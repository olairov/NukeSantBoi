using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickController : MonoBehaviour
{
    private RectTransform backgroundTransform, handleTransform, myTransform;
    private Transform cameraTransform;

    public Vector2 direction;
    private Vector2 backgroundOriginPos, touchPos, originalHandlePos;

    [SerializeField] private float distanceToMoveBackground;

    private int lastTouchCount;

    public bool touchingJoystick;
    private bool lastFrameTouched, isPaused, canTouchJoystick;
    public bool SetIsPaused
    {
        set { isPaused = value; }
    }
    public bool SetCanTouchJoystick
    {
        set { canTouchJoystick = value; }
    }

    void Start()
    {
        myTransform = GetComponent<RectTransform>();
        backgroundTransform = transform.GetChild(0).GetComponent<RectTransform>();
        handleTransform = backgroundTransform.GetChild(0).GetComponent<RectTransform>();
        cameraTransform = GameObject.Find("Camera/CameraRiser/Main Camera").transform;

        originalHandlePos = handleTransform.position;
        backgroundOriginPos = backgroundTransform.anchoredPosition;
    }

    void Update()
    {
        touchingJoystick = GetInputInformation(isPaused);
        BasicJoystickPositioning(touchingJoystick, Camera.main.ScreenToWorldPoint(touchPos), lastFrameTouched);

        lastTouchCount = Input.touchCount;
        lastFrameTouched = touchingJoystick;
    }

    // Getting Basic Input Info --->

    bool GetInputInformation(bool isPaused)
    {
        if (isPaused || !canTouchJoystick) return false;

        if (touchingJoystick)
        {
            touchPos = GetTouchPosition(false);
        }
        else
        {
            touchPos = GetTouchPosition(true);
        }

        return touchPos != Vector2.zero;
    }

    Vector2 GetTouchPosition(bool firstTouch)
    {
        if (!firstTouch)
        {
            if (Input.touchCount <= 0) return Vector2.zero;

            for (int touchIdx = Input.touchCount - 1; touchIdx >= 0; touchIdx--)
            {
                if (Input.touches[touchIdx].position.x > Screen.width / 2) return Input.touches[touchIdx].position;
            }
        }
        else
        {
            if (Input.touchCount <= 0 || Input.touchCount <= lastTouchCount) return Vector2.zero;

            if (Input.touches[Input.touchCount - 1].position.x > Screen.width / 2) return Input.touches[Input.touchCount - 1].position;
        }

        return Vector2.zero;
    }

    // <--- Getting Basic Input Info

    // Managing Joystick Movement & Position depending on the touch position and whether it's pressed or not --->

    void BasicJoystickPositioning(bool touched, Vector2 touchPos, bool wasTouchedBefore)
    {
        if (!touched)
        {
            backgroundTransform.anchoredPosition = backgroundOriginPos;
            handleTransform.anchoredPosition = Vector2.zero;
        }
        else
        {
            if (!wasTouchedBefore) backgroundTransform.position = touchPos;
            else FollowFinger(touchPos);
        }

        backgroundTransform.position = new Vector3(backgroundTransform.position.x, backgroundTransform.position.y, 0);
        handleTransform.position = new Vector3(handleTransform.position.x, handleTransform.position.y, 0);

        if (touched) direction = handleTransform.position - backgroundTransform.position;
    }

    void FollowFinger(Vector2 touchPos)
    {
        handleTransform.position = touchPos;

        Vector2 directionToHandle = handleTransform.position - backgroundTransform.position;
        if (directionToHandle.magnitude > distanceToMoveBackground) DisplaceBackgroundTowardsFinger(touchPos, directionToHandle);
    }

    void DisplaceBackgroundTowardsFinger(Vector2 touchPos, Vector2 direction)
    {
        Vector3 amountToDisplace = direction - direction.normalized * distanceToMoveBackground;
        backgroundTransform.position += amountToDisplace;

        handleTransform.position = touchPos;
    }
}
