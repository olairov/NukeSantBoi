using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour, ResetPoolObject
{
    void Start()
    {
        Initialize();
    }

    public void ResetState()
    {
        
    }

    public void Initialize()
    {
        transform.position = new Vector3(Random.Range(Camera.main.ScreenToWorldPoint(Vector2.zero).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x),
        Random.Range(Camera.main.ScreenToWorldPoint(Vector2.zero).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y),
        transform.position.z);
        
        transform.localScale *= Random.Range(2, 4);
        
        GetComponent<ObjectPassingBy>().passingSpeed = Random.Range(1, 2) * transform.localScale.x;
        
        if (Random.value > 0.5f) transform.localScale = new Vector2(transform.localScale.x, -transform.localScale.y);
    }
}
