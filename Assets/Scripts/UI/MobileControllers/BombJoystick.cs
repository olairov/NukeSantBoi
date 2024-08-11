using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombJoystick : MonoBehaviour
{
    private JoystickController myJoystick;

    private Animator touchArrowAnim;
    private Transform arrowTransform;

    private bool arrowActivated, lastFrameTouched, validTouch;

    private PlayerController playerScript;

    void Start()
    {
        if (PlayerController.dead) return;

        arrowTransform = GameObject.Find("Player/TouchArrow").transform;
        touchArrowAnim = arrowTransform.GetComponent<Animator>();

        myJoystick = transform.GetComponent<JoystickController>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        if (PlayerController.dead) return;
        validTouch = myJoystick.direction.magnitude > 0.02f;

        CheckJoystick();
        TouchArrowManagement();

        lastFrameTouched = myJoystick.touchingJoystick;
    }

    void CheckJoystick()
    {
        if (!myJoystick.touchingJoystick && lastFrameTouched && validTouch)
            playerScript.DropBomb(myJoystick.direction.normalized);
    }

    void TouchArrowManagement()
    {
        if (myJoystick.touchingJoystick)
        {
            arrowTransform.up = -myJoystick.direction;

            if (!arrowActivated && validTouch)
            {
                arrowActivated = true;
                touchArrowAnim.SetBool("enabled", true);
            }
        }

        if (!myJoystick.touchingJoystick && arrowActivated || !validTouch)
        {
            arrowActivated = false;
            touchArrowAnim.SetBool("enabled", false);
        }
    }
}
