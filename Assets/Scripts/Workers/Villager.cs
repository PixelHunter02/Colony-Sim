using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
                    break;
                case VillagerStates.Working:
                    GetAnimationForRole();
                    break;
                case VillagerStates.Sleeping:
                    break;
                case VillagerStates.Walking:
                    _animator.Play("Walking");
                    break;
            }
        }
    }

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

    public PickUpItemSO currentlyHolding;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        TryGetComponent(out Outline outline);
        outline.UpdateMaterialProperties();
    }


    private void GetAnimationForRole()
    {
        switch (villagerRole)
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
        villager._agent.SetDestination(position);
    }

    public static void StopVillager(Villager villager, bool value)
    {
        villager._agent.ResetPath();
        villager._agent.isStopped = value;
    }

    public void OnInteraction()
    {
        _gameManager.uiManager.ShowVillagerInformation(this);
        Interactions.SetNewSelectedVillager(this);
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
}
public enum Model
{
    Man,
    Woman,
}
