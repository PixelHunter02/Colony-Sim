using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Worker : MonoBehaviour, IInteractable
{
    /// <summary>
    /// The Workers Role will give the worker boosted stats in a specific craft as well as more abilities linked to that craft.
    /// </summary>
    [SerializeField] private Roles workerRole;
    public Roles CurrentRole
    {
        get => workerRole;
        set
        {
            workerRole = value;

            switch (workerRole)
            {
                case Roles.Default:
                    break;
                case Roles.Farmer:
                    break;
                case Roles.Fighter:
                    break;
                case Roles.Lumberjack:
                    workerImage.sprite = lumberjackImage;
                    break;
                case Roles.Miner:
                    workerImage.sprite = minerImage;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    /// <summary>
    /// The Workers Current State
    /// </summary>
    private WorkerStates _currentState;
    public WorkerStates CurrentState
    {
        get => _currentState;
        set
        {
            _currentState = value;

            switch (_currentState)
            {
                case WorkerStates.Idle:
                    _animator.Play("Idle");
                    break;
                case WorkerStates.Working:
                    GetAnimationForRole();
                    break;
                case WorkerStates.Sleeping:
                    break;
                case WorkerStates.Walking:
                    _animator.Play("Walking");
                    break;
            }
        }
    }

    /// <summary>
    /// The Workers Current model
    /// </summary>
    [SerializeField] private Model gender;

    /// <summary>
    /// The NavMeshAgent
    /// </summary>
    private NavMeshAgent _agent;

    /// <summary>
    /// Worker Information
    /// </summary>
    [SerializeField] private string workerName;

    /// <summary>
    /// Worker Image
    /// </summary>
    [SerializeField] private Image workerImage;
    
    /// <summary>
    /// The object that the Worker is interacting with
    /// </summary>
    public HarvestObjectManager interactingWith;

    /// <summary>
    /// Reference To The Animator Component of the Worker
    /// </summary>
    [SerializeField]private Animator _animator;

    /// <summary>
    /// Images
    /// </summary>
    [SerializeField] private Sprite minerImage;
    [SerializeField] private Sprite lumberjackImage;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        TryGetComponent(out Outline outline);
        outline.UpdateMaterialProperties();
    }


    private void GetAnimationForRole()
    {
        switch (workerRole)
        {
            case Roles.Lumberjack:
                _animator.Play("Axe");
                break;
            case Roles.Miner:
                _animator.Play("Pick");
                break;
        }
    }
    
    public static void SetWorkerDestination(Worker worker, Vector3 position)
    {
        worker._agent.SetDestination(position);
    }

    public static void StopWorker(Worker worker, bool value)
    {
        worker._agent.ResetPath();
        worker._agent.isStopped = value;
    }

    public void OnInteraction()
    {
        var infoUI = gameObject.transform.GetChild(0).GetChild(1).gameObject;
        infoUI.SetActive(!infoUI.activeSelf);
        infoUI.transform.Find("Name").TryGetComponent(out TMP_Text workerNameTMP);
        infoUI.transform.Find("Job").TryGetComponent(out TMP_Text workerRoleTMP);
        workerNameTMP.text = workerName;
        workerRoleTMP.text = workerRole.ToString();
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
public enum Model
{
    Man,
    Woman,
}
