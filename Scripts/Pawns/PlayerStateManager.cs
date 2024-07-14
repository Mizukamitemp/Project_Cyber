//using JetBrains.Annotations;
//using System;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;


public class PlayerStateManager : PlayerStateBase
{
    #region Параметры юнитов

    [Header("Unit Characteristics")]
    [SerializeField] private int _hitpoints = 3;    public int HitPoints { get { return _hitpoints; } }
    [SerializeField] private int _maxhitpoints = 3; public int MaxHitPoints { get { return _maxhitpoints; } }
    [SerializeField] private int _fatigue = 100000;         public int Fatigue { get { return _fatigue; } }
    [SerializeField] private int _maxfatigue = 100000;      public int MaxFatigue { get { return _maxfatigue; } }
    // Утомляемость, или "энергия". Снижаем и повышаем с помощью DecreaseFatigue

    [SerializeField] private float _meleerange = 50;    public float MeleeRange { get { return _meleerange; } }
    // Данность срабатывания атаки у AI
    [SerializeField] private int _meleedamage = 1;      public int MeleeDamage { get { return _meleedamage; } }
    // Просто повреждение, наносимое при попадании
    [SerializeField] private float _walkspeed = 5f;     public float WalkSpeed { get { return _walkspeed; } }
    // Используется для NavMeshAgent и CharacterController

    [SerializeField] private float _rotatespeed = 5f;   public float RotateSpeed { get { return _rotatespeed; } }
    // Используется для поворота AI в ShootingAIState
    [SerializeField] private float _attackrate = 0.5f;  public float AttackRate { get { return _attackrate; } }
    // Скорострельность AI, используется в ShootingAIState

    private Vector3 _gravityVector = new Vector3(0, -9.8f, 0);
    // Применяем к юниту под контролем игрока

    [SerializeField] private bool _isstationaryalways = false;           public bool IsStationaryAlways { get { return _isstationaryalways; } }
    //является стационарной башней
    private bool _isstationary = false;                 public bool IsStationary { get { return _isstationary; } set { _isstationary = value; } }
    //имеет приказ держать местность

    #endregion

    #region Поведение юнитов
    [Header("Unit Behaviour")]

    [SerializeField] private int _side = 0;                public int Side { get {return _side;} set {_side = value;} }
    //Сторона, за которую играет
    [SerializeField] private bool _isplayable = true;      public bool IsPlayable { get { return _isplayable; } set { _isplayable = value; } }
    // Можно играть за него, заменяет //public bool bPlayable = true; 
    private bool _isinplayersarmy = false;                 public bool IsInPlayersArmy { get { return _isinplayersarmy; } set { _isinplayersarmy = value; } }
    // Находится уже в армии игрока или действует сам
    private int _unitnumber = 0;                           public int UnitNumber { get { return _unitnumber; } set { _unitnumber = value; } }
    // Номер юнита в ЭТОЙ команде, 1-100; влияет на иконку на UI

    [SerializeField] private bool _ishiding = true;        public bool IsHiding { get { return _ishiding; } set { _ishiding = value; } }
    // юнит будет пытаться прятаться после потери обзора на игрока
    private bool _ishidden = false;                        public bool IsHidden { get { return _ishidden; } set { _ishidden = value; } }
    // мы добежали до точки пряток и не хотим бежать еще раз, пока не встретились с игроком
    private bool _hashidingpoint = false;                  public bool HasHidingPoint { get { return _hashidingpoint; } set { _hashidingpoint = value; } }
    // Юнит выбрал точку чтобы туда прятаться
    [SerializeField] private int _agrdistance = 100;       public int AgrDistance { get { return _agrdistance; } set { _agrdistance = value; } }
    // Дистанция, на которой юнит агрится на врага. Дистанция детекта.

    [SerializeField] private bool _isguardingthatpoint = true; public bool IsGuardingThatPoint { get { return _isguardingthatpoint; } set { _isguardingthatpoint = value; } }
    // Потеряв цель, возвращается на эту точку, а не остается где попало

    [SerializeField] private int _pursuedistance = 1000;       public int PursueDistance { get { return _pursuedistance; } set { _pursuedistance = value; } }
    // Будет преследовать цель на это расстояние

    //    private Vector3 _guardlocation = new Vector3(0, 0, 0);     public Vector3 GuardLocation { get { return _guardlocation; } set { _guardlocation = value; } }
    private Vector3 _guardlocation = new Vector3(0, 0, 0); public Vector3 GuardLocation { get { return _guardlocation; } set { _guardlocation = value; } }
    //где именно он сторожит, куда возвращается. Нельзя использовать GuardLocation.x= 

    public float timeWithoutTarget = 0; // мы столько секунд не видим цель
    public float timeToForgetTarget = 10; // по истечение срока нам сбрасывают цель

    // Патруль
    [SerializeField] private PatrolPt _nextpatrol;
    public PatrolPt NextPatrol { get { return _nextpatrol; } set { _nextpatrol = value; } }

    // Приказ передвижения
    public bool hasMoveOrders = false; //если есть приказ итти то не надо возвращаться на пост охраны
    public Vector3 moveDestination = new Vector3(0, 0, 0); //куда дали команду итти
    public bool IgnoreWhenMoving = false; 

    // Прочие приказы
    public bool StandingGround = false; // Игрок приказал держать местность
    public GameObject MYCommander; // Знать командира чтобы посчитать построение относительно него
    public Vector2 FormationOffset = new Vector2(0, 0); // Координаты сдвига для построения
    public bool isInFormation = false; // Игрок приказал запомнить построение
                                       //public Vector3 FormationDestination; // Назначение передвижения с поправкой на построение
 
    public bool hasTarget = false;
    public bool lostTarget = false;
    public GameObject MYTarget;
    [HideInInspector] public GameObject LockTarget;



    #endregion

    #region Отслеживание Состояния юнита
    [Header("Unit Current State")]

    [HideInInspector] public float timer = 0; // Используется в update чтобы некоторые события срабатывали не каждый цикл, а реже
    [HideInInspector] public bool isBeingPlayed = false;  // В данный момент за него играют
    [HideInInspector] public bool isDead = false;   //для простоты - умираем только раз
    [HideInInspector] public Vector3 DeathLocation;   // Место, где юнит сдох - для камеры
    [HideInInspector] public bool bSelected = false; // Является выделенным?

    #endregion

    #region Player Movement
    [Header("Movement Mechanics")]

    [HideInInspector] public Vector2 InputVector; //движение стрелками передается с PLayerController сюда
    [HideInInspector] public Quaternion ReceivedRotation; // Мы получили угол поворота с камеры и будем туда поворачивать игрока
    [HideInInspector] public bool RPGControlStyle = false;
    // Диабло режим
    [HideInInspector] public bool RPGControlStopped = false;
    [HideInInspector] public bool RPGControlWasStopped = false;
    [HideInInspector] public Vector3  clickLocation;   // кликнули по ландшафту
    [HideInInspector] public Vector3 hoverLocation;   // Над этим местом висит курсор в режиме Диабло с зажатым шифтом

    #endregion

    #region State Machines 

    public WalkState WalkingState = new WalkState();
    public IdleState IdlingState = new IdleState();
    public FallState FallingState = new FallState();
    public DeadState DeathState = new DeadState();
    public DrowningState DrownState = new DrowningState();
    public AttackState AttState = new AttackState();
    public BlockState BlockingState = new BlockState();
    public PassengerState CargoState = new PassengerState();
    public AimState AimingState = new AimState(); //целимся перед выстрелом
    public PlayerBaseState CurrentState;

    public IdleAIState IdleAIState = new IdleAIState();
    public AttackAIState AttackAIState = new AttackAIState();
    public ShootingAIState ShootAIState = new ShootingAIState();
    public PriorityMoveAIState PriorityMoveAIState = new PriorityMoveAIState();
    public NoAIState NoAIState = new NoAIState();
    public PlayerBaseAIState CurrentAIState;
    #endregion

    #region Player Components
    [Header("Components")]
    public GameObject Model;
    public GameObject EditorModel;
    [HideInInspector] public GameObject ModelInstance;
    [HideInInspector] public CharacterController Controller;
    [HideInInspector] public NavMeshAgent myNavMesh;
    [HideInInspector] public Animator Anim;
    public Sprite charPortrait;
    public GameObject BlockEffect;
    public GameObject BloodEffect;

    public GameObject ForceField;
    public GameObject ForceFieldInstance;

    public Material MaterialEnemy;
    public Material MaterialSelected;

    [HideInInspector] public GameObject SelectObject; // Это объект, который был инстанциирован
    public GameObject SelectObjectPrefab;


    [HideInInspector] public GameObject FigureCenter; // Проверяем попадания именно сюда, и видимость именно отсюда, и тонем если оно в воде  ! Это берется с экземпляра!
    [HideInInspector] public GameObject Gun1; //Для определения ее положения
    public GameObject PlasmaProj; // Это ссылка на снаряд, который мы пускаем


    #endregion

    #region Cargo
    [Header("In Cargo")]
    [HideInInspector] public ShipController myTransport;  // Прямая ссылка на корабль, в котором мы плывем
    [HideInInspector] public GameObject CargoLocation;        // Точка на корабле, к которой привязан игрок
    [HideInInspector] public Vector3 UnboardLocation; //Сюда выгружаем с корабля

    #endregion

    [Header("Sounds")]
    private AudioSource _audioplayer;
    [SerializeField] private AudioClip ShotSound;
    [SerializeField] private AudioClip Voice_Roger;
    [SerializeField] private AudioClip Voice_Hurt;
    [SerializeField] private AudioClip Voice_Death;
    [SerializeField] private AudioClip Voice_Selected;


    // Базовые методы

    void Start()
    {
        Physics.IgnoreLayerCollision(11, 12);
    }

    public virtual void Awake()
    {
        //NavMesh.RemoveAllNavMeshData();
        ModelInstance = Instantiate(Model, gameObject.transform);   // Это мы создали префаб модели
        Anim = ModelInstance.GetComponent<Animator>();      // Аниматор берем с экземпляра модели, а не с префаба Model

        ForceFieldInstance = Instantiate(ForceField, gameObject.transform);   // Это мы создали префаб поля, которое включается в состоянии блока

        SelectObject = Instantiate(SelectObjectPrefab, gameObject.transform);   // Это мы создали префаб эффекта выбора

        Controller = GetComponent<CharacterController>();
        myNavMesh = GetComponent<NavMeshAgent>();

        EditorModel.SetActive(false);//GetComponent<Renderer>().enabled = false;
       
        FigureCenter = ModelInstance.GetComponent<ModelScript>().FigureCenterObject;
        Gun1 = ModelInstance.GetComponent<ModelScript>().WeaponObject;

        GetComponent<Rigidbody>().isKinematic = true;//RB - для выделения и чтобы тонуть только
        myNavMesh.speed = WalkSpeed;
        GuardLocation = transform.position; //Если мы сторожим, то сторожим эту точку изначально
                                            //clickLocation = transform.position;

        CurrentState = IdlingState;
        CurrentState.EnterState(this);
        // Exported from Start


        //Debug.Log("Start Desu");
        CurrentAIState = IdleAIState;
        IdleAIState.EnterAIState(this);

        _audioplayer = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip sound)
    {
        if (_audioplayer != null)
        {
            _audioplayer.PlayOneShot(sound);
        }
    }

    public void PlaySound_Fire()
    {
        if (_audioplayer != null && ShotSound !=null)
        {
            _audioplayer.PlayOneShot(ShotSound);
        }
    }

    void FixedUpdate()
    {
    

        //if (!isBeingPlayed && agrDistance==66) Debug.Log(CurrentAIState);
        timer += Time.fixedDeltaTime;

        // CheckAIMoving();

        timeWithoutTarget += Time.fixedDeltaTime;
        

        if (CurrentState != CargoState)
        {
            ApplyGravity();
        }

        CurrentState.UpdateState(this);

        CurrentAIState.UpdateAIState(this);

        CheckSurface();

        if (LockTarget != null && LockTarget.GetComponent<PlayerStateManager>().isDead)
        {
            LockTarget = null;
        }

        //Debug.Log($"Fatigue: " + fatigue);
        

    }





    public bool DecreaseFatigue(int value)
    {
 
            if (value > 0)  //уменьшаем
            {
                if (_fatigue - value < 0) return false;
                else _fatigue -= value;
            }
            else if (value < 0) //увеличиваем - хначит надо обратить знак
            {
                value = -1 * value;
                if (_fatigue < _maxfatigue)
                {
                    _fatigue += value;
                    if (_fatigue > _maxfatigue) _fatigue = _maxfatigue;
                }

            }

        return true;

    }
    // Cyber

    public void MaybeDisableAi()    // Проверяем, не сдох ли ии пока чтото делал
    {
        if (!isDead && isBeingPlayed)
        {
            SwitchAIState(NoAIState);
        }
        else if (isDead)
        {
            SwitchAIState(NoAIState);
        }
    }





    /// <summary>
    /// Поворачиваем игрока не на центр экран, а в направлении, где был курсор
    /// </summary>
    /// <param name="rot"></param>
public void SetRotationToCursor(Vector3 rot)
    {
        transform.rotation = Quaternion.FromToRotation(transform.forward, rot);

        //Debug.Log($"Axis X" + rot.x);
        //Debug.Log($"Axis Y" + rot.y);
        //Debug.Log($"Axis Z" + rot.z);

    }



    // Прочие методы


    #region РЕЖИМ ЮНИТА: Игрок/AI, Cargo/Выгрузка



    public void makeUnplayable()
    {
        myNavMesh.enabled = true;
        Controller.enabled = false;
        isBeingPlayed = false;
        InputVector = Vector2.zero;
        GetComponent<CapsuleCollider>().enabled = true;


       




        //if (CurrentState != IdlingState) SwitchState(IdlingState);
        SwitchAIState(IdleAIState);
    }

    public void makePlayable()
    {
        myNavMesh.enabled = false;
        Controller.enabled = true;
        isBeingPlayed = true;
        clickLocation = transform.position;

        if (RPGControlStyle)
        {
            GetComponent<CapsuleCollider>().enabled = false;
        }
        else
        {
            GetComponent<CapsuleCollider>().enabled = false;
        }

        //if (CurrentState != IdlingState) SwitchState(IdlingState);
        SwitchAIState(NoAIState);
    }





    public void GetIntoShip(ShipController ship)
    {
        if (CurrentState != CargoState)
        { 
        myTransport = ship;
        SwitchState(CargoState);
        }
    }

    public void Unboard(Vector3 loc)
    {
        UnboardLocation = loc;
        //SwitchState(IdlingState);
        //Debug.Log("----------------------------------------123");
    }

    public void SwitchCargoStation(GameObject station)
    {
        CargoLocation = station;

        station.GetComponent<ShipStation>().IsOccupied = true;
        station.GetComponent<CapsuleCollider>().enabled = false;

    }




    #endregion
    // makeUnplayable
    // makePlayable
    // GetIntoShip
    // SwitchCargoStation
    // Unboard


    #region СОСТОЯНИЯ ЮНИТА



    public void SwitchState(PlayerBaseState state)
    {
        if (CurrentState != DeathState)
        {
            CurrentState.ExitState(this);
            CurrentState = state;
            state.EnterState(this);
        }

    }

    public void SwitchAIState(PlayerBaseAIState state)
    {
        CurrentAIState.ExitAIState(this);
        CurrentAIState = state;
        state.EnterAIState(this);
    }
 

    public void Select()
    {
        PlaySound(Voice_Selected);
        SelectObject.GetComponent<Renderer>().enabled = true;
        bSelected = true;
        IsInPlayersArmy = true;
    }

    public void Deselect()
    {
        SelectObject.GetComponent<Renderer>().enabled = false;
        bSelected = false;
    }

    public GameObject GetLockTarget()
    {
        if (LockTarget != null)
        {
            return LockTarget;
        }
        return null;
    }

    public void SetLockTarget(GameObject receivetarget)
    {
        if (LockTarget != null)
        {
            LockTarget.GetComponent<PlayerStateManager>().UnmakeLockTarget();
        }
        LockTarget = receivetarget;
        LockTarget.GetComponent<PlayerStateManager>().MakeLockTarget();
    }

    public void ClearLockTarget()
    {
        if (LockTarget != null)
        {
            LockTarget.GetComponent<PlayerStateManager>().UnmakeLockTarget();
            LockTarget = null;
        }
    }

    public void UnmakeLockTarget()
    {
        if (bSelected)
        {
            SelectObject.GetComponent<Renderer>().enabled = true;
        }
        else
        {
            SelectObject.GetComponent<Renderer>().enabled = false;
        }
        SelectObject.GetComponent<Renderer>().material = MaterialSelected;
    }

    public void MakeLockTarget()
    {
        SelectObject.GetComponent<Renderer>().enabled = true;
        SelectObject.GetComponent<Renderer>().material = MaterialEnemy;
    }


    #endregion
    // SwitchState
    // SwitchAIState
    // MakeDead
    // Select
    // Deselect


    #region БОЕВАЯ СИСТЕМА




    public bool IsBlocking()
    {
        if (CurrentState == BlockingState)
        {
            return true;
        }
        return false;
    }

    public void TakeDamage(int damage)
    {
        if (CurrentState != BlockingState)
        {

            if (_hitpoints > 0)
            {
                PlaySound(Voice_Hurt);
                _hitpoints--;
            }

            if (_hitpoints < 0)
            {
                _hitpoints = 0;
            }

            if (_hitpoints <= 0)
            {
                PlaySound(Voice_Death);
                SwitchState(DeathState);
            }

        }
    }
    #endregion
    // PerformAttack
    // PerformBlock
    // IsBlocking
    // TakeDamage




    #region ПРЯМОЙ КОНТРОЛЬ ДВИЖЕНИЯ ЮНИТА

    void CheckSurface()
    {
        RaycastHit hit;

        if (FigureCenter != null)
        {

            if (Physics.Raycast(transform.position, -transform.up, out hit, 0.5f)) // + new Vector3(0, MeleeOffsetY)//FigureCenter
            {
               // Debug.Log("TEST");
               //Debug.Log($"we hit Z" + hit.transform.gameObject.name);
                if (hit.transform.tag == "WaterBase")
                {
                    DeathLocation = transform.position;
                    SwitchState(DrownState);
                }
            }
        }
    }

    public void ApplyGravity()

    {
        if (isBeingPlayed && Controller.enabled) // Гравитацию на CharacterController - ЕСЛИ это игрок! Иначе пусть NavMesh разбирается
        {
            Controller.Move(_gravityVector * Time.deltaTime);
        }
    }

    /// <summary>
    /// Мы получаем направление игрока откуда? С камеры, ежесекундно, с помощью PlayerController
    /// </summary>
    /// <param name="newrotation"></param>
    public void SetCameraRotation(Quaternion newrotation)
    {
        ReceivedRotation = newrotation;
    }

    #endregion
    // doShipMovementRoutine
    // CheckSurface
    // ApplyGravity





    #region AI: GUARD
    public bool checkAwayFromGuardPoint()
    // Возвращается ли чел на точку которую сторожит
    {
        Vector3 FormationDestination;


        FormationDestination.x = GuardLocation.x - FormationOffset.x;
        FormationDestination.z = GuardLocation.z - FormationOffset.y;
        FormationDestination.y = GuardLocation.y;
        
        if (!isInFormation && GetUnitDistance(transform.position, GuardLocation).x<0.5 && GetUnitDistance(transform.position, GuardLocation).y < 2)   // было 0.8
        {
            //Debug.Log($"GetUnitDistance x" + GetUnitDistance(transform.position, guardLocation).x);
           // Debug.Log($"GetUnitDistance y" + GetUnitDistance(transform.position, guardLocation).y);
            hasMoveOrders = false;
            return false;
        }
        else if (isInFormation && GetUnitDistance(transform.position, FormationDestination).x < 0.5 && GetUnitDistance(transform.position, FormationDestination).y < 2)
        {
            hasMoveOrders = false;
            return false;
        }
        else
        {
            //Debug.Log($"GetUnitDistance x" + GetUnitDistance(transform.position, guardLocation).x);
            //Debug.Log($"GetUnitDistance y" + GetUnitDistance(transform.position, guardLocation).y);
            return true;
        }
    }
    public bool checkExceededPursuitDistance()
    // Возвращается ли чел на точку которую сторожит
    {
        if (Vector3.Distance(transform.position, GuardLocation) > PursueDistance)
        {
            return true;
        }
        else return false;
    }

    #endregion
    // GetUnitDistance
    // checkAwayFromGuardPoint
    // checkExceededPursuitDistance

    #region AI: FORMATION

    public void StartFormationRelative()
    {
        if (MYCommander != null)
        {
            PlaySound(Voice_Roger);
            FormationOffset.x = MYCommander.transform.position.x - transform.position.x;
            FormationOffset.y = MYCommander.transform.position.z - transform.position.z;

            Vector3 GuardLocationCopy = GuardLocation;  // это имеет отношение к тому, что GuardLocation - это проперти
            GuardLocationCopy.x = transform.position.x + FormationOffset.x;
            GuardLocationCopy.z = transform.position.z + FormationOffset.y;

            GuardLocation = GuardLocationCopy;


            //Debug.Log($"FORMATION OFFSET X:" + FormationOffset.x);
            //Debug.Log($"FORMATION OFFSET Y:" + FormationOffset.y);
            isInFormation = true;
        }
    }

    public void TerminateFormation()
    {
        PlaySound(Voice_Roger);


        Vector3 GuardLocationCopy = GuardLocation;  // это имеет отношение к тому, что GuardLocation - это проперти

        GuardLocationCopy.x = GuardLocation.x - FormationOffset.x;
        GuardLocationCopy.y = GuardLocation.z - FormationOffset.y;
        GuardLocation = GuardLocationCopy;

        FormationOffset.x = 0;
        FormationOffset.y = 0;
        isInFormation = false;
    }



    public void FindCommander()
    {
        GameObject[] allUnits;
        allUnits = GameObject.FindGameObjectsWithTag("SelectableUnit");
        
        Debug.Log("start");
        foreach (GameObject go in allUnits)
        {
            Debug.Log(go.name);
            Debug.Log("starting cycle");
            if (go.GetComponent<PlayerStateManager>().isBeingPlayed)
            {
                Debug.Log("found one");
                MYCommander = go;
            }
        }
        Debug.Log("end");

    }

    public bool GetIsInFormation()
    {

        return isInFormation;
    }

    #endregion
    // StartFormationRelative
    // TerminateFormation
    // FindCommander
    // GetIsInFormation


    #region AI STAND GROUND

    public bool GetStandGround()
    {
        //Debug.Log($"CALLED AI, STAND GROUND:" + StandingGround);
        return IsStationary;
    }
    public void StandGroundOn()
    {
        //StandingGround = true;
        PlaySound(Voice_Roger);
        IsStationary = true;

    }
    public void StandGroundOff()
    {
        //StandingGround = false;
        if (!IsStationaryAlways) //если не является изначально башней
        {
            PlaySound(Voice_Roger);
            IsStationary = false;
        }
        
    }


    #endregion
    // GetStandGround
    // StandGroundOn
    // StandGroundOff


    #region ПРИКАЗ ДВИЖЕНИЯ

    public void MoveOrders(Vector3 DestinationCoord)
    {
        //Debug.Log("moveto");
        PlaySound(Voice_Roger);
        hasMoveOrders = true;
        moveDestination = DestinationCoord;
        GuardLocation = DestinationCoord;
    }

    public void MoveTo(Vector3 DestinationCoord)
    {
        moveDestination = DestinationCoord;
    }
    public void MoveModeAggr()
    {
        PlaySound(Voice_Roger);
        IgnoreWhenMoving = false;
    }

    public void MoveModePass()
    {
        PlaySound(Voice_Roger);
        IgnoreWhenMoving = true;
    }

    public bool GetIgnoreWhenMoving()
    {
        // Debug.Log($"CALLED AI, IGNORE:" + IgnoreWhenMoving);
        return IgnoreWhenMoving;
    }

    public void ResetDestinations()
    {
        PlaySound(Voice_Roger);
        GuardLocation = transform.position;
        moveDestination = transform.position;
        hasMoveOrders = false;
    }

    #endregion
    // MoveOrders
    // MoveTo
    // MoveModeAggr
    // MoveModePass
    // GetIgnoreWhenMoving
    // ResetDestinations


    #region ПРОВЕРКИ СОСТОЯНИЯ AI
    public bool HasTarget()
    {
        if (hasTarget && MYTarget != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Check if unit can be given move commands: is alive, has navmesh agent etc.
    /// </summary>
    /// <returns></returns>
    public bool CanBeCommanded()
    {
        if ((isDead == false) && (myNavMesh.enabled == true))
        {
            return true;
        }
        return false;
    }

    #endregion
    // HasTarget
    // CanBeCommanded






































}
