using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SnakeAI : MonoBehaviour
{
    // Hide underground during the day until approached
    // At night come to attack livestock and people
    // Start is called before the first frame update
    // Other Components
    private NavMeshAgent agent;
    private Animator _animator;
    [SerializeField] private string _currentState;
    [SerializeField] private string _idle = "Idle";
    [SerializeField] private string _moving = "Run Forward In Place";
    [SerializeField] private string _attack = "Bite Attack";
    [SerializeField] private string _goHome = "Underground 0";

    // Stats
    public float attackCoolDown;

    private Vector3 den;

    // Targeting for Combat
    [SerializeField] private List<GameObject> objInTriggerZone;
    [SerializeField] private List<GameObject> objInAwarenessZone;
    [SerializeField] private float distance;
    [SerializeField] private float nearestDistance = 1000;
    public GameObject nearestObject;
    [SerializeField] private GameObject target;

    [SerializeField] private List<GameObject> villagers;

    [SerializeField] private bool goHome;

    private bool attackStarted;

    private void Awake()
    {
        attackCoolDown = 4;
        villagers = new List<GameObject>();

        _animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        foreach (Villager villager in VillagerManager.GetVillagers())
        {
            villagers.Add(villager.gameObject);
        }

        den = gameObject.transform.localPosition;

        
    }

    private void OnEnable()
    {
        GameEvents.current.onNightTimeStart += OnNightTime;
        GameEvents.current.onNightTimeEnd += OnDayTime;
    }

    private void OnDisable()
    {
        GameEvents.current.onNightTimeStart -= OnNightTime;
        GameEvents.current.onNightTimeEnd -= OnDayTime;
    }

    // Update is called once per frame
    void Update()
    {

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                ChangeAnimationState(_goHome);
            }
        }
    }
    private void OnNightTime()
    {
        goHome= false;
    }
    private void OnDayTime()
    {
        goHome = true;
        agent.SetDestination(den);
    }


    private void ChangeAnimationState(string newState)
    {
        if (newState == _currentState)
        {
            return;
        }
        _animator.Play(newState);
        _currentState = newState;
    }
}
