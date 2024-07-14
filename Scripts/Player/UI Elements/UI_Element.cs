using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Element : MonoBehaviour
{
    private GameObject gameobject;
    public UIManager uimanager;

    public string CursorHoverText;

    void Start()
    {
        gameobject = GameObject.Find("UIManager");
        uimanager = gameobject.GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Click()
    {
        Debug.Log("I'm clicked!");
    }

    public virtual void CursorHover()
    {
        if (uimanager != null)
        {
            uimanager.ShowMessage(CursorHoverText);
        }
    }
}
