using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public ObjectPool myObjectPool;
    public bool alreadyUsed;

    public void ReturnToPool(GameObject returnedObject)
    {
        if (myObjectPool != null)
        {
            myObjectPool.ReturnObject(returnedObject);
        }
        else
        {
            Debug.LogWarning(returnedObject.name + "'s Object Pool not assigned");
            Destroy(returnedObject);
        }
    }
}
