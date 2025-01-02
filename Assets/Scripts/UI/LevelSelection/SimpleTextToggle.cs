using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTextToggle : MonoBehaviour
{
    private GameObject obj1, obj2;

    private Transform infoSelectedTransform;

    private Animator infoSelectedAnimator;

    private AudioSource clickedSound;

    private bool toggled;

    void Start()
    {
        clickedSound = GameObject.Find("UIsounds/SliderSound").GetComponent<AudioSource>();
        infoSelectedTransform = GameObject.Find("CanvasLevelSelection/MainScreen/infoGlow").transform;
        infoSelectedAnimator = infoSelectedTransform.GetComponent<Animator>();

        obj1 = transform.Find("Text1").gameObject;
        obj2 = transform.Find("Text2").gameObject;

        obj2.SetActive(false);
    }

    public void Pressed()
    {
        toggled = !toggled;

        obj1.SetActive(!toggled);
        obj2.SetActive(toggled);

        infoSelectedTransform.position = new Vector3(transform.Find("icon").position.x, transform.Find("icon").position.y, infoSelectedTransform.position.z);
        infoSelectedAnimator.SetTrigger("selected");

        clickedSound.Play();
    }
}
