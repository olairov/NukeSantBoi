using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitLevelSelectionButton : MonoBehaviour
{
    public void Exit()
    {
        transform.parent.GetComponent<Animator>().SetTrigger("exit");
        LevelButtonResizer.exittingScene = true;
    }
}
