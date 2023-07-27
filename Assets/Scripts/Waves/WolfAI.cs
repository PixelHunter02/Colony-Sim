using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WolfAI : MonoBehaviour
{
    // At night, primarily go after villagers but may attack crops instead
    // During the day, wolves will stay in a den sleeping

    // Other Components
    private NavMeshAgent agent;
    private Animator _animator;
    [SerializeField] private string _currentState;
    [SerializeField] private string _idle = "Idle";
    [SerializeField] private string _moving = "Run Forward In Place";
    [SerializeField] private string _attack = "Bite Attack";

    // Targeting for Combat
    [SerializeField] private List<GameObject> objInTriggerZone;
    [SerializeField] private float distance;
    [SerializeField] private float nearestDistance = 1000;
    public GameObject nearestObject;
    [SerializeField] private GameObject target;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        GameEvents.current.onNightTimeStart += OnNightTime;
        GameEvents.current.onNightTimeEnd += OnDayTime;
    }

    private void OnNightTime()
    {
    }
    private void OnDayTime()
    {
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