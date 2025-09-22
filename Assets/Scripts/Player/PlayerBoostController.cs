using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoostController : MonoBehaviour
{
    [SerializeField] float trailSpeed;

    private TrailRenderer myTrail;

    private Transform lightTransform;

    private bool imEnemyPlane;

    private void Awake()
    {
        myTrail = transform.Find("BoostTrail").GetComponent<TrailRenderer>();
    }

    void Start()
    {
        lightTransform = transform.Find("Light");

        imEnemyPlane = transform.parent.name.StartsWith("Enemy");
    }

    private void Update()
    {
        if (Time.deltaTime < 0.02f) PushTrailBackwards();
        ChangeBoostSprite();
        lightTransform.eulerAngles = Vector3.zero;

        if (PlayerController.dead && !imEnemyPlane) transform.gameObject.SetActive(false);

        PushTrailBackwards();
    }

    private void PushTrailBackwards()
    {
        for (int posNum = 0; posNum < myTrail.positionCount; posNum++)
        {
            myTrail.SetPosition(posNum, myTrail.GetPosition(posNum) - new Vector3(trailSpeed * Time.deltaTime * ObjectPassingBy.speedMultiplier, 0, 0));
        }
    }

    private void ChangeBoostSprite()
    {
        if (Time.timeScale <= 0) return;
        myTrail.widthMultiplier = Random.Range(0.7f, 1.3f);
    }

    public void ResetTrail()
    {
        myTrail.Clear();
    }
}
