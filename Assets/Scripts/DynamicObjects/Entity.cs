using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, ResetPoolObject
{
    public bool dead;

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        
    }

    // Reset Pooled Object State

    public virtual void ResetState()
    {
        dead = false;
    }

    public virtual void Initialize()
    {
        
    }
}
