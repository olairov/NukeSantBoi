using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectInputs : MonoBehaviour
{
    [SerializeField] private float lerpSpeed;

    static public float verticalAxis;
    static public int rawVerticalAxis;
    static public bool justPressedVertical, justReleasedVertical;

    private bool upButtonMobilePressed, downButtonMobilePressed, lastFramePressed;

    void Update()
    {
        CalculateVerticalAxis();
    }

    private void LateUpdate()
    {
        if (rawVerticalAxis == 0) lastFramePressed = false;
        else lastFramePressed = true;
    }

    void CalculateVerticalAxis()
    {
        justPressedVertical = false;
        justReleasedVertical = false;

        if (!upButtonMobilePressed && !downButtonMobilePressed)
        {
            rawVerticalAxis = (int)Input.GetAxisRaw("Horizontal");
            if (!lastFramePressed && Input.GetAxisRaw("Horizontal") != 0) justPressedVertical = true;
            if (lastFramePressed && Input.GetAxisRaw("Horizontal") == 0) justReleasedVertical = true;
        }

        // From now on, what it's doing is a smooth lerp between -1, 0 & 1 depending on the player's input.

        float lastVerticalAxis = verticalAxis;
        if (verticalAxis < rawVerticalAxis) verticalAxis += lerpSpeed * Time.deltaTime;
        if (verticalAxis > rawVerticalAxis) verticalAxis -= lerpSpeed * Time.deltaTime;

        if (rawVerticalAxis == 0 && (lastVerticalAxis < 0 && verticalAxis > 0 || lastVerticalAxis > 0 && verticalAxis < 0)) verticalAxis = 0;

        if (verticalAxis < -1) verticalAxis = -1;
        if (verticalAxis > 1) verticalAxis = 1;
    }

    public void UpButtonMobile(bool pressed)
    {
        upButtonMobilePressed = pressed;

        if (pressed)
        {
            rawVerticalAxis = -1;
            if (!lastFramePressed) justPressedVertical = true;
        }
        else
        {
            if (downButtonMobilePressed) rawVerticalAxis = 1;
            else rawVerticalAxis = (int)Input.GetAxisRaw("Horizontal");

            if (lastFramePressed) justReleasedVertical = true;
        }
    }

    public void DownButtonMobile(bool pressed)
    {
        downButtonMobilePressed = pressed;

        if (pressed)
        {
            rawVerticalAxis = 1;
            if (!lastFramePressed) justPressedVertical = true;
        }
        else
        {
            if (upButtonMobilePressed) rawVerticalAxis = -1;
            else rawVerticalAxis = (int)Input.GetAxisRaw("Horizontal");

            if (lastFramePressed) justReleasedVertical = true;
        }
    }
}
