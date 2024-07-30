using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAwayWhenExplosion : MonoBehaviour
{
    [SerializeField] private float pushAwayForce, pushAwayTimeLasts;
    private float pushAwayProgress, pushAwayTime;

    void Update()
    {
        if (pushAwayTime > 0) KeepPushingAway();
    }

    public void PushAway(float Xdirection, float distance)
    {
        pushAwayProgress = -Xdirection * pushAwayForce / (distance * 2.5f);

        pushAwayTime = pushAwayTimeLasts;
    }

    private void KeepPushingAway()
    {
        pushAwayTime /= 1 + (Time.deltaTime * 3);

        float pushAwayLerpProgress = (-pushAwayTime + 1) * 5;
        if (pushAwayTime < pushAwayTimeLasts - 0.2f) pushAwayLerpProgress = pushAwayTime * 1.111f;

        transform.eulerAngles = new Vector3(0, 0, pushAwayProgress * pushAwayLerpProgress);
    }
}
