using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneController : MonoBehaviour, ResetPoolObject
{
    ObjectPool cranePool;

    // Start is called before the first frame update
    void Start()
    {
        cranePool = GameObject.Find("DetailGenerator/CraneGenerator/RepeatedCranes").GetComponent<ObjectPool>();
        Initialize();
    }


    // Reset Pooled Object State

    public void ResetState()
    {
        
    }

    public void Initialize()
    {
        if (Random.value > 0.6f)
        {
            Transform newCraneTransform = cranePool.GetObject(false).transform;

            newCraneTransform.position += new Vector3(Random.value > 0.5f ? 4 : -4, -3, 0.2f);
            newCraneTransform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1) * 0.8f;
        }
    }
}
