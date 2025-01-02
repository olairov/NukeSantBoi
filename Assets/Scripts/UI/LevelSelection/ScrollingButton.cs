using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingButton : MonoBehaviour
{
    Transform levelsTransform;
    CanvasGroup myCanvasGroup;
    Animator myAnimator;

    ScrollerController scrollerScript;

    [SerializeField] int direction;

    [SerializeField] float initialSpeed, startingSpeedAccelerationTime, scrollSpeed, speedAcceleration, timeMomentumLasts;
    float timeSinceButtonPressed, scrollingActualSpeed, leftEdgePosition, rightEdgePosition, startingSpeed; // StartingSpeed increments fastly only in the beggining
    
    bool pressed, locked;

    // Start is called before the first frame update
    void Start()
    {
        levelsTransform = GameObject.Find("CanvasLevelSelection/MainScreen/Levels").transform;
        scrollerScript = GameObject.Find("CanvasLevelSelection/MainScreen/Scroller").GetComponent<ScrollerController>();
        myCanvasGroup = GetComponent<CanvasGroup>();
        myAnimator = GetComponent<Animator>();

        if (IsOutOfBounds())
        {
            locked = true;
            myAnimator.Play("ButtonStartPos");
        }
        else myAnimator.Play("ButtonIn");

        leftEdgePosition = levelsTransform.position.x;
        rightEdgePosition = ButtonsScreenScroller.rightEdgePosition;
    }

    private void Update()
    {
        CheckEdge();
        if (!locked) Move();
    }

    void Move()
    {
        if (pressed) scrollingActualSpeed = GetScrollingSpeed();
        else if (scrollingActualSpeed != 0)
        {
            scrollingActualSpeed /= 1 + Time.deltaTime / timeMomentumLasts;
            if (Mathf.Abs(scrollingActualSpeed) < 0.1f) scrollingActualSpeed = 0;
        }

        levelsTransform.position += new Vector3(scrollingActualSpeed * Time.deltaTime, 0, 0);
    }

    void CheckEdge()
    {
        // The substraction and addition of 0.1 is to make sure it disappears a little before the edge, because it sometimes didn't disappear.
        if (IsOutOfBounds())
        {
            if (locked) return;

            if (scrollingActualSpeed != 0) scrollerScript.Momentum = scrollingActualSpeed;
            scrollerScript.UsingButtons = false;

            myAnimator.Play("ButtonOut");
            myCanvasGroup.blocksRaycasts = false;
            scrollingActualSpeed = 0;
            pressed = false;
            locked = true;
        }
        else if (locked)
        {
            myAnimator.Play("ButtonIn");
            myCanvasGroup.blocksRaycasts = true;
            locked = false;
        }
    }

    float GetScrollingSpeed()
    {
        timeSinceButtonPressed += Time.deltaTime;

        startingSpeed = timeSinceButtonPressed / startingSpeedAccelerationTime;
        if (startingSpeed > 1) startingSpeed = 1;

        float additionalAcceleration = Mathf.Pow(timeSinceButtonPressed, speedAcceleration);
        return (initialSpeed + additionalAcceleration) * direction * scrollSpeed * startingSpeed;
    }

    public void Clicked(bool value)
    {
        pressed = value;
        scrollerScript.UsingButtons = value;

        if (value)
        {
            timeSinceButtonPressed = 0;
            startingSpeed = 0;
        }
    }

    public void ExitScene()
    {
        if (!IsOutOfBounds()) myAnimator.Play("ButtonOut");
    }

    bool IsOutOfBounds ()
    {
        return direction > 0 && levelsTransform.position.x >= leftEdgePosition - 0.1f || direction < 0 && levelsTransform.position.x <= rightEdgePosition + 0.1f;
    }
}
