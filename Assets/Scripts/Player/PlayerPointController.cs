using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPointController : MonoBehaviour
{
    public int otherPointsTouched;

    void Start()
    {
        
    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("playerPoint")) otherPointsTouched++;
    }
}
