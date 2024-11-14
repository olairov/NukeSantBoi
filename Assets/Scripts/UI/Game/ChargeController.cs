using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeController : MonoBehaviour
{
    private ShakeController shakeScript;
    private Animator playerPowerFieldAnim;
    private AudioSource rechargeSound;

    private List<GameObject> cells = new List<GameObject>();

    private float timeUntilNextBomb, defaultTimeUntilNextBomb;
    
    public float SetDefaultTimeUntilNextBomb
    {
        set { defaultTimeUntilNextBomb = value; }
    }

    void Start()
    {
        shakeScript = transform.GetChild(0).GetComponent<ShakeController>();
        playerPowerFieldAnim = GameObject.Find("Player/LightCircle").GetComponent<Animator>();
        rechargeSound = GetComponent<AudioSource>();

        for (int idx = 0; idx < 5; idx++)
        {
            cells.Add(transform.GetChild(0).GetChild(idx).gameObject);
        }
    }

    void Update()
    {
        IncreaseCounter();
    }

    private void IncreaseCounter()
    {
        if (timeUntilNextBomb <= 0 || PlayerController.dead) return;

        timeUntilNextBomb -= Time.deltaTime;
        if (timeUntilNextBomb <= 0)
        {
            timeUntilNextBomb = 0;
            playerPowerFieldAnim.SetTrigger("Recharge");

            rechargeSound.pitch = Random.Range(0.92f, 1.07f);
            rechargeSound.Play();
        }

        int numberOfCellsEnabled = (int)(5 - timeUntilNextBomb / (defaultTimeUntilNextBomb / ObjectPassingBy.realSpeedMultiplier) * 5) - 1;

        for (int idx = cells.Count - 1; idx >= 0; idx--)
        {
            if (numberOfCellsEnabled >= idx) cells[idx].SetActive(true);
            else cells[idx].SetActive(false);
        }
    }

    public void DropBomb(float givenTimeForNextBomb)
    {
        timeUntilNextBomb = givenTimeForNextBomb;

        shakeScript.Shake(1);
    }
}
