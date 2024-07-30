using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBombDropAnimDestroyer : MonoBehaviour
{
    private Vector3 myDirection;
    public Vector3 SetMyDirection
    {
        set { myDirection = value; }
    }

    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        transform.parent.position = playerTransform.position + myDirection;
    }

    private void Update()
    {
        
    }

    public void Destroy()
    {
        Destroy(transform.parent.gameObject);
    }
}
