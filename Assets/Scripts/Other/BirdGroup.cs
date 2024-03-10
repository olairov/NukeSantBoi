using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdGroup : MonoBehaviour
{
    void Start()
    {
        ChoseStats();
    }

    private void ChoseStats()
    {
        transform.localScale = Vector3.one * Random.Range(0.6f, 1f);

        int childNum = transform.childCount; // Get the num of children before the loop, as while on it, this number is likely to change.

        for (int idx = 0; idx < childNum; idx++)
        {
            if ((transform.GetChild(idx).name.Contains("4") || transform.GetChild(idx).name.Contains("7")) && Random.value > 0.6f)
            {
                Destroy(transform.GetChild(idx).gameObject);
                continue;
            }

            transform.GetChild(idx).localScale = Vector3.one * Random.Range(0.23f, 0.37f);
        }
    }

    void Update()
    {
        
    }
}
