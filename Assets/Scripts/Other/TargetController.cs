 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    private Camera mainCamera;
    private Animator myAnimator;

    private bool isPaused;
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
        Vector2 targetPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        transform.position = targetPos;

        if (isPaused || PlayerController.dead) transform.position = new Vector3(0, -20, 0);
    }
}
