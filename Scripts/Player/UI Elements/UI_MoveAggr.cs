using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MoveAggr : UI_Element
{
    public override void Click()
    {
        //Debug.Log("I'm special");
        uimanager.MoveModeAggr();
    }
}
