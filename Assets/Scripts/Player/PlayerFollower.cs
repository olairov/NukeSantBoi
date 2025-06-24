using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    Transform player;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    void Update()
    {
        if (player != null) transform.position = player.position;
    }
}
