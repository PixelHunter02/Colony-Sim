using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
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
            Level.AddToVillagerLog(this, $"{_villagerName} Changed From {_villagerRole} To {value}");

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
                    break;
                case Roles.Miner:
                    break;
                case Roles.Crafter:
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
                    StartCoroutine(RandomWalk(4));
                    break;
                case VillagerStates.Working:
                    Debug.Log("Switching");
                    GetAnimationForRole();
                    break;
                case VillagerStates.Sleeping:
                    break;
                case VillagerStates.Walking:
                    agent.isStopped = false;
                    _animator.Play("Walking");
                    break;
                case VillagerStates.Pickup:
                    agent.isStopped = true;
                    _animator.Play("Pickup");
                    break;
                case VillagerStates.Fighting:
                    _animator.Play("Attack03_SwordAndShiled");
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
    [SerializeField]private Animator _animator;

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
                    maleHead.transform.GetChild(3).GetComponent<MeshRenderer>().material = hairColour;
                    break;
                case Model.Woman:
                    femaleHead.SetActive(true);
                    femaleBody.SetActive(true);
                    maleHead.SetActive(false);
                    maleBody.SetActive(false);
                    var randomPosition = Random.Range(0, VillagerManager.femaleNames.Count);
                    VillagerName = VillagerManager.femaleNames[randomPosition];
                    femaleHead.transform.GetChild(3).GetComponent<MeshRenderer>().material = hairColour;
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
    public Material hairColour;

    #endregion


    #region Stats
    
    public int health;

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
            Debug.Log($"A New Craft Value Has Been Assigned! The new value is {_strength}");
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
            Debug.Log($"A New Craft Value Has Been Assigned! The new value is {_craft}");
        }
    }

    [SerializeField] private Camera portraitCamera;
    public RenderTexture _portraitRenderTexture;
    #endregion


    private void Awake()
    {
        monsters = new List<GameObject>();
        health = 20;
        _gameManager = GameManager.Instance;
        agent = GetComponent<NavMeshAgent>();
        _villagerTasks = new List<IEnumerator>();

        TasksToQueue = new List<IEnumerator>();
        _animator = transform.GetChild(0).GetComponent<Animator>();
        _portraitRenderTexture = new RenderTexture(256,256, 8);
        portraitCamera.targetTexture = _portraitRenderTexture;

    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("New Scene"))
        {
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
            StartCoroutine(RandomWalk(4));
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

        for (int i = 0; i < objInAwarenessZone.Count; i++)
        {
            if (!objInAwarenessZone[i])
            {
                return;
            }
            if (objInAwarenessZone[i].GetComponent<Monster>())
            {
                if (_villagerRole == Roles.Fighter && !finding)
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
                    target.GetComponent<Monster>().health -= 1;
                    print(target.name + " health is down to: " + target.GetComponent<Monster>().health);
                    
                    if (target.GetComponent<Monster>().health <= 0)
                    {
                        monsters.Remove(target.gameObject);
                        Destroy(target.gameObject);

                    }
                    CurrentState = VillagerStates.Idle;
                }
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
        _gameManager.level.ShowVillagerInformation(this);
        _gameManager.uiManager.SetVillagerStatsUI(this);
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

    private IEnumerator RandomWalk(float size)
    {
        agent.isStopped = false;
        yield return new WaitForSeconds(Random.Range(0.1f, 8f));
        var position = transform.position;
        var newPosition = new Vector3(position.x + Random.Range(-size, size), position.y,
            position.z + Random.Range(-size, size));
        while(!Physics.Raycast(newPosition, Vector3.down * 5, 3,~3))
        {
            newPosition = new Vector3(transform.position.x + Random.Range(-size, size), position.y+2,
                position.z + Random.Range(-size, size));
            yield return null;
        }
       
        if (_villagerTasks.Count > 0 || TasksToQueue.Count > 0 || CurrentState is not VillagerStates.Idle) 
        {
            yield break;
        }
        
        agent.SetDestination(newPosition);

        _animator.Play("Walking");
        while (Vector3.Distance(transform.position, newPosition) > 0.5)
        {
            yield return null;
        }

        CurrentState = VillagerStates.Idle;
    }
}

public enum Roles 
{
    Default,
    Lumberjack,
    Farmer,
    Fighter,
    Miner,
    Crafter,
    Leader,
}

public enum VillagerStates
{
    Idle,
    Working,
    Sleeping,
    Walking,
    Pickup,
    Fighting,
}
public enum Model
{
    Man,
    Woman,
}
