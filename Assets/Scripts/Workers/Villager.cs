using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SG_Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = System.Random;

[RequireComponent(typeof(VillagerStats))]
public class Villager : MonoBehaviour, IInteractable
{
    [SerializeField] private List<GameObject> monsters;
    [SerializeField] private GameObject nearestObject;
    [SerializeField] private List<GameObject> objInTriggerZone;
    [SerializeField] private List<GameObject> objInAwarenessZone;
    public GameObject target;
    private bool attackStarted;
    private bool finding;
    public bool ignoreQueue;
    [SerializeField] private float distance;


    /// <summary>
    /// The Villagers Role will give the Villager boosted stats in a specific craft as well as more abilities linked to that craft.
    /// </summary>
    private Roles _villagerRole;
    public Roles CurrentRole
    {
        get => _villagerRole;
        set
        {
            if (SceneManager.GetActiveScene().name == "New Scene")
            {
                Level.AddToVillagerLog(this, $"{_villagerStats.VillagerName} Changed From {_villagerRole} To {value}");
            }

            _villagerRole = value;

            Debug.Log($"{_villagerStats.VillagerName} Changed To {_villagerRole}");

            switch (_villagerRole)
            {
                case Roles.Default:
                    break;
                case Roles.Farmer:
                    break;
                case Roles.Fighter:
                    break;
                case Roles.Lumberjack:
                    if (_gameManager.level.tutorialManager.TutorialStage is TutorialStage.VillagerManagementTutorial)
                    {
                        _gameManager.level.tutorialManager.TutorialStage = TutorialStage.BuildingTutorial;
                    }
                    break;
                case Roles.Miner:
                    break;
                case Roles.Crafter:
                    break;
                case Roles.Leader:
                    _villagerStats.Strength = 6;
                    _villagerStats.Craft = 6;
                    _villagerStats.Magic = 6;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    /// <summary>
    /// The Villagers Current State
    /// </summary>
    private VillagerStates _currentState;
    public VillagerStates CurrentState
    {
        get => _currentState;
        set
        {
            _currentState = value;

            switch (_currentState)
            {
                case VillagerStates.Idle:
                    _animator.Play("Idle");
                    break;
                case VillagerStates.Working:
                    Debug.Log("Switching");
                    GetAnimationForRole();
                    break;
                case VillagerStates.Sleeping:
                    break;
                case VillagerStates.Walking:
                    agent.isStopped = false;
                    Debug.Log($"Agent is stopped? {agent.isStopped}");
                    _animator.Play("Walking");
                    break;
                case VillagerStates.Pickup:
                    agent.isStopped = true;
                    _animator.Play("Pickup");
                    break;
                case VillagerStates.Fighting:
                    _animator.Play("Stab");
                    break;
                case VillagerStates.AssigningRole:
                    _animator.Play("Spin");
                break;
            }
        }
    }
    
    /// <summary>
    /// The NavMeshAgent
    /// </summary>
    [FormerlySerializedAs("_agent")] public NavMeshAgent agent;
    
    /// <summary>
    /// Reference To The Animator Component of the Villager
    /// </summary>
    public Animator _animator;

    private GameManager _gameManager;
    
    [SerializeField] private Camera portraitCamera;
    public RenderTexture _portraitRenderTexture;
    
    
    [SerializeField] private VillagerStats _villagerStats;
    public VillagerStats VillagerStats
    {
        get => _villagerStats;
    }
    
    
    
    [SerializeField] private VillagerCustomisation _villagerCustomisation;
    public VillagerCustomisation VillagerCustomisation
    {
        get => _villagerCustomisation;
    }
    
    

    public Queue<IEnumerator> villagerQueue = new Queue<IEnumerator>();

    private void Awake()
    {
        monsters = new List<GameObject>();
        _gameManager = GameManager.Instance;
        agent = GetComponent<NavMeshAgent>();

        _animator = transform.GetChild(0).GetComponent<Animator>();
        if (SceneManager.GetActiveScene().name.Equals("New Scene"))
        {
            _portraitRenderTexture = new RenderTexture(256, 256, 8);
            portraitCamera.targetTexture = _portraitRenderTexture;
            _gameManager.villagerManager.GenerateNewVillagerStats(this);
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("New Scene"))
        {
            objInTriggerZone = gameObject.GetComponentInChildren<TriggerZone>().objInTriggerZone;
            objInAwarenessZone = gameObject.GetComponentInChildren<AwarenessZone>().objInAwarenessZone;
            
            transform.Find("FemaleCharacterPBR").Find("PortraitCamera").gameObject.SetActive(false);
            TryGetComponent(out Outline outline);
            outline.UpdateMaterialProperties();

            StartCoroutine(RunTasksCR());
        }
    }

    private void FixedUpdate()
    {
        portraitCamera.Render();
    }

    public void EditName(string text)
    {
        _villagerStats.VillagerName = text;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name.Contains("New Scene"))
        {

            if (objInAwarenessZone.Count == 0)
            {
                return;
            }

            for (int i = 0; i < objInAwarenessZone.Count; i++)
            {
                if (objInAwarenessZone[i].TryGetComponent(out Monster monster))
                {
                    Debug.Log($"{_villagerStats.VillagerName} is {CurrentRole}");
                    if ((CurrentRole == Roles.Fighter || CurrentRole == Roles.Leader) && !finding)
                    {
                        Debug.Log($"{_villagerStats.VillagerName} is {CurrentRole}");
                        finding = true;
                        StartCoroutine(FindTarget(3));
                    }
                    else if ((CurrentRole != Roles.Fighter || CurrentRole != Roles.Leader) &&!finding)
                    {
                        StartCoroutine(GoToLight(10));
                        //run away but this will be hard to code, making sure it doesn't run into another monster or to a fighter that is already busy
                    }

                    if (!monsters.Contains(objInAwarenessZone[i]))
                    {
                        monsters.Add(objInAwarenessZone[i]);
                    }
                }
            }

            if (target)
            {
                transform.LookAt(target.transform);

                for (int i = 0; i < objInTriggerZone.Count; i++)
                {
                    if (objInTriggerZone[i] == target && !attackStarted)
                    {
                        attackStarted = true;
                        print(gameObject.name + " has found its target");
                        StartCoroutine(AttackMonster());
                    }
                }
            }
        }
    }

    private IEnumerator RunTasksCR()
    {
        while (true)
        {
            while(villagerQueue.Count > 0)
            {
                yield return StartCoroutine(villagerQueue.Dequeue());
                Debug.Log($"Taks {villagerQueue.Count} Completed");
            }

            if (villagerQueue.Count == 0)
            {
                Debug.Log(_villagerStats.VillagerName + "Started Walking Randomly");
                villagerQueue.Enqueue(RandomWalk(3));
            }
            yield return null;
        }
        yield return null;  
    }

    
    public IEnumerator GoToLight(float timeTicks)
    {
        print("Go to Light");
        GameObject[] lightSource = GameObject.FindGameObjectsWithTag("Light");
        for (int i = 0; i < monsters.Count; i++)
        {
            distance = Vector3.Distance(transform.position, lightSource[i].transform.position);

            float nearestDistance = 1000;
            if (distance < nearestDistance)
            {
                nearestObject = lightSource[i];
                nearestDistance = distance;
                target = nearestObject;
            }
            if (target)
            {
                agent.SetDestination(target.transform.position);
            }
        }
        yield return new WaitForSeconds(timeTicks);
    }

    public IEnumerator FindTarget(float timeTicks)
    {
        yield return new WaitForSeconds(timeTicks);

        float nearestDistance = 1000;
        for (int i = 0; i < monsters.Count; i++)
        {
            //check for closest villager
            distance = Vector3.Distance(transform.position, monsters[i].transform.position);
            if (distance < nearestDistance)
            {
                nearestObject = monsters[i];
                nearestDistance = distance;
                target = nearestObject;
            }
        }

        if (target)
        {
            CurrentState = VillagerStates.Walking;
            agent.SetDestination(target.transform.position);
        }
        finding = false;
    }

    private IEnumerator AttackMonster()
    {
        CurrentState = VillagerStates.Fighting;
        agent.isStopped = true;

        yield return new WaitForSeconds(1);
        
        agent.isStopped = false;

        if (target)
        {
            GetComponent<NavMeshAgent>().SetDestination(target.transform.position);
            for (int i = 0; i < objInTriggerZone.Count; i++)
            {
                if (objInTriggerZone[i] == target)
                {
                    target.GetComponent<Monster>().health -= 1 + _villagerStats.Strength / 10;
                    if (target.GetComponent<Monster>().health <= 0)
                    {
                        monsters.Remove(target.gameObject);
                        Destroy(target.gameObject);
                        _gameManager.level.villageHeart.GetComponent<VillageHeart>().Experience += 10;

                    }
                    CurrentState = VillagerStates.Idle;
                }
            }
            if(!target)
            {
                _currentState = VillagerStates.Idle;
            }
        }
        attackStarted = false;
        _currentState = VillagerStates.Idle;
        StartCoroutine(FindTarget(1));
    }


    private void GetAnimationForRole()
    {
        switch (_villagerRole)
        {
            case Roles.Lumberjack:
                _animator.Play("Axe");
                break;
            case Roles.Miner:
                _animator.Play("Pick");
                break;
        }
    }
    
    public static void SetVillagerDestination(Villager villager, Vector3 position)
    {
        villager.agent.SetDestination(position);
    }

    public static void StopVillager(Villager villager, bool value)
    {
        villager.agent.isStopped = value;
    }

    public void OnInteraction()
    {
        // if (_gameManager.IsOverUI())
        // {
        //     return;
        // }
        
        // _gameManager.level.ShowVillagerInformationOnClick(this);
        // _gameManager.uiManager.SetVillagerStatsUI(this);
        
        

        var uIToolkitManager = GameObject.Find("UIToolkit").GetComponent<UIToolkitManager>();
        uIToolkitManager.ShowVillagerInformation(this);

        if (uIToolkitManager.IsPointerOverUI(_gameManager.inputManager.playerInputActions.UI.Point.ReadValue<Vector2>()))
        {
            return; 
        }
        
        VillagerStats.CurrentEmotion = Emotion.None;
        var tutorialManager = _gameManager.level.tutorialManager;
        if (tutorialManager.TutorialStage == TutorialStage.KeyboardMovementTutorial)
        {
            tutorialManager.wKey.transform.DOScale(Vector3.one,0.5f);
            tutorialManager.sKey.transform.DOScale(Vector3.one, 0.5f);
            tutorialManager.aKey.transform.DOScale(Vector3.one, 0.5f);
            tutorialManager.dKey.transform.DOScale(Vector3.one, 0.5f);
        }
    }

    public bool CanInteract()
    {
        if (!_gameManager.level.stockpileMode)
        {
            return true;
        }

        return true;
    }
    
    public IEnumerator RandomWalk(float size)
    {
        var position = transform.position;  
    
        var newPosition = new Vector3(position.x + UnityEngine.Random.Range(-size, size), position.y,
            position.z + UnityEngine.Random.Range(-size, size));

        while (!NavMesh.SamplePosition(newPosition, out NavMeshHit hit, 0, 3))
        {
            newPosition = new Vector3(position.x + UnityEngine.Random.Range(-size, size), position.y,
                position.z + UnityEngine.Random.Range(-size, size));
        }

        yield return StartCoroutine(Tasks.WalkToLocation(this, newPosition));

        CurrentState = VillagerStates.Idle;
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1f));

    }

    public void OnDeath()
    {
        VillagerManager.villagers.Remove(this);
        Destroy(_gameManager.uiManager.templateDictionary[this]);
        _gameManager.uiManager.templateDictionary.Remove(this);
        Destroy(gameObject);
    }
}

public enum Roles 
{
    Default = 0,
    Lumberjack = 1,
    Farmer = 2,
    Fighter = 3,
    Miner = 4,
    Crafter = 5,
    Leader = 6,
}

public enum VillagerStates
{
    Idle,
    Working,
    Sleeping,
    Walking,
    Pickup,
    Fighting,
    AssigningRole
}
public enum Model
{
    Man,
    Woman,
}
