using UnityEngine;
using System;
using System.Linq;
using KinematicCharacterController;
using Audio;
using PlayerSounds;
using Sirenix.OdinInspector;
using System.Collections;
using PlayerAnimations;
using Unity.VisualScripting;

public class Player : Entity
{
    public event Action OnParryAttack;
    public event Action<HealthComponent.HealthModificationReport> OnDeath;
    protected event Action<int> OnBlockAttack;

    public event Action<CameraLockTarget> TargetLock;
    public event Action TargetUnlock;

    private const string _mouseXInput = "Mouse X";
    private const string _mouseYInput = "Mouse Y";
    private const string _mouseScrollInput = "Mouse ScrollWheel";
    private const string _horizontalInput = "Horizontal";
    private const string _verticalInput = "Vertical";

    private Vector3 _lookInputVector = Vector3.zero;
    private PlayerInputs _characterInputs;

    public int healItems;
    public int cureAmount;

    public Camera PlayerCamera;

    public int[] damages = { 0, 5, 20 };
    public int CurrentComboDamage => EntityStats.rawDamage + damages[Controller.attackAction.actionAmmount];
    private int damageStatMultiplier = 3;
    private int healthStatMultiplier = 10;
    private float speedStatMultiplier = 0.25f;

    public LayerMask enemyMask;

    public Vector3 velocity;

    public float MaxHp => Health.Max;
    public bool isDead => Controller.deadAction.isDoingAction;
    public bool CantMove { get; set; }
    public bool isBlocking => Controller.blockAction.isDoingAction;

    [field: SerializeField, TabGroup("References")] public CharacterCameraKCC CharacterCamera { get; protected set; }
    [field: SerializeField, TabGroup("Components")] public TargetLockingComponent TargetLocking { get; protected set; }
    [field: SerializeField, TabGroup("References")] public CharacterControllerKCC Controller { get; protected set; }

    public int maxActionAmount;

    [SerializeField, TabGroup("Inputs")] private KeyCode rollKeyCode = KeyCode.Space;
    [SerializeField, TabGroup("Inputs")] private KeyCode blockKeyCode = KeyCode.Mouse1; //Click Derecho
    [SerializeField, TabGroup("Inputs")] private KeyCode pickUpKeyCode = KeyCode.E;
    [SerializeField, TabGroup("Inputs")] private KeyCode attackKeyCode = KeyCode.Mouse0; //Click Izquierdo
    [SerializeField, TabGroup("Inputs")] private KeyCode inventoryKeyCode = KeyCode.Tab;
    [SerializeField, TabGroup("Inputs")] private KeyCode focusEnemyKeyCode = KeyCode.Q;
    [SerializeField, TabGroup("Inputs")] private KeyCode changeCameraViewKeyCode = KeyCode.C;
    [SerializeField, TabGroup("Inputs")] private KeyCode healKeyCode = KeyCode.R;

    public Transform CameraFollowPoint;

    public bool isOnFog;

    public PlayerInputs Inputs => _characterInputs;

    [SerializeField] public CharacterView view;
    
    [SerializeField] private AudioConfigurationData parrySound;

    [SerializeField] private ParticleSystem healParticles;

    #region DontDestroyOnLoad
    public static Player Instance { get; set; }
    private bool alreadyInitialized;
    [field: SerializeField, TabGroup("Components")] public InventorySystem InventoryComponent { get; private set; }
    [field: SerializeField, TabGroup("Components")] public InteractComponent InteractComponent { get; private set; }
    [field: SerializeField, TabGroup("Components")] public ParryComponent ParryComponent { get; private set; }

    private void Awake()
    {
        #region singleton
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(this);
        }
        else
        {
            if (this != Instance)
                Destroy(this.gameObject);
        }
        #endregion
    }
    protected override void InitializeStateMachine()
    {
        StateMachine = new StateMachine();
        StateMachine.AddState(States.PlayerIdle, new IdlePlayerState(this));
        StateMachine.AddState(States.PlayerInBonfire, new BonfirePlayerState(this));
        StateMachine.AddState(States.PlayerWalk, new WalkPlayerState(this));
        StateMachine.AddState(States.TargetLockWalk, new WalkWhileTargetPlayerState(this));
        StateMachine.AddState(States.PlayerRoll, new RollPlayerState(this));
        StateMachine.AddState(States.PlayerBlock, new BlockPlayerState(this));
        StateMachine.AddState(States.PlayerAttack, new AttackPlayerState(this));
        StateMachine.AddState(States.PlayerChargedAttack, new ChargedAttackPlayerState(this));
        StateMachine.AddState(States.PlayerHurt, new HurtPlayerState(this));
        StateMachine.AddState(States.PlayerDeath, new DeathPlayerState(this));
        StateMachine.AddState(States.PlayerHeal, new HealSelfState(this));
        StateMachine.AddState(States.Paused, new PauseStatePlayer(this));
        StateMachine.ChangeState(States.PlayerIdle);


        healItems = PlayerPrefs.GetInt("HealItem", 5);
        Canvas_Playing.Instance.healPotions.text = healItems.ToString();

        _characterInputs = new PlayerInputs
        {
            MoveAxisForward = Input.GetAxisRaw(_verticalInput),
            MoveAxisRight = Input.GetAxisRaw(_horizontalInput),
            CameraRotation = CharacterCamera.Transform.rotation,
            Roll = Input.GetKeyDown(rollKeyCode),
            BlockDown = Input.GetKeyDown(blockKeyCode) || Input.GetKey(blockKeyCode),
            BlockUp = Input.GetKeyUp(blockKeyCode),
            PickUp = Input.GetKeyDown(pickUpKeyCode),
            Attack = Input.GetKeyDown(attackKeyCode),
            OpenInventory = Input.GetKeyDown(inventoryKeyCode),
            CloseInventory = Input.GetKeyDown(inventoryKeyCode),
            FocusEnemy = Input.GetKeyDown(focusEnemyKeyCode),
            heal = Input.GetKeyDown(healKeyCode)
        };
    }

    protected override void UpdateStamina(float obj)
    {
        base.UpdateStamina(obj);
        
        Canvas_Playing.Instance.UpdateStamina(Stamina.StaminaPercentage);
    }

    public override void Stagger()
    {
        GameManager.ShowLog("Player Staggered");
    }
    protected override void Death(HealthComponent.HealthModificationReport obj)
    {
        base.Death(obj);

        OnDeath?.Invoke(obj);
        
        Canvas_Playing.Instance.GameEventMessage.ShowMessage("You die", Color.red, 10f);

        StateMachine.ChangeState(States.PlayerDeath);
    }
    protected override void Damage(HealthComponent.HealthModificationReport obj)
    {
        Canvas_Playing.Instance.UpdateLife(Health.CurrentPercentage);
        
        base.Damage(obj);

        if (!isDead)
        {
            StateMachine.ChangeState(States.PlayerHurt);
            CharacterCamera.StartShake();
        }
    }

    protected override void Heal(HealthComponent.HealthModificationReport obj)
    {
        Canvas_Playing.Instance.UpdateLife(Health.CurrentPercentage);

        healParticles.Play();

        //Controller.InterruptAction(CharacterState.PlayerHeal); ????

        base.Heal(obj);
    }
    private void LockCameraTarget(CameraLockTarget target)
    {
        CharacterCamera.SetCameraLockTarget(target);
        CharacterCamera.SetCameraLockState(true);
        StateMachine.ChangeState(States.TargetLockWalk);
    }
    private void UnlockCameraTarget()
    {
        StateMachine.ChangeState(States.PlayerWalk);
        CharacterCamera.SetCameraLockState(false);
    }

    public void Initialize(Action<HealthComponent.HealthModificationReport> _onDeath, Transform lastRegisterPoint)
    {
        if (alreadyInitialized) return;
        
        Motor.SetPositionAndRotation(lastRegisterPoint.position, lastRegisterPoint.rotation);
        
        Initialize();
        
        OnDeath = _onDeath;

        TargetLocking.Initialize(this, LockCameraTarget, UnlockCameraTarget);

        Canvas_Inventory.Instance.onUpgradeStat += UpgradeStats;
        Canvas_Inventory.Instance.onRefreshPoints += (() =>
        {
            EntityStats.ResetValues(); 
        });
        EntityStats.AddToMaxHealth(Canvas_Inventory.Instance.GetSavedHealthPoints() * healthStatMultiplier);
        Canvas_Playing.Instance.UpdateLife(MaxHp);
        EntityStats.AddDamage(Canvas_Inventory.Instance.GetSavedDamagePoints() * damageStatMultiplier);
        EntityStats.AddWalkSpeed(Canvas_Inventory.Instance.GetSavedSpeedPoints() * speedStatMultiplier);

        Cursor.lockState = CursorLockMode.Locked;


        // Tell camera to follow transform
        CharacterCamera.SetFollowTransform(CameraFollowPoint);

        // Ignore the character's collider(s) for camera obstruction checks
        CharacterCamera.IgnoredColliders = Controller.GetComponentsInChildren<Collider>().ToList();
        #endregion

        // view = GetComponentInChildren<CharacterView>(); ideal
    
        alreadyInitialized = true;
    }

    protected override void Update()
    {
        base.Update();
        //Cambiar a otro lugar
        if (Input.GetKeyDown(changeCameraViewKeyCode))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        if(StateMachine != null && StateMachine.GetCurrentState() != States.Paused) UpdateInputs();
        //character.SetInputs(ref _characterInputs); //Cambiar la logica de aca adentro a la fsm.
    }

    [SerializeField] float attackTimeThreshold = 0.8f; // Umbral de tiempo para considerar un click sostenido
    float timeSinceAttackDown = 0;

    bool isPreparingCharge=true,beginToAttack;

    public void ChangeState(States state) => StateMachine.ChangeState(state);

    public void SetInputs(PlayerInputs newInputs,float time)
    {
        _characterInputs = newInputs;
        Controller.Walk(_characterInputs, time);
    }

    private void UpdateInputs()
    {
        if (isOnFog || CantMove)
        {
            _characterInputs = new PlayerInputs
            {
                MoveAxisForward = 0,
                MoveAxisRight = 0,
                CameraRotation = Quaternion.identity,
                Roll = false,
                BlockDown = false,
                BlockUp = false,
                PickUp = false,
                Attack = false,
                OpenInventory = false,
                CloseInventory = false,
                FocusEnemy = false
            };
            Controller.Walk(_characterInputs, Time.deltaTime);
            return;
        }

        _characterInputs.MoveAxisForward = Input.GetAxisRaw(_verticalInput);
        _characterInputs.MoveAxisRight = Input.GetAxisRaw(_horizontalInput);
        _characterInputs.CameraRotation = CharacterCamera.Transform.rotation;
        _characterInputs.Roll = Input.GetKeyDown(rollKeyCode);
        _characterInputs.BlockDown = Input.GetKeyDown(blockKeyCode) || Input.GetKey(blockKeyCode);
        _characterInputs.BlockUp = Input.GetKeyUp(blockKeyCode);
        _characterInputs.PickUp = Input.GetKeyDown(pickUpKeyCode);
        _characterInputs.OpenInventory = Input.GetKeyDown(inventoryKeyCode);
        _characterInputs.CloseInventory = Input.GetKeyDown(inventoryKeyCode);
        _characterInputs.FocusEnemy = Input.GetKeyDown(focusEnemyKeyCode);
        _characterInputs.heal= Input.GetKeyDown(healKeyCode);
        _characterInputs.AttackDown = Input.GetKeyDown(attackKeyCode);

        if (_characterInputs.AttackDown)
        {
            isPreparingCharge = false;
            beginToAttack = true;
            timeSinceAttackDown = attackTimeThreshold;
        }

        if (timeSinceAttackDown >= 0)
            timeSinceAttackDown -= Time.deltaTime;

        bool canChargedAttack = beginToAttack && timeSinceAttackDown <= 0;

        if (Input.GetKeyUp(attackKeyCode))
        {
            if (canChargedAttack)
            {
                timeSinceAttackDown = 0;
                _characterInputs.Attack = false;
                _characterInputs.ChargedAttack = true;
            }
            else
            {
                _characterInputs.Attack = true;
                _characterInputs.ChargedAttack = false;
            }
            beginToAttack = false;
            canChargedAttack = false;
            view.chargeWeaponParticle.Stop();
            timeSinceAttackDown = 0;
        }
        else
        {
            _characterInputs.Attack = false;
            _characterInputs.ChargedAttack = false;
        }

        if (canChargedAttack)
        {
            float clampValue = 0.5f;
            _characterInputs.MoveAxisForward = Mathf.Clamp(_characterInputs.MoveAxisForward, -clampValue, clampValue);
            _characterInputs.MoveAxisRight = Mathf.Clamp(_characterInputs.MoveAxisRight, -clampValue, clampValue);

            if (!isPreparingCharge)
            {
                isPreparingCharge = true;
                view.chargeWeaponParticle.Play();
                view.chargeWeaponSound.Play2D();
            }
            Controller.Walk(_characterInputs, Time.deltaTime);
        }

        if (_characterInputs.FocusEnemy)
        {
            if (TargetLocking.LockingActive) TargetLocking.Unlock();
            else TargetLocking.Lock();
        }
    }

    private void LateUpdate() => HandleCameraInput();

    private void HandleCameraInput()
    {
        // Create the look input vector for the camera
        float mouseLookAxisUp = Input.GetAxisRaw("Mouse Y");
        float mouseLookAxisRight = Input.GetAxisRaw("Mouse X");
        _lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

        // Prevent moving the camera while the cursor isn't locked
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            _lookInputVector = Vector3.zero;
        }

        
//TODO: COMENTO PARA SACAR EL ZOOM DE CAMARA HORRIBLE
        // Input for zooming the camera (disabled in WebGL because it can cause problems)
        // float scrollInput = -Input.GetAxis("Mouse ScrollWheel");
        float scrollInput = 0;
#if UNITY_WEBGL
            scrollInput = 0f;
#endif

        // Apply inputs to the camera
        CharacterCamera.UpdateWithInput(Time.deltaTime, scrollInput, _lookInputVector);

        // Handle toggling zoom level
        if (Input.GetKeyDown(changeCameraViewKeyCode))
        {
            CharacterCamera.TargetDistance = (CharacterCamera.TargetDistance == 0f) ? CharacterCamera.DefaultDistance : 0f;
        }
    }

    private void HandleCharacterInput()
    {

        //GameManager.ShowLog(Vector3.ClampMagnitude(new Vector3(characterInputs.MoveAxisRight, 0f, characterInputs.MoveAxisForward), 1f));
        // Apply inputs to character
        Controller.SetInputs(ref _characterInputs);

        /*if (Input.GetKeyDown(rollKeyCode))
        {
            character.motor.ForceUnground(0.1f);
            character.AddVelocity(Vector3.one * 10f);
        }*/
    }

    private void UpgradeStats(StatType st)
    {
        switch (st)
        {
            case StatType.Health:
                EntityStats.AddToMaxHealth(healthStatMultiplier);
                Canvas_Playing.Instance.UpdateLife(MaxHp);

                GameManager.ShowLog($"Stat {st} ,value {EntityStats.HealthComponent.Current}");
                break;
            case StatType.Damage:
                EntityStats.AddDamage(damageStatMultiplier);

                GameManager.ShowLog($"Stat {st} ,value {EntityStats.rawDamage}");
                break;
            case StatType.Speed:
                EntityStats.AddWalkSpeed(speedStatMultiplier);

                GameManager.ShowLog($"Stat {st} ,value {EntityStats.rawWalkSpeed}");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(st), st, null);
        }

        Canvas_Inventory.Instance.AddUsedPoint();
    }
    public void EnableDamage() => Health.SetInvulnerability(false);
    public void DisableDamage() => Health.SetInvulnerability(true);

    public void OnDamageEntity(Entity entity)
    {
        view.Play2DAudio(Sounds.soundBlock);
        ParticlesManager.Instance.SpawnParticles(entity.transform.position, entity.hurtParticle);
    }

    public void SlowDownTime()
    {
        Time.timeScale = 0.2f;
        StartCoroutine(ProgresiveAddTimescale());
    }

    IEnumerator ProgresiveAddTimescale()
    {
        while (Time.timeScale <= 1f)
        {
            Time.timeScale += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void SitInBonfire()
    {
        EnemyManager.Instance.ResetEnemies();

        //if (Canvas_Inventory.Instance != null)
        //    Canvas_Inventory.Instance.SavePoints();//save

        Health.FullHeal();
        RestoreHealItem();//bottles

        StateMachine.ChangeState(States.PlayerInBonfire);
    }
    public void SetFallDeathCamera(Transform point) => CharacterCamera.SetFollowTransform(point, true);
    
    public void Parry()
    {
        GameManager.ShowLog("Player Successfully parry!");
        
        parrySound.Play2D();
        SlowDownTime();
        OnParryAttack?.Invoke();
    }
    
    public void BlockAttack(int amount)
    {
        GameManager.ShowLog("Player Successfully block an attack!");

        Stamina.ConsumeStamina(amount);
        
        view.Play2DAudio(Sounds.soundBlock);
        OnBlockAttack?.Invoke(amount);
    }
    public void UpdateHealItem(int newHealItems,bool updateSave=false)
    {
        healItems += newHealItems;
        Canvas_Playing.Instance.healPotions.text = healItems.ToString();

        if(updateSave)
            PlayerPrefs.SetInt("HealItem",PlayerPrefs.GetInt("HealItem",5)+ newHealItems);
    }
    public void RestoreHealItem()
    {
        healItems = PlayerPrefs.GetInt("HealItem", 5);
        Canvas_Playing.Instance.healPotions.text = healItems.ToString();
    }
    //used to find all particles and pause it(full mal optimizado pero bueno)
    public ParticleSystem[] FindParticles()
        => FindObjectsOfType<ParticleSystem>().Where(x => x.isPlaying).ToArray();
}