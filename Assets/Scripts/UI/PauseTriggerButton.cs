using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseTriggerButton : MonoBehaviour
{
    private HudController hudScript;
    private PlayerController playerScript;

    void Start()
    {
        hudScript = GameObject.Find("________________Canvas________________").GetComponent<HudController>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    public void Pointed()
    {
        playerScript.SetCanDropBomb = false;
    }

    public void Unpointed()
    {
        playerScript.SetCanDropBomb = true;
    }

    public void Pressed()
    {
        if (PlayerController.dead) return;

        if (hudScript.GivePretendsToBePaused)
        {
            hudScript.Continue();
        }
        else
        {
            //hudScript.IsPaused = false;
            hudScript.Pause();
        }
    }
}
