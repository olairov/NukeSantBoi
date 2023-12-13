using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
    private float timeBetweenVertices = 0.1f, timePassedSinceLastVertex;

    [SerializeField] private GameObject pointPrefab;
    private List<GameObject> pointsList = new List<GameObject>();

    private Transform pointsContainer;

    void Start()
    {
        pointsContainer = GameObject.Find("PointsContainer").transform;
    }

    void Update()
    {
        timePassedSinceLastVertex += Time.unscaledDeltaTime;
        if (timePassedSinceLastVertex >= timeBetweenVertices) NewVertex(transform.position);
    }

    private void NewVertex(Vector2 point)
    {
        timePassedSinceLastVertex = 0;

        pointsList.Add(Instantiate(pointPrefab, point, Quaternion.identity, pointsContainer));
        if (pointsList.Count <= 1) return;
        
        GameObject pointToConfigure = pointsList[pointsList.Count - 2];
        Vector2 actualPointToPreviousPointDir = new Vector2(pointsList[pointsList.Count - 1].transform.position.x, pointsList[pointsList.Count - 1].transform.position.y) - new Vector2(pointToConfigure.transform.position.x, pointToConfigure.transform.position.y);
        
        pointToConfigure.transform.up = actualPointToPreviousPointDir;
        pointToConfigure.GetComponent<BoxCollider2D>().size = new Vector2(actualPointToPreviousPointDir.magnitude, pointToConfigure.GetComponent<BoxCollider2D>().size.y);
        pointToConfigure.GetComponent<BoxCollider2D>().offset = new Vector2(actualPointToPreviousPointDir.magnitude / 2, pointToConfigure.GetComponent<BoxCollider2D>().offset.y);

        if (pointsList.Count <= 2) return;

    }
}
