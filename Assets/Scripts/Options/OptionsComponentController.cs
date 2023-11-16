using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsComponentController : MonoBehaviour
{
    void Start()
    {
        transform.parent.GetComponent<OptionsController>().SumActiveComponent += 1;
    }
}
