using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

public class MonsterAI : MonoBehaviour
{
    private NavMeshAgent agent;

    public float health;
    
    public GameObject target;

    public GameObject nearestObject;

    public bool inRange;

    public float coolDown;

    [SerializeField] private float distance;
    [SerializeField] private float nearestDistance;

    private Animator _animator;
    [SerializeField] private string _currentState;
    [SerializeField] private string _idle = "Idle";
    [SerializeField] private string _moving = "Run Forward In Place";
    [SerializeField] private string _attack = "Bite Attack";

    private void Start()
    {
        //target nearest crop, if no crop, target person
        //if target unreachable, destroy structure blocking way when collide ig?
        health = 100;
        _animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        GameObject[] crops = GameObject.FindGameObjectsWithTag("Crops");
        GameObject[] villagers = GameObject.FindGameObjectsWithTag("Villagers");

        for (int i = 0; i < crops.Length; i++)
        {
            //check for closest crop
            distance = Vector3.Distance(this.transform.position, crops[i].transform.position);
            if (distance < nearestDistance)
            {
                nearestObject = crops[i];
                nearestDistance = distance;
                target = nearestObject;
            }
        }
        //if no crops are available
        if (crops == null)
        {
            for (int i = 0; i < villagers.Length; i++)
            {
                //check for closest villager
                distance = Vector3.Distance(this.transform.position, villagers[i].transform.position);
                if (distance < nearestDistance)
                {
                    nearestObject = villagers[i];
                    nearestDistance = distance;
                    target = nearestObject;
                }
            }
        }
        //target = villagers[Random.Range(0,3)];
    }

    void Update()
    {
        if (health < 0)
        {
            Destroy(gameObject);
        }
        if (!inRange)//not in range, move closer
        {
            ChangeAnimationState(_moving);
            agent.SetDestination(target.transform.position);
        }
        else if (inRange)//in range, attack
        {
            if (coolDown <= 0)
            {
                _animator.Play(_attack);
                print("attacking");
                target.GetComponent<Villager>().health -= 5;
                print(target.GetComponent<Villager>().VillagerName + " Health: " + target.GetComponent<Villager>().health);
                coolDown = 3;
            }

            coolDown -= Time.deltaTime;
        }
        //else if (inRange && coolDown > 0)
        //{
        //    ChangeAnimationState(_idle);
        //}
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
    

    //approach

    //attack
    
    //die
}
