using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoostController : MonoBehaviour
{
    [SerializeField] float boostSpeed;

    [SerializeField] bool imEnemy;

    private EnemyPlaneController myPlaneController;

    private TrailRenderer myTrail;

    void Start()
    {
        myTrail = transform.GetChild(0).GetComponent<TrailRenderer>();

        if (imEnemy) myPlaneController = transform.parent.GetComponent<EnemyPlaneController>();
    }

    void Update()
    {
        if (imEnemy ? !myPlaneController.dead : !PlayerController.dead) GoBackwards();
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    void GoBackwards()
    {
        myTrail.time = Random.Range(0.12f, 0.16f);
        myTrail.endWidth = Random.Range(0.25f, 0.15f);

        for (int positionNum = 0; positionNum < transform.GetChild(0).GetComponent<TrailRenderer>().positionCount; positionNum++)
        {
            myTrail.SetPosition(positionNum, new Vector3(imEnemy? myTrail.GetPosition(positionNum).x - boostSpeed * Time.deltaTime * ObjectPassingBy.speedMultiplier - 5 * Time.deltaTime * ObjectPassingBy.speedMultiplier : myTrail.GetPosition(positionNum).x - boostSpeed * Time.deltaTime * ObjectPassingBy.speedMultiplier, myTrail.GetPosition(positionNum).y, myTrail.GetPosition(positionNum).z));
        }

        transform.Find("Light/LightRays").eulerAngles = new Vector3(0, 0, 180);
    }
}
