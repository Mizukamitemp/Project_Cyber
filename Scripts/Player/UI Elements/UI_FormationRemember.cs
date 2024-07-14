using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FormationRemember : UI_Element
{
    public override void Click()
    {
        //Debug.Log("I'm special");
        uimanager.FormationRemember();
    }
}
