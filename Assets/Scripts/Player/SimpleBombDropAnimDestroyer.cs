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

    private void Start()
    {
        Transform playerTransform = GameObject.Find("Player").transform;
        transform.parent.position = playerTransform.position + myDirection;
        transform.position = new Vector3(transform.position.x + (transform.position.x - playerTransform.position.x) / 2, transform.position.y, transform.position.z);
    }

    public void Destroy()
    {
        Destroy(transform.parent.gameObject);
    }
}
