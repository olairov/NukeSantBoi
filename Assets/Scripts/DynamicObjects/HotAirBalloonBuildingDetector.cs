using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotAirBalloonBuildingDetector : MonoBehaviour
{
    [SerializeField] private float escapeSpeed;
    private float screenDistance;

    private HotAirBalloonScript myOhotAirBalloonScript;

    private bool willEscapeTheBuilding;

    void Start()
    {
        willEscapeTheBuilding = Random.value > 0.7f;

        screenDistance = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x + 1;
        myOhotAirBalloonScript = transform.parent.GetComponent<HotAirBalloonScript>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == 3)
        {
            Vector2 direction = (transform.position - other.transform.position).normalized;
            if (other.CompareTag("Skyscraper")) direction = new Vector2(transform.position.x - other.transform.position.x, 0).normalized;
            direction *= escapeSpeed;

            if (transform.position.x > screenDistance) HardEscapeBuilding(direction);
            else
            {
                if (willEscapeTheBuilding) myOhotAirBalloonScript.DirChange(direction);
                else myOhotAirBalloonScript.SlowDown();
            }
        }

    }
    void HardEscapeBuilding(Vector2 direction)
    {
        transform.parent.position += (Vector3)direction;
    }
}
