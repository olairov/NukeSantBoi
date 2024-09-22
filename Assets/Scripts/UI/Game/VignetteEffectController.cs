using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VignetteEffectController : MonoBehaviour
{
    private Animator myAnim;

    void Start()
    {
        myAnim = GetComponent<Animator>();
    }

    public void Explosion(bool playerDies)
    {
        if (playerDies) myAnim.SetTrigger("Death");
        else myAnim.SetTrigger("Explosion");
    }
}
