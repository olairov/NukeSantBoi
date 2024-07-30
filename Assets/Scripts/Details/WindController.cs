using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour
{
    void Start()
    {
        ChoseStats();
    }

    void Update()
    {
        
    }

    void ChoseStats()
    {
        transform.position = new Vector3(Random.Range(Camera.main.ScreenToWorldPoint(Vector2.zero).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x),
        Random.Range(Camera.main.ScreenToWorldPoint(Vector2.zero).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y),
        transform.position.z);
        
        float Xpos = transform.position.x / (Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector2.zero).x) + Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);
        float Ypos = transform.position.y / (Mathf.Abs(Camera.main.ScreenToWorldPoint(Vector2.zero).y) + Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
        float XsizeMultiplier = 8 * Mathf.Pow(Xpos, 4) + 0.5f;
        float YsizeMultiplier = 8 * Mathf.Pow(Ypos, 4) + 0.5f;

        transform.localScale = new Vector2(Random.Range(4, 6) * XsizeMultiplier, Random.Range(4, 6) * YsizeMultiplier);
        
        GetComponent<ObjectPassingBy>().passingSpeed = Random.Range(8, 12) * ((XsizeMultiplier + YsizeMultiplier) / 2);
        
        if (Random.value > 0.5f) transform.localScale = new Vector2(transform.localScale.x, -transform.localScale.y);
    }
}
