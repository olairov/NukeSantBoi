 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    private Camera mainCamera;
    private Animator myAnimator;
    private HudController hudScript;

    private float mouseSensitivity = 1;
    public float SetMouseSensitivity
    {
        set { mouseSensitivity = value; }
    }

    void Start()
    {
        mainCamera = Camera.main;
        myAnimator = GetComponent<Animator>();
        hudScript = GameObject.Find("________________Canvas________________").GetComponent<HudController>();

        Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump")) myAnimator.SetBool("Clicked", true);
        else if ((Input.GetButtonUp("Fire1") && !Input.GetButton("Jump")) || (Input.GetButtonUp("Jump") && !Input.GetButton("Fire1"))) myAnimator.SetBool("Clicked", false);

        UpdatePos();
    }

    void UpdatePos()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePos;

        // If game is paused or finished, use OS's cursor.
        if (hudScript.GetPretendsToBePaused || PlayerController.dead) transform.position = new Vector3(0, -20, 0);
    }
}
