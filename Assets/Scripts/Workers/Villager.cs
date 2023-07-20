using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Villager : MonoBehaviour, IInteractable
{
    /// <summary>
    /// The Villagers Role will give the Villager boosted stats in a specific craft as well as more abilities linked to that craft.
    /// </summary>
    private Roles _villagerRole;
    public Roles CurrentRole
    {
        get => _villagerRole;
        set
        {
            _villagerRole = value;

            switch (_villagerRole)
            {
                case Roles.Default:
                    Debug.Log($"{_villagerName} Changed To {_villagerRole}");
                    break;
                case Roles.Farmer:
                    Debug.Log($"{_villagerName} Changed To {_villagerRole}");
                    break;
                case Roles.Fighter:
                    Debug.Log($"{_villagerName} Changed To {_villagerRole}");
                    break;
                case Roles.Lumberjack:
                    Debug.Log($"{_villagerName} Changed To {_villagerRole}");
                    break;
                case Roles.Miner:
                    Debug.Log($"{_villagerName} Changed To {_villagerRole}");
                    break;
                case Roles.Crafter:
                    Debug.Log($"{_villagerName} Changed To {_villagerRole}");
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
            }
        }
    }
    
    /// <summary>
    /// The NavMeshAgent
    /// </summary>
    [FormerlySerializedAs("_agent")] public NavMeshAgent agent;

    /// <summary>
    /// Villager Image
    /// </summary>
    private Image _roleImage;
    
    /// <summary>
    /// Reference To The Animator Component of the Villager
    /// </summary>
    [SerializeField]private Animator _animator;

    private GameManager _gameManager;
    
    private bool _runningTasks;

    private List<IEnumerator> _tasks;

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
        health = 20;
        _gameManager = GameManager.Instance;
        agent = GetComponent<NavMeshAgent>();
        _tasks = new List<IEnumerator>();

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
            if (_tasks.Count == 0 && !_runningTasks && TasksToQueue.Count > 0)
            {
                foreach (var task in TasksToQueue)
                {
                    _tasks.Add(task);
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
        if (_tasks.Count == 0 && !_runningTasks && TasksToQueue.Count > 0)
        {
            foreach (var task in TasksToQueue)
            {
                _tasks.Add(task);
            }
            TasksToQueue.Clear();
            _runningTasks = true;
            StartCoroutine(RunTasks());
        }
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
        foreach (var task in _tasks)
        {
            yield return task;
        }
        _runningTasks = false;
        _tasks.Clear();
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
       
        if (_tasks.Count > 0 || TasksToQueue.Count > 0 || CurrentState is not VillagerStates.Idle) 
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
}

public enum VillagerStates
{
    Idle,
    Working,
    Sleeping,
    Walking,
    Pickup,
}
public enum Model
{
    Man,
    Woman,
}
