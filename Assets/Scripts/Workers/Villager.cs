using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.AI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
                Level.AddToVillagerLog(this, $"{_villagerName} Changed From {_villagerRole} To {value}");
            }

            _villagerRole = value;

            Debug.Log($"{_villagerName} Changed To {_villagerRole}");

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
                    Strength = 6;
                    Craft = 6;
                    Magic = 6;
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
                    if (RandomWalkCR is null)
                    {
                        RandomWalkCR = StartCoroutine(RandomWalk(4));
                    }
                    else
                    {
                        RandomWalkCR = null;
                        RandomWalkCR = StartCoroutine(RandomWalk(4));
                    }
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
    
    private bool _runningTasks;

    private List<IEnumerator> _villagerTasks;

    public List<IEnumerator> TasksToQueue 
    { 
        get; 
        private set;
    }


    private Model _gender;

    public Model Gender
    {
        get => _gender;
        set
        {
            _gender = value;
            Debug.Log($"The Gender of The Villager {_villagerName} has changed to {_gender}");
            switch (_gender)
            {
                case Model.Man:
                    femaleHead.SetActive(false);
                    femaleBody.SetActive(false);
                    maleHead.SetActive(true);
                    maleBody.SetActive(true);
                    var randomPositionMale = Random.Range(0, VillagerManager.maleNames.Count);
                    VillagerName = VillagerManager.maleNames[randomPositionMale];
                    maleHead.transform.GetChild(3).GetComponent<MeshRenderer>().material = HairColour;
                    break;
                case Model.Woman:
                    femaleHead.SetActive(true);
                    femaleBody.SetActive(true);
                    maleHead.SetActive(false);
                    maleBody.SetActive(false);
                    var randomPosition = Random.Range(0, VillagerManager.femaleNames.Count);
                    VillagerName = VillagerManager.femaleNames[randomPosition];
                    femaleHead.transform.GetChild(3).GetComponent<MeshRenderer>().material = HairColour;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    #region BodyParts

    [SerializeField] private GameObject femaleHead;
    [SerializeField] private GameObject maleHead;
    [SerializeField] private GameObject femaleBody;
    [SerializeField] private GameObject maleBody;
    private Material _hairColour;

    public Material HairColour
    {
        get => _hairColour;
        set
        {
            _hairColour = value;
            
            maleHead.transform.GetChild(3).GetComponent<MeshRenderer>().material = HairColour;
            femaleHead.transform.GetChild(3).GetComponent<MeshRenderer>().material = HairColour;
        }
    }

    #endregion


    #region Stats
    
    private int health;
    private int maxHealth;
    int modifiedMaxHealth;

    public int Health
    {
        get => health;
        set
        {
            health = value;
            Debug.Log($"{_villagerName}'s health has changed to {value}");
            switch (value)
            {
                case <= 0:
                    OnDeath();
                    Debug.Log("Died");
                    break;
            }
        }
    }

    private string _villagerName;
    public string VillagerName
    {
        get => _villagerName;
        set
        {
            _villagerName = value;
            Debug.Log($"A New Name Has Been Assigned! The new value is {_villagerName}");
        }
    }
    
    private int _strength;
    public int Strength
    {
        get => _strength;
        set
        {
            _strength = value;
            
            Debug.Log($"A New Strength Value Has Been Assigned! The new value is {_strength}");
        }
    }
    
    private int _craft;
    public int Craft
    {
        get => _craft;
        set
        {
            _craft = value;
            Debug.Log($"A New Craft Value Has Been Assigned! The new value is {_craft}");
        }
    }
    
    private int _magic;
    public int Magic
    {
        get => _magic;
        set
        {
            _magic = value;
            Debug.Log($"A New Magic Value Has Been Assigned! The new value is {_magic}");
        }
    }

    [SerializeField] private Camera portraitCamera;
    public RenderTexture _portraitRenderTexture;
    #endregion


    private void Awake()
    {
        monsters = new List<GameObject>();
        

        _gameManager = GameManager.Instance;
        agent = GetComponent<NavMeshAgent>();
        _villagerTasks = new List<IEnumerator>();

        TasksToQueue = new List<IEnumerator>();
        _animator = transform.GetChild(0).GetComponent<Animator>();
        if (SceneManager.GetActiveScene().name.Equals("New Scene") || SceneManager.GetActiveScene().name.Equals("Tablet"))
        {
            _portraitRenderTexture = new RenderTexture(256, 256, 8);
            portraitCamera.targetTexture = _portraitRenderTexture;
            _gameManager.villagerManager.GenerateNewVillagerStats(this);
        }
        else if (SceneManager.GetActiveScene().name.Equals("te"))
        {
            CurrentState = VillagerStates.Idle;
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("New Scene") || SceneManager.GetActiveScene().name.Equals("Tablet"))
        {
            maxHealth = 20;
            modifiedMaxHealth = Mathf.CeilToInt(20 + (0.3f * Strength));
            Debug.Log(modifiedMaxHealth);
            Health = modifiedMaxHealth;
            transform.Find("FemaleCharacterPBR").Find("PortraitCamera").gameObject.SetActive(false);
            TryGetComponent(out Outline outline);
            outline.UpdateMaterialProperties();
            if (_villagerTasks.Count == 0 && !_runningTasks && TasksToQueue.Count > 0)
            {
                foreach (var task in TasksToQueue)
                {
                    _villagerTasks.Add(task);
                }
                TasksToQueue.Clear();
            }
            StartCoroutine(RunTasks());
            if (RandomWalkCR is null)
            {
                RandomWalkCR = StartCoroutine(RandomWalk(4));
            }
            else
            {
                RandomWalkCR = null;
                RandomWalkCR = StartCoroutine(RandomWalk(4));
            }
            // RandomWalkAsync(4);
        }
    }

    private void FixedUpdate()
    {
        portraitCamera.Render();
    }

    public void EditName(string text)
    {
        VillagerName = text;
    }
    
    private void Update()
    {
        if (SceneManager.GetActiveScene().name.Contains("New Scene") || SceneManager.GetActiveScene().name.Equals("Tablet"))
        {
            objInTriggerZone = gameObject.GetComponentInChildren<TriggerZone>().objInTriggerZone;
            objInAwarenessZone = gameObject.GetComponentInChildren<AwarenessZone>().objInAwarenessZone;
            
            if (_villagerTasks.Count == 0 && !_runningTasks && TasksToQueue.Count > 0)
            {
                foreach (var task in TasksToQueue)
                {
                    _villagerTasks.Add(task);
                }
                TasksToQueue.Clear();
                _runningTasks = true;
                StartCoroutine(RunTasks());
            }
        }
        
        

        for (int i = 0; i < objInAwarenessZone.Count; i++)
        {
            if (!objInAwarenessZone[i])
            {
                return;
            }
            if (objInAwarenessZone[i].GetComponent<Monster>())
            {
                if ((_villagerRole == Roles.Fighter|| _villagerRole == Roles.Leader) && !finding)
                {
                    finding = true;
                    StartCoroutine(FindTarget(3));
                }
                else
                {
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
                //print(gameObject.name + " has come into contact with " + objInTriggerZone[i]);
                //print(gameObject.name + " is looking for " + target);
                if (objInTriggerZone[i] == target && !attackStarted)
                {
                    attackStarted = true;
                    print(gameObject.name + " has found its target");
                    StartCoroutine(AttackMonster());
                }
            }
        }
    }

    public IEnumerator FindTarget(float timeTicks)
    {
        yield return new WaitForSeconds(timeTicks);

        float nearestDistance = 1000;
        for (int i = 0; i < monsters.Count; i++)
        {
            //check for closest villager
            distance = Vector3.Distance(transform.position, monsters[i].transform.position);
            //print(distance);
            //print(nearestDistance);

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
                print(_villagerName + " has come into contact with " + objInTriggerZone[i]);
                print(_villagerName + " is looking for " + target);
                if (objInTriggerZone[i] == target)
                {
                    //transform.LookAt(target.transform);
                    //CurrentState = VillagerStates.Fighting;
                    print("Villager fights Back");
                    target.GetComponent<Monster>().health -= 1 + _strength / 10;
                    print(target.name + " health is down to: " + target.GetComponent<Monster>().health);
                    
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
        StartCoroutine(FindTarget(1));
    }


    private void GetAnimationForRole()
    {
        switch (_villagerRole)
        {
            case Roles.Lumberjack:
                Debug.Log("Playing Axe");
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
        villager.agent.ResetPath();
        villager.agent.isStopped = value;
    }

    public void OnInteraction()
    {
        if (_gameManager.IsOverUI())
        {
            return;
        }
        
        _gameManager.level.ShowVillagerInformationOnClick(this);
        _gameManager.uiManager.SetVillagerStatsUI(this);
        if (_gameManager.level.tutorialManager.TutorialStage == TutorialStage.VillagerStatsTutorial)
        {
            _gameManager.level.tutorialManager.TutorialStage = TutorialStage.StockpileTutorial;
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


    private IEnumerator RunTasks()
    {
        foreach (var task in _villagerTasks)
        {
            yield return task;
        }
        _runningTasks = false;
        _villagerTasks.Clear();
    }

    public Coroutine RandomWalkCR;
    public IEnumerator RandomWalk(float size)
    {

        agent.ResetPath();
        agent.isStopped = false;
        yield return new WaitForSeconds(Random.Range(0.1f, 8f));
        var position = transform.position;  
       
        if (_villagerTasks.Count > 0 || TasksToQueue.Count > 0 || CurrentState is not VillagerStates.Idle) 
        {
            yield break;
        }

        var newPosition = new Vector3(position.x + Random.Range(-size, size), position.y,
            position.z + Random.Range(-size, size));


        if(!NavMesh.SamplePosition(newPosition, out NavMeshHit hit, 1f, NavMesh.AllAreas)){
            Debug.Log("reset");
            if (RandomWalkCR is null)
            {
                RandomWalkCR = StartCoroutine(RandomWalk(4));
            }
            else
            {
                RandomWalkCR = null;
                RandomWalkCR = StartCoroutine(RandomWalk(4));
            }
            yield break;
        }

        agent.SetDestination(newPosition);

        _animator.Play("Walking");
        while (Vector3.Distance(transform.position, newPosition) > 2)
        {
            yield return null;
        }
    
        CurrentState = VillagerStates.Idle;
    }

    private void OnDeath()
    {
        // Debug.Log(VillagerManager.villagers.Contains(target.GetComponent<Villager>()));
        VillagerManager.villagers.Remove(this);
        // villagers/.Remove(target.gameObject);
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
