using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundMovingBombsController : MonoBehaviour
{
    private List<RawImage> rowsList = new List<RawImage>();

    [SerializeField] private float bombsSpeed;
    private float lastY, lastFasterY;

    void Start()
    {
        CreateRowsAndList();
    }

    private void CreateRowsAndList()
    {
        Transform firstBombsRow = transform.GetChild(0);

        float startPosition = Camera.main.ScreenToWorldPoint(Vector3.zero).x, limitPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;

        Debug.Log("Start Position: " + startPosition + ", Limit Position: " + limitPosition);

        firstBombsRow.position = new Vector3(startPosition, 0, 0);
        rowsList.Add(firstBombsRow.GetComponent<RawImage>());

        for (int displacement = 4; startPosition + displacement < limitPosition + 4; displacement += 4)
        {
            Transform newRow = Instantiate(firstBombsRow, transform);
            newRow.position = new Vector3(startPosition + displacement, 0, 0);
            rowsList.Add(newRow.GetComponent<RawImage>());
        }
    }

    void Update()
    {
        DisplaceRows();
    }

    private void DisplaceRows()
    {
        lastY += bombsSpeed * Time.deltaTime;
        lastFasterY += bombsSpeed * 1.5f * Time.deltaTime;

        for (int row = 0; row < rowsList.Count; row++)
        {
            rowsList[row].uvRect = new Rect(0, row % 2 == 0? lastY : lastFasterY, 1, 7);
        }
    }
}
