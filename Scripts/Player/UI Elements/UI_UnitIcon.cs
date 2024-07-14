using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_UnitIcon : UI_Element
{

    public int UnitIndex = 0;

    public override void Click()
    {
        Debug.Log("UNIT ICON");
        //uimanager.ClearSelection();
        uimanager.UnitIconClicked(UnitIndex);


    }
}
