using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoostController : MonoBehaviour
{
    [SerializeField] bool imEnemy;

    private EnemyPlaneController myPlaneController;

    private List<TrailRenderer> myTrails = new List<TrailRenderer>();

    private Transform myBoosts, playerTransform;

    private int myActualBoost;

    private bool alreadyDisabledBoosts;

    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        myBoosts = transform.Find("Boosts");
        for (int child = 0; child < myBoosts.childCount; child++) myTrails.Add(myBoosts.GetChild(child).GetComponent<TrailRenderer>());

        if (imEnemy) myPlaneController = transform.parent.GetComponent<EnemyPlaneController>();
    }

    void Update()
    {
        if (imEnemy ? !myPlaneController.dead : !PlayerController.dead)
        {
            //ChangeBoostSprite();
            GoBackwards();
        }
        else if (!alreadyDisabledBoosts)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);

            alreadyDisabledBoosts = true;
        }
    }

    void GoBackwards()
    {
        float playerRotIdx = Mathf.Cos(transform.parent.eulerAngles.z / 57.3f);
        if (!imEnemy) playerRotIdx *= -2;

        for (int positionNum = 0; positionNum < myTrails[myActualBoost].positionCount; positionNum++)
        {
            myTrails[myActualBoost].SetPosition(positionNum, new Vector3(myTrails[myActualBoost].GetPosition(positionNum).x - 5f * playerRotIdx * ObjectPassingBy.realSpeedMultiplier * Time.deltaTime, myTrails[myActualBoost].GetPosition(positionNum).y, myTrails[myActualBoost].GetPosition(positionNum).z));
            //imEnemy? myTrails[myActualBoost].GetPosition(positionNum).x - 5f * playerRotIdx * ObjectPassingBy.realSpeedMultiplier * Time.deltaTime : myTrails[myActualBoost].GetPosition(positionNum).x - 10 * playerRotIdx * ObjectPassingBy.realSpeedMultiplier * Time.deltaTime
        }

        transform.Find("Light/LightRays").eulerAngles = new Vector3(0, 0, 180);
    }

    void ChangeBoostSprite()
    {
        myTrails[myActualBoost].widthMultiplier = 0;
        myActualBoost++;
        if (myActualBoost > 2) myActualBoost = 0;
        myTrails[myActualBoost].widthMultiplier = 0.8f;
    }
}
