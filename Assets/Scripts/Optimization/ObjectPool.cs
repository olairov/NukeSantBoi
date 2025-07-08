using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    Queue<GameObject> pool = new Queue<GameObject>();
    public GameObject poolObject;
    public Transform containerObject;
    public int defaultObjectCount;

    private void Awake()
    {
        if (containerObject == null) containerObject = transform;

        for (int idx = 0; idx < defaultObjectCount; idx++)
        {
            GameObject enqueuedObject = CreatePoolObject(poolObject, containerObject);
            enqueuedObject.SetActive(false);
            pool.Enqueue(enqueuedObject);
        }
    }

    public GameObject GetObject(bool resetObject)
    {
        if (pool.Count > 0)
        {
            GameObject dequeuedObject = pool.Dequeue();
            dequeuedObject.SetActive(true);
            if (dequeuedObject.GetComponent<PooledObject>().alreadyUsed)
            {
                foreach (var poolable in dequeuedObject.GetComponents<ResetPoolObject>())
                {
                    poolable.ResetState();
                    if (resetObject) poolable.Initialize();
                }
            }
            return dequeuedObject;
        }
        else return CreatePoolObject(poolObject, containerObject);
    }

    GameObject CreatePoolObject(GameObject objectToInstantiate, Transform container)
    {
        GameObject instantiatedObject = Instantiate(objectToInstantiate, container);
        instantiatedObject.AddComponent<PooledObject>().myObjectPool = GetComponent<ObjectPool>();
        return instantiatedObject;
    }

    public void ReturnObject(GameObject returnedObject)
    {
        returnedObject.GetComponent<PooledObject>().alreadyUsed = true;
        returnedObject.SetActive(false);
        pool.Enqueue(returnedObject);
    }
}