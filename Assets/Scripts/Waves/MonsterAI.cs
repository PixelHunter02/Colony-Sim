using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;

public class MonsterAI : MonoBehaviour
{
    private NavMeshAgent agent;

    public float health;
    
    public GameObject target;

    public bool inRange;

    public float coolDown;

    private Animator _animator;
    [SerializeField] private string _currentState;
    [SerializeField] private string _idle = "Idle";
    [SerializeField] private string _moving = "Run Forward In Place";
    [SerializeField] private string _attack = "Bite Attack";

    private void Start()
    {
        health = 100;
        _animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        GameObject[] villagers = GameObject.FindGameObjectsWithTag("Villagers");
        target = villagers[Random.Range(0,3)];
        //randomly selecting a target cuz too tired to do it based off closest

    }

    void Update()
    {
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
