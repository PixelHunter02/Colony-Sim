using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    /// <summary>
    /// The Workers Role will give the worker boosted stats in a specific craft as well as more abilities linked to that craft.
    /// </summary>
    [SerializeField] private Roles role;
    
    /// <summary>
    /// The Workers Current State
    /// </summary>
    [SerializeField] private WorkerStates _currentState;

    /// <summary>
    /// The NavMeshAgent
    /// </summary>
    private NavMeshAgent agent;

    /// <summary>
    /// Worker Information
    /// </summary>
    [SerializeField] private string workerName;
    
    /// <summary>
    /// The object that the Worker is interacting with
    /// </summary>
    public HarvestObjectManager interactingWith;

    /// <summary>
    /// Reference To The Task Handler Script
    /// </summary>
    [SerializeField] private TaskHandler taskHandler;
    
    /// <summary>
    /// Reference To The Animator Component of the Worker
    /// </summary>
    [SerializeField]private Animator _animator;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    WorkerStates CurrentState
    {
        set
        {
            _currentState = value;
            
            switch (_currentState)
            {
                case WorkerStates.Idle:
                    _animator.Play("Idle");
                    break;
                case WorkerStates.Working:
                    _animator.Play("Idle");
                    break;
                case WorkerStates.Sleeping:
                    break;
                case WorkerStates.Walking:
                    _animator.Play("Walking");
                    break;
            }
        }
    }
    
    private void Update()
    {
        
    }

    public static void ChangeWorkerState(Worker worker, WorkerStates newState)
    {
        worker.CurrentState = newState;
    }

    public static WorkerStates GetWorkerState(Worker worker)
    {
        return worker._currentState;
    }

    public static void ChangeWorkerRole(Worker worker, Roles newRole )
    {
        worker.role = newRole;
    }

    public static Roles GetWorkerRole(Worker worker)
    {
        return worker.role;
    }

    public static void SetWorkerDestination(Worker worker, Vector3 position)
    {
        worker.agent.SetDestination(position);
    }

    public static void StopWorker(Worker worker, bool value)
    {
        worker.agent.isStopped = value;
    }
}

public enum Roles 
{
    Default,
    Lumberjack,
    Farmer,
    Fighter,
    Miner,
}

public enum WorkerStates
{
    Idle,
    Working,
    Sleeping,
    Walking,
}
