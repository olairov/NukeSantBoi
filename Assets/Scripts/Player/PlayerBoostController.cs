using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoostController : MonoBehaviour
{
    [SerializeField] float trailSpeed;

    private TrailRenderer myTrail;

    void Start()
    {
        myTrail = transform.Find("Boosts/BoostTrail1").GetComponent<TrailRenderer>();
    }

    void FixedUpdate()
    {
        PushTrailBackwards();
    }

    private void PushTrailBackwards()
    {
        for (int posNum = 0; posNum < myTrail.positionCount; posNum++)
        {
            Debug.Log(posNum);
            myTrail.SetPosition(posNum, myTrail.GetPosition(posNum) - new Vector3(trailSpeed * Time.deltaTime * ObjectPassingBy.speedMultiplier, 0, 0));
        }
    }
}
