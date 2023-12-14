using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoopChecker : MonoBehaviour
{
    [SerializeField] private GameObject pointsPrefab;

    private HudController hudScript;

    private Rigidbody2D rb;

    private Transform pointsContainer;

    private float realZrot;

    void Start()
    {
        pointsContainer = GameObject.Find("NotPhysicElementsContainer").transform;
        hudScript = GameObject.Find("________________Canvas________________").GetComponent<HudController>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (PlayerController.dead) return;

        realZrot += rb.angularVelocity * Time.unscaledDeltaTime;

        CheckLoop();
    }

    void CheckLoop()
    {
        Debug.Log(realZrot);
        if (realZrot > 340)
        {
            realZrot -= 360;
            AddPointsWhenLoop();
            Debug.Log("Looped");
        }
        if (realZrot < -310)
        {
            realZrot += 360;
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
