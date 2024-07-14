using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [Header("Camera Parameters")]
    public float RTSCam_scrollrate = 7;
    private float VerticalCamRotSpeed = 10;
    private float HorizontalCamRotSpeed = 20f;


    public Camera RTSCam;
    public Camera FollowCam;
    public Camera CurrentCam;
    public bool RTSMode;
    public Vector3 CamRotateValue;

    public CursorBehaviour Cursor;
    public Vector3 CursorVector; // Положение курсора нормализованное -1 до +1
    public Vector3 CursorVector1; // Координата виртуального курсора от левого нижнего угла
    float mouseX;
    float mouseY;

    selected_dictionary selected_table; // таблица выбранных юнитов

    public GameObject PlayerPawn;


    private int _side = 0; //Сторона, за которую играет
    public int Side
    {
        get
        {
            return _side;
        }
        set
        {
            _side = value;
        }
    }




    private bool leftMouseDown = false;
    private bool rightMouseDown;


    void Awake()
    {
        Side = PlayerPawn.GetComponent<PlayerStateManager>().Side;

    }
        void Start()
    {



        if (PlayerPawn.GetComponent<PlayerStateManager>() == null) Debug.Log("SHIT");
        PlayerPawn.GetComponent<PlayerStateManager>().makePlayable();
        RTSMode = true;
        RTSCam.enabled = true;
        FollowCam.enabled = false;
        CurrentCam = RTSCam;
        selected_table = GetComponent<selected_dictionary>();   //У нас есть на игроке такой скрипт
        FollowCam.transform.eulerAngles = new Vector3(10, 0, 0);
    }

    public int GetHealth() // Запрашивает UIController для отобр полосы здоровья
    {
        if (PlayerPawn.GetComponent<PlayerStateManager>() != null) return PlayerPawn.GetComponent<PlayerStateManager>().HitPoints;
        else return 0;
    }

    public int GetMaxHealth()   // Запрашивает UIController для отобр полосы здоровья
    {
        if (PlayerPawn.GetComponent<PlayerStateManager>() != null) return PlayerPawn.GetComponent<PlayerStateManager>().MaxHitPoints;
        else return 0;
    }

    public int GetFatigue() // Запрашивает UIController для отобр полосы утомления
    {
        if (PlayerPawn.GetComponent<PlayerStateManager>() != null) return PlayerPawn.GetComponent<PlayerStateManager>().Fatigue;
        else return 0;
    }

    public int GetMaxFatigue() // Запрашивает UIController для отобр полосы утомления
    {
        if (PlayerPawn.GetComponent<PlayerStateManager>() != null) return PlayerPawn.GetComponent<PlayerStateManager>().MaxFatigue;
        else return 0;
    }

    private void OnAttack(InputValue value) 
    {
        if ((PlayerPawn.GetComponent<PlayerStateManager>().CurrentState == PlayerPawn.GetComponent<PlayerStateManager>().IdlingState ||
              PlayerPawn.GetComponent<PlayerStateManager>().CurrentState == PlayerPawn.GetComponent<PlayerStateManager>().WalkingState) &&
              !PlayerPawn.GetComponent<PlayerStateManager>().RPGControlStyle)
        {
            Cursor.AimingFireMode = true;
            Cursor.CursorCenter();
            Ray ray = CurrentCam.ScreenPointToRay(CursorVector1);
            //PlayerPawn.GetComponent<PlayerStateManager>().AimDirection = ray.direction;
            PlayerPawn.GetComponent<PlayerStateManager>().SetRotationToCursor(ray.direction);


            PlayerPawn.GetComponent<PlayerStateManager>().transform.rotation = Quaternion.LookRotation(ray.direction, Vector3.up);
            FollowCam.transform.rotation = Quaternion.LookRotation(ray.direction, Vector3.up);
        }
        PlayerPawn.GetComponent<PlayerStateManager>().CurrentState.execOnAttack(value); // Врубить соответствующую функцию в State - из сост любого - в AIM
    }

    private void OnBlock(InputValue value)
    {
        PlayerPawn.GetComponent<PlayerStateManager>().CurrentState.execOnBlock(value);  // Передать на Pawn, что надо атаковать
    }

    private void OnUse(InputValue value)
    {
    

        if (PlayerPawn.GetComponent<PlayerStateManager>().CurrentState == PlayerPawn.GetComponent<PlayerStateManager>().CargoState)
        {
            RaycastHit hit;

            Ray ray = CurrentCam.ScreenPointToRay(CursorVector1);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {

                if (hit.transform.tag == "Ground") // Если у ландшафта есть такой тэг, то 
                {

                    PlayerPawn.GetComponent<PlayerStateManager>().UnboardLocation=hit.point;
                    PlayerPawn.GetComponent<PlayerStateManager>().SwitchState(PlayerPawn.GetComponent<PlayerStateManager>().IdlingState);

                }
            }
        }
        PlayerPawn.GetComponent<PlayerStateManager>().CurrentState.execOnUse(value);
    }


    private void OnMovement(InputValue value)
    {
            PlayerPawn.GetComponent<PlayerStateManager>().InputVector = value.Get<Vector2>(); // получили с клавиатуры WASD
            PlayerPawn.GetComponent<PlayerStateManager>().CurrentState.execOnMovement(value);
    }





    // сперва курсор проверяет, не попал ли по интерфейсу, а потом просит playercontroller делать LeftClick()
    // чтобы не было клика и по UI и по миру
    public void LeftClick()
    {
        leftMouseDown = true;
    }
    
    private void OnMouseClick(InputValue value) 
    {
        PlayerPawn.GetComponent<PlayerStateManager>().CurrentState.execOnMouseClick(value); // Вызываем действие в состояниях существа

        RaycastHit hit;
        Ray ray = CurrentCam.ScreenPointToRay(CursorVector1); // CursorVector1 - Координата виртуального курсора от левого нижнего угла

        if (value.isPressed)
        {
            // левый клик делаем не здесь. см. PerformTerrainClick() ниже
        }
        else // а тут мы просто отпустили кнопку - нет взаимодействия с UI
        {
            //Debug.Log($"NOT PRESSED");
            leftMouseDown = false;  // сигнализируем, что отпустили мышь

            if ((PlayerPawn.GetComponent<PlayerStateManager>().CurrentState != PlayerPawn.GetComponent<PlayerStateManager>().AimingState) 
                && (PlayerPawn.GetComponent<PlayerStateManager>().CurrentState != PlayerPawn.GetComponent<PlayerStateManager>().AttState) 
                && (!PlayerPawn.GetComponent<PlayerStateManager>().isDead) && (Physics.Raycast(ray, out hit, Mathf.Infinity)))
                // ибо в режимах AimingState и AttState мы стреляем, а не выполняем действие
            {
                if (hit.collider.transform.tag == "CargoStation")   // это точка для крепления юнита на корабле
                {

                    if (hit.collider.transform.gameObject.GetComponent<ShipStation>().ship != null  // если эта точка прикреплена к какому-либо кораблю
                    && (PlayerPawn.GetComponent<PlayerStateManager>().CurrentState == PlayerPawn.GetComponent<PlayerStateManager>().CargoState) // при этом юнит тыкает в нее, будучи уже на корабле
                     && PlayerPawn.GetComponent<PlayerStateManager>().myTransport == hit.collider.transform.gameObject.GetComponent<ShipStation>().ship // и тыкает он в точку СВОЕГО корабля
                     && !hit.collider.transform.gameObject.GetComponent<ShipStation>().IsOccupied   // и точка не занята другим юнитом
                    ) // Забираемся на точку посадки в транспорте
                    { 
                        PlayerPawn.GetComponent<PlayerStateManager>().CargoLocation.GetComponent<ShipStation>().IsOccupied = false; // возвращаем текущую точку в дефолтное состояние
                        PlayerPawn.GetComponent<PlayerStateManager>().CargoLocation.GetComponent<CapsuleCollider>().enabled = true;
                        hit.collider.transform.gameObject.GetComponent<ShipStation>().ship.GetMeInto(hit.collider.transform.gameObject, PlayerPawn);    // кораблю - обработать нового пассажира
                    }
                }


                // попадаем по юниту
                if (hit.collider.transform.tag == "SelectableUnit" // это юнит
                    && !hit.collider.transform.gameObject.GetComponent<PlayerStateManager>().bSelected // он не выбран
                    && hit.collider.transform.gameObject.GetComponent<PlayerStateManager>().Side == Side) // он НАШЕЙ армии
                {
                    selected_table.addSelected(hit.collider.transform.gameObject); // добавляем в список выбранных
                }
                else if (hit.collider.transform.tag == "SelectableUnit" && hit.collider.transform.gameObject.GetComponent<PlayerStateManager>().Side != Side) // он НЕ нашей армии
                {
                    if (PlayerPawn.GetComponent<PlayerStateManager>().GetLockTarget() == null)  // если у игрока нет выделенного вражеского юнита
                    {
                        PlayerPawn.GetComponent<PlayerStateManager>().SetLockTarget(hit.collider.transform.gameObject);  // выделить этот юнит, в которого ткнули мышью
                    }
                    else if (PlayerPawn.GetComponent<PlayerStateManager>().GetLockTarget() == hit.collider.transform.gameObject)    // если у игрока выделен тот же юнит, в которого ткнули мышью
                    {
                        PlayerPawn.GetComponent<PlayerStateManager>().ClearLockTarget(); // развыделить этот юнит
                    }
                    else if (PlayerPawn.GetComponent<PlayerStateManager>().GetLockTarget() != null // у игрока есть выделенный юнит
                        && PlayerPawn.GetComponent<PlayerStateManager>().GetLockTarget() != hit.collider.transform.gameObject) // и это другой юнит
                    {
                        PlayerPawn.GetComponent<PlayerStateManager>().SetLockTarget(hit.collider.transform.gameObject); // тогда выделить этот юнит
                    }
                }
                else if (hit.collider.transform.tag == "SelectableUnit" // если попали по юниту
                    && hit.collider.transform.gameObject.GetComponent<PlayerStateManager>().bSelected) // и юнит в списке выделенного
                {
                    selected_table.deselect(hit.collider.transform.gameObject.GetInstanceID()); // развыделить его и убрать из списка выделенного
                }
            }
        }
    }

    // курсор сперва проверяет клик по UI в другом месте, чтобы не кликать UI и мир. Если нет, он задает leftMouseDown=true
    // если leftMouseDown=true, то update() вызывает PerformTerrainClick() (только в режиме вида сверху)
    private void PerformTerrainClick()
    {
        RaycastHit hit;
        Ray ray = CurrentCam.ScreenPointToRay(CursorVector1);

        if ((!PlayerPawn.GetComponent<PlayerStateManager>().IsStationary) // не нужно сторожевой башне тыкать ландшафт
            && (!PlayerPawn.GetComponent<PlayerStateManager>().isDead) // мертвым тоже
            && (Physics.Raycast(ray, out hit, Mathf.Infinity)))
        {
            if (hit.transform.tag == "Ground") // Если ткнули по земле, то 
            {
                PlayerPawn.GetComponent<PlayerStateManager>().clickLocation = hit.point; //передать существу, управляемому игроком, куда мы ткнули
                
                if (PlayerPawn.GetComponent<PlayerStateManager>().CurrentState == PlayerPawn.GetComponent<PlayerStateManager>().IdlingState) // если существо игрока ждет
                {
                    PlayerPawn.GetComponent<PlayerStateManager>().SwitchState(PlayerPawn.GetComponent<PlayerStateManager>().WalkingState); // пусть идет и разбирается с полученными координатами
                }
            }
        }
    }

    private void OnMouseRightClick(InputValue value) // Тут мы выделяем юнитсы
    {
        RaycastHit hit;
        if (value.isPressed)
        {
            rightMouseDown = true;
            return;
        }
        rightMouseDown = false;

        Ray ray = CurrentCam.ScreenPointToRay(CursorVector1);

        if ((!PlayerPawn.GetComponent<PlayerStateManager>().isDead) &&(Physics.Raycast(ray, out hit, Mathf.Infinity)))
        {

            if (hit.transform.tag == "Ground") // Если у ландшафта есть такой тэг, то 
            {

                selected_table.MoveUnitsTo(hit.point);

            }
            else if (hit.transform.tag == "ShipTransport") 
            { 
                selected_table.UnitsBoardShip(hit.transform.GetComponent<ShipController>());
            }
        }
    }








 // вызывается из update(); передает юниту в состоянии прицеливания при виде сверху, куда целиться
    private void PerformTerrainScan()
    {
        RaycastHit hit;
        Ray ray = CurrentCam.ScreenPointToRay(CursorVector1);

        if ((!PlayerPawn.GetComponent<PlayerStateManager>().isDead) && (PlayerPawn.GetComponent<PlayerStateManager>().RPGControlStyle) && (PlayerPawn.GetComponent<PlayerStateManager>().CurrentState == PlayerPawn.GetComponent<PlayerStateManager>().AimingState) && (Physics.Raycast(ray, out hit, Mathf.Infinity)))
        {

            if (hit.transform.tag == "Ground") // Если у ландшафта есть такой тэг, то 
            {
                PlayerPawn.GetComponent<PlayerStateManager>().hoverLocation = hit.point;
            }
        }
    }







    public bool GetRightMouseDown()
    {
        return rightMouseDown;
    }


    private void OnControlNextUnit(InputValue value)
    {


        PlayerPawn.GetComponent<PlayerStateManager>().makeUnplayable();
        PlayerPawn.GetComponent<PlayerStateManager>().ResetDestinations();
        selected_table.deselectAll();
        PlayerPawn =selected_table.GetNextPlayable(PlayerPawn, Side);
        PlayerPawn.GetComponent<PlayerStateManager>().makePlayable();
        RTSCam.GetComponent<RTSCam>().viewTarget = PlayerPawn.GetComponent<Transform>();
    }



    private void OnCamSwitch(InputValue value)
    {
        if (RTSMode == false)
        {
           PlayerPawn.GetComponent<CapsuleCollider>().enabled = true;
            RTSMode = true;
            RTSCam.enabled = true;
            FollowCam.enabled = false;
            CurrentCam = RTSCam;
        }
        else if (RTSMode == true)
        {
           PlayerPawn.GetComponent<CapsuleCollider>().enabled = false;
            RTSMode = false;
            RTSCam.enabled = false;
            FollowCam.enabled = true;
            CurrentCam = FollowCam;
        }

    }


    public void OnMouseView(InputValue value)
    {
        mouseY = value.Get<Vector2>().y;
        mouseX = value.Get<Vector2>().x;

    }

    public void OnRPGStop(InputValue value)
    {

        if ((PlayerPawn.GetComponent<PlayerStateManager>().CurrentState == PlayerPawn.GetComponent<PlayerStateManager>().IdlingState || 
            PlayerPawn.GetComponent<PlayerStateManager>().CurrentState == PlayerPawn.GetComponent<PlayerStateManager>().WalkingState) && 
            !PlayerPawn.GetComponent<PlayerStateManager>().RPGControlStyle)

        {
            Cursor.AimingFireMode = true;
            Cursor.CursorCenter();
            Ray ray = CurrentCam.ScreenPointToRay(CursorVector1);
            //PlayerPawn.GetComponent<PlayerStateManager>().AimDirection = ray.direction;
            PlayerPawn.GetComponent<PlayerStateManager>().SetRotationToCursor(ray.direction);


            PlayerPawn.GetComponent<PlayerStateManager>().transform.rotation = Quaternion.LookRotation(ray.direction, Vector3.up);
            FollowCam.transform.rotation = Quaternion.LookRotation(ray.direction, Vector3.up);
        }
        PlayerPawn.GetComponent<PlayerStateManager>().CurrentState.execOnRPGStop(value); // Вызываем текущее состояние

    }




    public void ChooseRPGCameraZoom()
    {
        if (PlayerPawn.GetComponent<PlayerStateManager>().CurrentState == PlayerPawn.GetComponent<PlayerStateManager>().CargoState)
        {
            RTSCam.GetComponent<Camera>().orthographicSize = 20;
            //Debug.Log("Cargo");
        }
        else
        {
            RTSCam.GetComponent<Camera>().orthographicSize = 8;
            //Debug.Log("NOOO Cargo");
        }
    }

        #region ДВИЖЕНИЯ КАМЕРЫ

        /// <summary>
        /// Камера в режиме Дьябло сдвигается вбок
        /// </summary>
        public void PerformCameraSlide()
    {
        if (CursorVector.x > 0.97 && RTSCam.GetComponent<RTSCam>().offset.x < 13)
        {
            RTSCam.GetComponent<RTSCam>().offset.x += RTSCam_scrollrate * Time.deltaTime;
        }
        else if (CursorVector.x < -0.97 && RTSCam.GetComponent<RTSCam>().offset.x > -13)
        {
            RTSCam.GetComponent<RTSCam>().offset.x -= RTSCam_scrollrate * Time.deltaTime;
        }
        if (CursorVector.y > 0.97 && RTSCam.GetComponent<RTSCam>().offset.z < -12)
        {
            RTSCam.GetComponent<RTSCam>().offset.z += RTSCam_scrollrate * Time.deltaTime;
        }
        else if (CursorVector.y < -0.97 && RTSCam.GetComponent<RTSCam>().offset.z > -34)
        {
            RTSCam.GetComponent<RTSCam>().offset.z -= RTSCam_scrollrate * Time.deltaTime;
        }
    }

    /// <summary>
    /// Все движения камеры в режиме TR
    /// </summary>
    public void Perform3DCameraMotion()
    {
        CamRotateValue = Vector3.zero;
        FollowCam.transform.eulerAngles = FollowCam.transform.eulerAngles + CamRotateValue * Time.deltaTime;
        if (PlayerPawn.GetComponent<PlayerStateManager>() != null)
        {
            PlayerPawn.GetComponent<PlayerStateManager>().SetCameraRotation(FollowCam.transform.rotation);
        }
        if (!PlayerPawn.GetComponent<PlayerStateManager>().isDead)
        {
            if (PlayerPawn.GetComponent<PlayerStateManager>().CurrentState != PlayerPawn.GetComponent<PlayerStateManager>().CargoState)
            {
                FollowCam.transform.position = PlayerPawn.transform.position - Vector3.forward * Mathf.Cos(FollowCam.transform.eulerAngles.y * (MathF.PI / 180f)) * 2f - Vector3.right * Mathf.Sin(FollowCam.transform.eulerAngles.y * (MathF.PI / 180f)) * 2f + Vector3.up * 2.3f;
            }
            else
            {
                FollowCam.transform.position = PlayerPawn.transform.position - Vector3.forward * Mathf.Cos(FollowCam.transform.eulerAngles.y * (MathF.PI / 180f)) * 1f - Vector3.right * Mathf.Sin(FollowCam.transform.eulerAngles.y * (MathF.PI / 180f)) * 1f + Vector3.up * 1.5f;
            }
        }
        else
        {
            FollowCam.transform.position = PlayerPawn.GetComponent<PlayerStateManager>().DeathLocation - Vector3.forward * Mathf.Cos(FollowCam.transform.eulerAngles.y * (MathF.PI / 180f)) * 2f - Vector3.right * Mathf.Sin(FollowCam.transform.eulerAngles.y * (MathF.PI / 180f)) * 2f + Vector3.up * 2.3f;
        }
    }


    public void Perform3DCameraMotionRightMouse()
    {
        //Debug.Log("выс" + FollowCam.transform.eulerAngles.x);
        //Debug.Log("mous" + mouseY);
        if ((FollowCam.transform.eulerAngles.x >= 350f && FollowCam.transform.eulerAngles.x <= 360f) || (FollowCam.transform.eulerAngles.x <= 30f && FollowCam.transform.eulerAngles.x >= 0f))
        {
            CamRotateValue.x = (0f - mouseY) * VerticalCamRotSpeed;// / 6f;
        }
        else if (FollowCam.transform.eulerAngles.x < 350f && FollowCam.transform.eulerAngles.x > 180f)
        {
            if (mouseY > 0f)
            {
                CamRotateValue.x = 0f;
            }
            else
            {
                CamRotateValue.x = (0f - mouseY) * VerticalCamRotSpeed;// / 6f;
            }
        }
        else if (FollowCam.transform.eulerAngles.x > 30f && FollowCam.transform.eulerAngles.x < 180f)
        {
            if (mouseY < 0f)
            {
                CamRotateValue.x = 0f;
            }
            else
            {
                CamRotateValue.x = (0f - mouseY) * VerticalCamRotSpeed;// / 6f;
            }
        }
        CamRotateValue.y = mouseX * HorizontalCamRotSpeed;// / 6f;
        CamRotateValue.z = 0f;
        if ((FollowCam.transform.eulerAngles + CamRotateValue * Time.deltaTime).x < 350f && (FollowCam.transform.eulerAngles + CamRotateValue * Time.deltaTime).x > 180f)
        {
            FollowCam.transform.eulerAngles = new Vector3(350f, (FollowCam.transform.eulerAngles + CamRotateValue * Time.deltaTime).y, (FollowCam.transform.eulerAngles + CamRotateValue * Time.deltaTime).z);
        }
        else if ((FollowCam.transform.eulerAngles + CamRotateValue * Time.deltaTime).x > 30f && (FollowCam.transform.eulerAngles + CamRotateValue * Time.deltaTime).x < 180f)
        {
            FollowCam.transform.eulerAngles = new Vector3(30f, (FollowCam.transform.eulerAngles + CamRotateValue * Time.deltaTime).y, (FollowCam.transform.eulerAngles + CamRotateValue * Time.deltaTime).z);
        }
        else
        {
            FollowCam.transform.eulerAngles = FollowCam.transform.eulerAngles + CamRotateValue * Time.deltaTime;
        }
        if (PlayerPawn.GetComponent<PlayerStateManager>() != null)
        {
            PlayerPawn.GetComponent<PlayerStateManager>().SetCameraRotation(FollowCam.transform.rotation);
        }
        if (!PlayerPawn.GetComponent<PlayerStateManager>().isDead)
        {
            if (PlayerPawn.GetComponent<PlayerStateManager>().CurrentState != PlayerPawn.GetComponent<PlayerStateManager>().CargoState)
            {
                FollowCam.transform.position = PlayerPawn.transform.position - Vector3.forward * Mathf.Cos(FollowCam.transform.eulerAngles.y * (MathF.PI / 180f)) * 2f - Vector3.right * Mathf.Sin(FollowCam.transform.eulerAngles.y * (MathF.PI / 180f)) * 2f + Vector3.up * 2.3f;
            }
            else
            {
                FollowCam.transform.position = PlayerPawn.transform.position - Vector3.forward * Mathf.Cos(FollowCam.transform.eulerAngles.y * (MathF.PI / 180f)) * 1f - Vector3.right * Mathf.Sin(FollowCam.transform.eulerAngles.y * (MathF.PI / 180f)) * 1f + Vector3.up * 1.5f;
            }
        }
        else
        {
            FollowCam.transform.position = PlayerPawn.GetComponent<PlayerStateManager>().DeathLocation - Vector3.forward * Mathf.Cos(FollowCam.transform.eulerAngles.y * (MathF.PI / 180f)) * 2f - Vector3.right * Mathf.Sin(FollowCam.transform.eulerAngles.y * (MathF.PI / 180f)) * 2f + Vector3.up * 2.3f;
        }
    }




    #endregion;
    // PerformCameraSlide
    // Perform3DCameraMotion

    void Update()
    {
     
        CursorVector = Cursor.GetCursorVector1();
        CursorVector1 = Cursor.GetCursorScreenPosInvert();

        if (RTSMode)
        {
            PlayerPawn.GetComponent<PlayerStateManager>().RPGControlStyle = true;
            ChooseRPGCameraZoom();

            if (leftMouseDown)
            {
                PerformTerrainClick();
            }
            PerformTerrainScan();

            PerformCameraSlide();
        }
        else
        {
            PlayerPawn.GetComponent<PlayerStateManager>().RPGControlStyle = false;
        }

        if (!RTSMode && !rightMouseDown)
        {
            Perform3DCameraMotion();
        }
        else if (!RTSMode && rightMouseDown)
        {
            Perform3DCameraMotionRightMouse();
        }

        if (!RTSMode && PlayerPawn.GetComponent<PlayerStateManager>().CurrentState== PlayerPawn.GetComponent<PlayerStateManager>().AimingState)
        {
            Perform3DCameraMotionRightMouse();
            Cursor.AimingFireMode = true;
            PlayerPawn.GetComponent<PlayerStateManager>().transform.rotation = FollowCam.transform.rotation;
        }
        else if (!RTSMode && PlayerPawn.GetComponent<PlayerStateManager>().CurrentState != PlayerPawn.GetComponent<PlayerStateManager>().AimingState)
        {
            Cursor.AimingFireMode = false;
        }
    }

}
