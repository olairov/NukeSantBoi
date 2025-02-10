using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickController : MonoBehaviour
{
    private RectTransform backgroundTransform, handleTransform;
    private Transform cameraTransform;

    public Vector2 direction;
    private Vector2 backgroundOriginPos, touchPos;

    [SerializeField] private float distanceToMoveBackground;

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
        backgroundTransform = transform.GetChild(0).GetComponent<RectTransform>();
        handleTransform = backgroundTransform.GetChild(0).GetComponent<RectTransform>();
        cameraTransform = GameObject.Find("Camera/CameraRiser/Main Camera").transform;

        backgroundOriginPos = backgroundTransform.anchoredPosition;
    }

    void Update()
    {
        touchingJoystick = GetInputInformation(isPaused);
        BasicJoystickPositioning(touchingJoystick, Camera.main.ScreenToWorldPoint(touchPos), lastFrameTouched);

        lastFrameTouched = touchingJoystick;
    }

    // Getting the Position of the touch (if it enters in the joystick area) and if it is being touched or not --->

    bool GetInputInformation(bool isPaused)
    {
        if (isPaused || !canTouchJoystick) return false;

        touchPos = GetTouchPosition();

        return touchPos != Vector2.zero;
    }

    Vector2 GetTouchPosition()
    {
        int touchCount = Input.touchCount; // UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count;
        if (touchCount <= 0) return Vector2.zero;

        for (int touchIdx = touchCount - 1; touchIdx >= 0; touchIdx--)
        {
            if (Input.touches[touchIdx].position.x > Screen.width / 2)
                return Input.touches[touchIdx].position;
        }

        return Vector2.zero;
    }

    // <--- Getting the Position of the touch (if it enters in the joystick area) and if it is being touched or not

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
