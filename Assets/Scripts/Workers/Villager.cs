using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Villager : MonoBehaviour, IInteractable
{
    /// <summary>
    /// The Villagers Role will give the Villager boosted stats in a specific craft as well as more abilities linked to that craft.
    /// </summary>
    public Roles villagerRole;
    public Roles CurrentRole
    {
        get => villagerRole;
        set
        {
            villagerRole = value;

            switch (villagerRole)
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

    private List<IEnumerator> tasks;
    private List<IEnumerator> completeTasks;

    /// <summary>
    /// The NavMeshAgent
    /// </summary>
    public NavMeshAgent _agent;

    /// <summary>
    /// Villager Information
    /// </summary>
    [SerializeField] private string villagerName;
    public string VillagerName
    {
        get => villagerName;
    }

    /// <summary>
    /// Villager Image
    /// </summary>
    private Image _roleImage;
    
    /// <summary>
    /// The object that the Villager is interacting with
    /// </summary>
    public HarvestObjectManager interactingWith;

    /// <summary>
    /// Reference To The Animator Component of the Villager
    /// </summary>
    [SerializeField]private Animator _animator;

    private GameManager _gameManager;

    public StoredItemSO currentlyHolding;

    private bool runningTasks;

    private List<IEnumerator> tasksToQueue;

    public int health;

    public List<IEnumerator> TasksToQueue
    {
        get => tasksToQueue;
    }
    


    private void Awake()
    {
        health = 200;
        _gameManager = GameManager.Instance;
        _agent = GetComponent<NavMeshAgent>();
        tasks = new List<IEnumerator>();

        completeTasks = new List<IEnumerator>();
        tasksToQueue = new List<IEnumerator>();
    }

    private void Start()
    {
        TryGetComponent(out Outline outline);
        outline.UpdateMaterialProperties();
        if (tasks.Count == 0 && !runningTasks && tasksToQueue.Count > 0)
        {
            // ClearCompleteTasks();
            foreach (var task in tasksToQueue)
            {
                tasks.Add(task);
            }
            tasksToQueue.Clear();
        }
        StartCoroutine(RunTasks());
        StartCoroutine(RandomWalk(4));
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
        _gameManager.ShowVillagerInformation(this);
        Interactions.SetNewSelectedVillager(this);
    }

    public void AddTaskToQueue(IEnumerator task)
    {
        tasksToQueue.Add(task);
    }

    private IEnumerator RunTasks()
    {
        completeTasks = new List<IEnumerator>();
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
