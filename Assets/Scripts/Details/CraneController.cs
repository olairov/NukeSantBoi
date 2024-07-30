using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneController : MonoBehaviour
{
    [SerializeField] private GameObject cranePrefab;

    public bool imRepeated;

    // Start is called before the first frame update
    void Start()
    {
        if (!imRepeated) ChooseStats();
    }

    void ChooseStats()
    {
        transform.position = new Vector3(transform.position.x, Random.Range(-2, 0), transform.position.z);
        transform.localScale = new Vector3(Random.value > 0.5f ? 1 : -1, 1, 1) * Random.Range(0.8f, 1.1f);

        if (Random.value > 0.6f)
        {
            Transform newCraneTransform = Instantiate(cranePrefab, GameObject.Find("DetailsContainer").transform).transform;

            newCraneTransform.position += new Vector3(Random.value > 0.5f ? 4 : -4, -2, 0.2f);
            newCraneTransform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1) * 0.8f;
            newCraneTransform.GetComponent<CraneController>().imRepeated = true;
            newCraneTransform.GetComponent<ObjectPassingBy>().appearingObject = true;
        }
    }
}
