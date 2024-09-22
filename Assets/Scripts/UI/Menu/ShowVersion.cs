using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowVersion : MonoBehaviour
{
    void Start()
    {
        GetComponent<TMP_Text>().text = "v" + Application.version;
    }
}
