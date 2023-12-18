using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VignetteEffectController : MonoBehaviour
{
    private Animator myAnim;

    void Start()
    {
        myAnim = GetComponent<Animator>();
        AdjustVignette();
    }

    void AdjustVignette()
    {
        transform.position = Vector2.zero;

        float cameraWidthInUnits = Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector2.zero).x) + Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x;
        GetComponent<SpriteRenderer>().size = new Vector2(cameraWidthInUnits, GetComponent<SpriteRenderer>().size.y);
    }

    public void Explosion(bool playerDies)
    {
        if (playerDies) myAnim.SetTrigger("Death");
        else myAnim.SetTrigger("Explosion");
    }
}
