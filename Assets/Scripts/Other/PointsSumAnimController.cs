using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointsSumAnimController : MonoBehaviour
{
    private TMP_Text pointsCounter;

    private int points, pointsIsum;
    public int SetPoints
    {
        set { points = value; }
    }
    public int SetPointsIsum
    {
        set { pointsIsum = value; }
    }

    private float timeIstart;

    void Start()
    {
        pointsCounter = GameObject.Find("Canvas/PointsCounter").GetComponent<TMP_Text>();
        transform.GetChild(1).GetComponent<TMP_Text>().text = "+" + pointsIsum;

        timeIstart = Time.time;
    }

    private void Update()
    {
        if (Time.time - timeIstart > 1f) Destroy(gameObject);
    }

    public void ChangePointsValue()
    {
        pointsCounter.text = points.ToString();
    }
}
