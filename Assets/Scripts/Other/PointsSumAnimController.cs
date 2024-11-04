using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointsSumAnimController : MonoBehaviour
{
    private TMP_Text pointsCounter;
    private AudioSource coinSound;

    [SerializeField] private float everyPointAddInterval;

    private int points, pointsIsum;
    public int SetPoints
    {
        set { points = value; }
    }
    public int SetPointsIsum
    {
        set { pointsIsum = value; }
    }

    void Start()
    {
        pointsCounter = GameObject.Find("Canvas/PointsCounter").GetComponent<TMP_Text>();
        coinSound = GetComponent<AudioSource>();
        transform.GetChild(1).GetComponent<TMP_Text>().text = "+" + pointsIsum;
    }

    public void ChangePointsValue()
    {
        //StartCoroutine(SlowlyAddPoints());
    }

    private IEnumerator SlowlyAddPoints()
    {
        for (int idx = 1; idx <= pointsIsum; idx++)
        {
            pointsCounter.text = (points - pointsIsum + idx).ToString(); // As "points" is not changed progressively, I have to substract the points THIS explosion sums
            // from the total points I have, this way it's like summing 0 points to the previous score. Then add "idx", which DOES change progressively, to have
            // A nice scale of +1s from the previous score to the actual one.

            coinSound.Stop();
            coinSound.pitch = Random.Range(0.9f, 1.05f);
            coinSound.Play();

            yield return new WaitForSeconds(everyPointAddInterval);
        }

        yield return new WaitForSeconds(1);

        Destroy(gameObject);
    }
}
