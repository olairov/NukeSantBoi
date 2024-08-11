using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBuildingDetector : MonoBehaviour
{
    [SerializeField] private float escapeSpeed;
    private float screenDistance;

    private ObstacleScript myObstacleScript;

    void Start()
    {
        screenDistance = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x + 1;
        myObstacleScript = transform.parent.GetComponent<ObstacleScript>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == 3)
        {
            Vector2 direction = (transform.position - other.transform.position).normalized;
            if (other.CompareTag("Skystraper")) direction = new Vector2(transform.position.x - other.transform.position.x, 0).normalized;
            direction *= escapeSpeed;

            if (transform.position.x > screenDistance) HardEscapeBuilding(direction);
            else myObstacleScript.DirChange(direction);
        }

    }
    void HardEscapeBuilding(Vector2 direction)
    {
        transform.parent.position += (Vector3)direction;
    }
}
