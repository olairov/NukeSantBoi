using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombJoystick : MonoBehaviour
{
    private Joystick myJoystick;

    private Animator touchArrowAnim;
    private Transform arrowTransform;

    private Vector2 lastDirection;

    private bool arrowActivated;

    private PlayerController playerScript;

    void Start()
    {
        if (PlayerController.dead) return;

        arrowTransform = GameObject.Find("Player/TouchArrow").transform;
        touchArrowAnim = arrowTransform.GetComponent<Animator>();

        myJoystick = transform.GetComponent<Joystick>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        if (PlayerController.dead) return;
        CheckJoystick();
        TouchArrowManagement();
    }

    void CheckJoystick()
    {
        if (lastDirection != Vector2.zero && myJoystick.Direction == Vector2.zero) playerScript.DropBomb(lastDirection);

        lastDirection = myJoystick.Direction;
    }

    void TouchArrowManagement()
    {
        if (myJoystick.Direction != Vector2.zero)
        {
            arrowTransform.up = -myJoystick.Direction;

            if (!arrowActivated)
            {
                arrowActivated = true;
                touchArrowAnim.SetBool("enabled", true);
            }
        }

        if (myJoystick.Direction == Vector2.zero && arrowActivated)
        {
            arrowActivated = false;
            touchArrowAnim.SetBool("enabled", false);
        }
    }
}
