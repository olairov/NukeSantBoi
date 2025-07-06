using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenEnded : MonoBehaviour
{
    [SerializeField] GameObject objectToDestroy;

    public void Destroy()
    {
        GameObject destroyedObject = objectToDestroy == null ? gameObject : objectToDestroy;
        
        if (GetComponent<PooledObject>() != null)
        {
            GetComponent<PooledObject>().ReturnToPool(destroyedObject);
            return;
        }

        if (destroyedObject.transform.parent.GetComponent<ObjectPool>() != null)
        {
            destroyedObject.transform.parent.GetComponent<ObjectPool>().ReturnObject(destroyedObject);
        }
        else
        {
            Debug.LogWarning("No Object Pool Assigned nor found in parent: " + destroyedObject.name);
            Destroy(destroyedObject);
        }
    }
}
