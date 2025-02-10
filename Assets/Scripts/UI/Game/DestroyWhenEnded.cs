using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenEnded : MonoBehaviour
{
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
