using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoopChecker : MonoBehaviour
{
    [SerializeField] private GameObject pointsPrefab;

    private HudController hudScript;

    private Transform pointsContainer;

    void Start()
    {
        pointsContainer = GameObject.Find("NotPhysicElementsContainer").transform;
        hudScript = GameObject.Find("________________Canvas________________").GetComponent<HudController>();
    }

    void Update()
    {
        CheckLoop(transform.eulerAngles.z - 180);
    }

    void CheckLoop(float zRot)
    {
        Debug.Log(transform.eulerAngles.z);
        if (zRot > 360)
        {
            transform.eulerAngles -= new Vector3(0, 0, 360);
            AddPointsWhenLoop();
            Debug.Log("Looped");
        }
        if (zRot < -360)
        {
            transform.eulerAngles += new Vector3(0, 0, 360);
            AddPointsWhenLoop();
            Debug.Log("Looped");
        }
    }

    void AddPointsWhenLoop()
    {
        Instantiate(pointsPrefab, transform.position + new Vector3(0, 1, -1), Quaternion.identity, pointsContainer).GetComponentInChildren<TMP_Text>().text = "+ 4";
        hudScript.ChangePointsValue(4);
    }
}
