using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsController : MonoBehaviour
{
    private int componentsAlreadyActive, targetActiveComponents;
    public int SumActiveComponent
    {
        get { return componentsAlreadyActive; }
        set { componentsAlreadyActive = value; }
    }

    void Start()
    {
        for (int idx = 0; idx < transform.childCount; idx++)
        {
            if (transform.GetChild(idx).GetComponent<OptionsComponentController>()) targetActiveComponents++;
        }
    }

    void Update()
    {
        if (componentsAlreadyActive >= targetActiveComponents)
        {
            GameObject.Find("________________Canvas________________").GetComponent<HudController>().SetCanDisableOptionsMenu = true;
            Destroy(this);
        }
    }

    // Program used to let know the canvas when it's secure to disable the options menu at the beggining of the scene.
}
