using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

public class Monster : MonoBehaviour
{
    public float health;

    // Targeting for Combat
    [SerializeField] private List<GameObject> objInTriggerZone;
    [SerializeField] private List<GameObject> objInAwarenessZone;
    [SerializeField] private float distance;
    [SerializeField] private float nearestDistance = 1000;
    public GameObject nearestObject;
    [SerializeField] private GameObject target;
    
    [SerializeField] private List<GameObject> villagers;

    private NavMeshAgent agent;
    private Animator _animator;
    [SerializeField] private string _currentState;
    [SerializeField] private string _idle = "Idle";
    [SerializeField] private string _moving = "Run Forward In Place";
    [SerializeField] private string _attack = "Bite Attack";

    public float attackCoolDown;
    private bool attackStarted;

    // AG EXAMPLES
    TriggerZone triggerZone;
    AwarenessZone AwarenessZone;

    private GameManager _gameManager;

    private void Awake()
    {
        attackCoolDown = 2;
        villagers = new List<GameObject>();

        _animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        GameEvents.current.onNightTimeStart += OnNightTime;
        GameEvents.current.onNightTimeEnd += OnDayTime;
        
        _gameManager = GameManager.Instance;
    }

    private void Start()
    {
        // AG SET UP REFERENCES
        triggerZone = gameObject.GetComponentInChildren<TriggerZone>();
        AwarenessZone = gameObject.GetComponentInChildren<AwarenessZone>();

        health = 10;
    }

    void Update()
    {
        //objInTriggerZone = gameObject.GetComponentInChildren<TriggerZone>().objInTriggerZone; AG COMMENTED OUT
        objInTriggerZone = triggerZone.objInTriggerZone;
        objInAwarenessZone = AwarenessZone.objInAwarenessZone;

        for (int i = 0; i < objInAwarenessZone.Count; i++)
        {
            if (objInAwarenessZone[i].GetComponent<Villager>())
            {
                StartCoroutine(FindTarget(2));
                //this might be an issue with the day time check
            }
        }
        //waiting for triggers
        objInTriggerZone = triggerZone.objInTriggerZone;
        for (int i = 0; i < objInTriggerZone.Count; i++)
        {
            //print(gameObject.name + " has come into contact with " + objInTriggerZone[i]);
            //print(gameObject.name + " is looking for " + target);
            if (objInTriggerZone[i] == target && !attackStarted)
            {
                attackStarted = true;
                print(gameObject.name + " has found its target");
                StartCoroutine(AttackTarget(attackCoolDown));
            }
        }
        if (target)
        {
            transform.LookAt(target.transform);
        }
    }
    private void OnNightTime()
    {
        StartCoroutine(FindTarget(2));
    }
    private void OnDayTime()
    {
    }

    public IEnumerator FindTarget(float timeTicks)
    {
        yield return new WaitForSeconds(timeTicks);

        foreach (Villager villager in VillagerManager.GetVillagers())
        {
            villagers.Clear();
            if (!villagers.Contains(villager.gameObject))
            {
                villagers.Add(villager.gameObject);
            }
        }

        nearestDistance = 1000;
        for (int i = 0; i < villagers.Count; i++)
        {
            //check for closest villager
            distance = Vector3.Distance(transform.position, villagers[i].transform.position);
            //print(distance);
            //print(nearestDistance);

            if (distance < nearestDistance)
            {
                nearestObject = villagers[i];
                nearestDistance = distance;
                target = nearestObject;
            }
        }

        if (target)
        {
            ChangeAnimationState(_moving);
            agent.SetDestination(target.transform.position);
        }
        StartCoroutine(FindTarget(1));
    }

    private IEnumerator AttackTarget(float timeTicks)
    {//if target unreachable, destroy structure blocking way when collide ig?
        yield return new WaitForSeconds(timeTicks);
        //Debug.Log("Attacking");
        if (target)
        {
            _animator.Play(_attack);
            target.GetComponent<Villager>().health -= 1;
            print(target.GetComponent<Villager>().VillagerName + " Health down to: " + target.GetComponent<Villager>().health);

            if (target.GetComponent<Villager>().health <= 0)
            {
                var villagerComponent = target.GetComponent<Villager>();
                Debug.Log(VillagerManager.villagers.Contains(target.GetComponent<Villager>()));
                VillagerManager.villagers.Remove(villagerComponent);
                villagers.Remove(target.gameObject);
                Destroy(_gameManager.uiManager.templateDictionary[villagerComponent]);
                _gameManager.uiManager.templateDictionary.Remove(villagerComponent);
                Destroy(target.gameObject);

            }
        }
        attackStarted = false;
        StartCoroutine(FindTarget(1));
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
