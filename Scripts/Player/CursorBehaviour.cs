using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
//using UnityEngine.UIElements;


public class CursorBehaviour : MonoBehaviour
{



    [SerializeField] GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    [SerializeField] EventSystem m_EventSystem;
    [SerializeField] RectTransform canvasRect2;
    public PlayerController controller;


    public float Sensitivity = 100f;
    float mouseX;
    float mouseY;
    RectTransform rectTransform;

    public RectTransform CanvasRect;

    private Vector3 mousePos;
    //    public PlayerInput Input;
    private float timer;
    public UIManager uimanager;

    public bool AimingFireMode = false;

    // Start is called before the first frame update
    void Start()
    {

        rectTransform = GetComponent<RectTransform>();
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if ((double)timer > 0.5)
        {
            RayCastUIForHints();
            timer = 0f;
        }
    }

    private void RayCastUIForHints()
    {
        m_PointerEventData = new PointerEventData(EventSystem.current);
        m_PointerEventData.position = GetCursorScreenPosInvert();
        List<RaycastResult> list = new List<RaycastResult>();
        EventSystem.current.RaycastAll(m_PointerEventData, list);
        if (list.Count > 0)
        {
            GameObject gameObject = list[0].gameObject;
            if (gameObject.GetComponent<UI_Element>() != null)
            {
                gameObject.GetComponent<UI_Element>().CursorHover();
            }
        }
        else if (uimanager != null)
        {
            uimanager.ShowMessage("");
        }
    }


    // Мы рэйкастим с виртуального курсора и получаем графику и запускаем с нее функцию Click
    private void OnMouseClick(InputValue value) // Тут мы выделяем юнитсы
    {
        m_PointerEventData = new PointerEventData(EventSystem.current);
        m_PointerEventData.position = GetCursorScreenPosInvert();
        List<RaycastResult> results = new List<RaycastResult>();


        if (value.isPressed)
        {
            EventSystem.current.RaycastAll(m_PointerEventData, results);

            if (results.Count > 0)
            {
                Debug.Log("Hit " + results[0].gameObject.name);
                GameObject o = results[0].gameObject;

                if (o.GetComponent<UI_Element>() != null)
                {
                    UI_Element myUI = o.GetComponent<UI_Element>();
                    Debug.Log("Got UI_Element ");
                    myUI.Click();
                }
            }
            else
            {
                if (controller != null)
                {
                    controller.LeftClick();
                }
            }
        }
    }



    /// <summary>
    /// Здесь курсор передвигается
    /// </summary>
    /// <param name="value"></param>
    public void OnMouseView(InputValue value)
    {
        mouseY = value.Get<Vector2>().y;
        mouseX = value.Get<Vector2>().x;

        if (!controller.GetRightMouseDown() && !AimingFireMode)
        {

            // получаем от - ширина канваса до + ширина канваса
            mousePos.x = Mathf.Clamp(mousePos.x + mouseX, -CanvasRect.rect.width / 2, CanvasRect.rect.width / 2);
            mousePos.y = Mathf.Clamp(mousePos.y + mouseY, -CanvasRect.rect.height / 2, CanvasRect.rect.height / 2);
            mousePos.z = 0f;

            // добавить сенситивити!

            rectTransform.localPosition = mousePos;
        }

        if (AimingFireMode) //перс целится
        {
            // получаем от - ширина канваса до + ширина канваса
            mousePos.x = 0f;
            mousePos.y = 0f;
            mousePos.z = 0f;

            // добавить сенситивити!

            rectTransform.localPosition = mousePos;

        }

    }

    public void CursorCenter()
    {
        mousePos.x = 0f;
        mousePos.y = 0f;
        mousePos.z = 0f;
        rectTransform.localPosition = mousePos;
    }

    /// <summary>
    /// Координата виртуального курсора от левого нижнего угла
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCursorScreenPosInvert()
    {
        Vector3 CursorTopLeftCoordPercent;
        Vector3 CursorScreenCoords;

        CursorTopLeftCoordPercent.x = (mousePos.x + (CanvasRect.rect.width / 2))/ CanvasRect.rect.width;
        CursorTopLeftCoordPercent.y = (mousePos.y + (CanvasRect.rect.height / 2))/ CanvasRect.rect.height;
        CursorTopLeftCoordPercent.z = 0;



        CursorScreenCoords.x = CursorTopLeftCoordPercent.x * Screen.width;
        CursorScreenCoords.y = CursorTopLeftCoordPercent.y * Screen.height;
        CursorScreenCoords.z = 0;






        // Debug.Log($"mouseX" + CursorScreenCoords.x);

        return CursorScreenCoords;
    }
   // CursorScreenCoords.x = CursorTopLeftCoordPercent.x * Screen.width;
 
/// <summary>
/// Вверх +1 по Y; Вниз - -1 по Y
/// </summary>
/// <returns></returns>
    public Vector3 GetCursorVector1()
    {
        Vector3 CursorVector;




        CursorVector.x = Mathf.Clamp(mousePos.x/(CanvasRect.rect.width/2), -1, 1);
        CursorVector.y = Mathf.Clamp(mousePos.y / (CanvasRect.rect.height/2), -1, 1);
        CursorVector.z = 0f;


        return CursorVector;
    }

    /// <summary>
    /// Вверх +1/2 экрана по Y; Вниз -1/2 экрана
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCursorVector()
    {
        Vector3 CursorVector;
        //float x;
        //float y;
        CursorVector.x = mousePos.x;
        CursorVector.y = mousePos.y;
        CursorVector.z = 0f;

        

        return CursorVector;
    }

}
