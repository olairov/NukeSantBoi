using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTypeB : BaseMovement
{
    private Transform playerTransform;
    private Rigidbody2D playerRB;

    public MovementTypeB(Transform transform, Rigidbody2D rb)
    {
        playerTransform = transform;
        playerRB = rb;
    }

    public override void MovementProcess()
    {

    }
}
