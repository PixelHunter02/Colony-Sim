using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Villager : MonoBehaviour, IInteractable
{
    /// <summary>
    /// The Villagers Role will give the Villager boosted stats in a specific craft as well as more abilities linked to that craft.
    /// </summary>
    private Roles villagerRole;
    public Roles CurrentRole
    {
        get => villagerRole;
        set
        {
            villagerRole = value;

            switch (villagerRole)
            {
                case Roles.Default:
                    Debug.Log($"{villagerName} Changed To {villagerRole}");
                    break;
                case Roles.Farmer:
                    Debug.Log($"{villagerName} Changed To {villagerRole}");
                    break;
                case Roles.Fighter:
                    Debug.Log($"{villagerName} Changed To {villagerRole}");
                    break;
                case Roles.Lumberjack:
                    Debug.Log($"{villagerName} Changed To {villagerRole}");
                    break;
                case Roles.Miner:
                    Debug.Log($"{villagerName} Changed To {villagerRole}");
                    break;
                case Roles.Crafter:
                    Debug.Log($"{villagerName} Changed To {villagerRole}");
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
                    _agent.isStopped = false;
                    _animator.Play("Walking");
                    break;
                case VillagerStates.Pickup:
                    _agent.isStopped = true;
                    _animator.Play("Pickup");
                    break;
            }
        }
    }
    
    /// <summary>
    /// The NavMeshAgent
    /// </summary>
    public NavMeshAgent _agent;

    /// <summary>
    /// Villager Image
    /// </summary>
    private Image _roleImage;
    
    /// <summary>
    /// Reference To The Animator Component of the Villager
    /// </summary>
    [SerializeField]private Animator _animator;

    private GameManager _gameManager;
    
    private bool runningTasks;

    private List<IEnumerator> tasks;

    private List<IEnumerator> tasksToQueue;

    public List<IEnumerator> TasksToQueue
    {
        get => tasksToQueue;
    }

    
    private Model gender;

    public Model Gender
    {
        get => gender;
        set
        {
            gender = value;
            Debug.Log($"The Gender of The Villager {villagerName} has changed to {gender}");
            switch (gender)
            {
                case Model.Man:
                    femaleHead.SetActive(false);
                    femaleBody.SetActive(false);
                    maleHead.SetActive(true);
                    maleBody.SetActive(true);
                    maleHead.transform.GetChild(3).GetComponent<MeshRenderer>().material = hairColour;
                    break;
                case Model.Woman:
                    femaleHead.SetActive(true);
                    femaleBody.SetActive(true);
                    maleHead.SetActive(false);
                    maleBody.SetActive(false);
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

    public TMP_Text magicText;
    public TMP_Text craftText;
    public TMP_Text strengthText;
    public TMP_InputField nameText;
    public int health;

    private string villagerName;
    public string VillagerName
    {
        get => villagerName;
        set
        {
            villagerName = value;
            if(SceneManager.GetActiveScene().name.Equals("Main Menu"))
                nameText.text = villagerName;
            Debug.Log($"A New Name Has Been Assigned! The new value is {villagerName}");
        }
    }
    
    private int strength;
    public int Strength
    {
        get => strength;
        set
        {
            strength = value;
            Debug.Log($"A New Craft Value Has Been Assigned! The new value is {strength}");
            if(SceneManager.GetActiveScene().name == _gameManager.mainMenu)
                strengthText.text = $"Strength: {strength}";
        }
    }
    private int craft;

    public int Craft
    {
        get => craft;
        set
        {
            craft = value;
            Debug.Log($"A New Craft Value Has Been Assigned! The new value is {craft}");
            if(SceneManager.GetActiveScene().name == _gameManager.mainMenu)
                craftText.text = $"Craft: {craft}";
        }
    }
    
    private int magic;

    public int Magic
    {
        get => magic;
        set
        {
            magic = value;
            Debug.Log($"A New Craft Value Has Been Assigned! The new value is {craft}");
            if(SceneManager.GetActiveScene().name == _gameManager.mainMenu)
                magicText.text = $"Magic: {magic}";
        }
    }

    #endregion


    private void Awake()
    {
        health = 200;
        _gameManager = GameManager.Instance;
        _agent = GetComponent<NavMeshAgent>();
        tasks = new List<IEnumerator>();

        tasksToQueue = new List<IEnumerator>();
        _animator = transform.GetChild(0).GetComponent<Animator>();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("New Scene"))
        {
            transform.Find("FemaleCharacterPBR").Find("PortraitCamera").gameObject.SetActive(false);
            TryGetComponent(out Outline outline);
            outline.UpdateMaterialProperties();
            if (tasks.Count == 0 && !runningTasks && tasksToQueue.Count > 0)
            {
                foreach (var task in tasksToQueue)
                {
                    tasks.Add(task);
                }
                tasksToQueue.Clear();
            }
            StartCoroutine(RunTasks());
            StartCoroutine(RandomWalk(4));
        }
        else if (SceneManager.GetActiveScene().name.Equals("Main Menu"))
        {
            nameText.onEndEdit.AddListener(EditName);
        }
    }

    private void EditName(string text)
    {
        VillagerName = text;
        // Debug.Log(villagerName);
    }
    
    private void Update()
    {
        if (tasks.Count == 0 && !runningTasks && tasksToQueue.Count > 0)
        {
            // ClearCompleteTasks();
            foreach (var task in tasksToQueue)
            {
                tasks.Add(task);
            }
            tasksToQueue.Clear();
            runningTasks = true;
            StartCoroutine(RunTasks());
        }
    }


    private void GetAnimationForRole()
    {
        switch (villagerRole)
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
        villager._agent.SetDestination(position);
    }

    public static void StopVillager(Villager villager, bool value)
    {
        villager._agent.ResetPath();
        villager._agent.isStopped = value;
    }

    public void OnInteraction()
    {
        _gameManager.level.ShowVillagerInformation(this);
        Interactions.SetNewSelectedVillager(this);
    }

    public void AddTaskToQueue(IEnumerator task)
    {
        tasksToQueue.Add(task);
    }

    private IEnumerator RunTasks()
    {
        foreach (var task in tasks)
        {
            yield return task;
        }
        runningTasks = false;
        tasks.Clear();
    }

    private IEnumerator RandomWalk(float size)
    {
        _agent.isStopped = false;
        yield return new WaitForSeconds(Random.Range(0.1f, 8f));
        var newPosition = new Vector3(transform.position.x + Random.Range(-size, size), transform.position.y,
            transform.position.z + Random.Range(-size, size));
        while(!Physics.Raycast(newPosition, Vector3.down * 5, 3,~3))
        {
            newPosition = new Vector3(transform.position.x + Random.Range(-size, size), transform.position.y+2,
                transform.position.z + Random.Range(-size, size));
            yield return null;
        }
       
        if (tasks.Count > 0 || tasksToQueue.Count > 0 || CurrentState is not VillagerStates.Idle) 
        {
            yield break;
        }
        
        _agent.SetDestination(newPosition);

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
