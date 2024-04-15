 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    private Camera mainCamera;
    private Animator myAnimator;

    private float mouseSensitivity = 1;
    public float SetMouseSensitivity
    {
        set { mouseSensitivity = value; }
    }

    private bool isPaused, lastFrameWasPaused = true;
    public bool SetIsPaused
    {
        set { isPaused = value; }
    }

    void Start()
    {
        mainCamera = Camera.main;
        myAnimator = GetComponent<Animator>();

        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump")) myAnimator.SetBool("Clicked", true);
        else if ((Input.GetButtonUp("Fire1") && !Input.GetButton("Jump")) || (Input.GetButtonUp("Jump") && !Input.GetButton("Fire1"))) myAnimator.SetBool("Clicked", false);

        UpdatePos();
    }

    void UpdatePos()
    {
        //Vector2 targetPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        transform.position += new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0) * 0.4f * mouseSensitivity;

        // Keeping the target inside the bounds

        if (transform.position.x < mainCamera.ScreenToWorldPoint(Vector3.zero).x)
            transform.position = new Vector3(mainCamera.ScreenToWorldPoint(Vector3.zero).x, transform.position.y, transform.position.z);

        if (transform.position.x > mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x)
            transform.position = new Vector3(mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x, transform.position.y, transform.position.z);

        if (transform.position.y < mainCamera.ScreenToWorldPoint(Vector3.zero).y)
            transform.position = new Vector3(transform.position.x, mainCamera.ScreenToWorldPoint(Vector3.zero).y, transform.position.z);

        if (transform.position.y > mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y)
            transform.position = new Vector3(transform.position.x, mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y, transform.position.z);

        // If game is paused or finished, use OS's cursor.
        if (isPaused || PlayerController.dead)
        {
            transform.position = new Vector3(0, -20, 0);
            lastFrameWasPaused = true;
        }
        else if (lastFrameWasPaused)
        {
            transform.position = Vector3.zero;
            lastFrameWasPaused = false;
        }
    }
}
