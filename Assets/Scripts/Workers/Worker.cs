using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    public Roles roles;
    public Tasks tasks;

    public enum WorkerStates
    {
        Available,
        Working,
        Sleeping,
        Eating,
    }

    public WorkerStates _workerStates;
    private bool walking;
    private NavMeshAgent agent;

    private IEnumerator distanceCheck;

    private void Awake()
    {
        roles = GetComponent<Roles>();
        tasks = GetComponent<Tasks>();
        agent = GetComponent<NavMeshAgent>();
        
    }
    
    public void WorkerStateManagement(WorkerStates state, Vector3 movementPoint, Tasks.Jobs job)
    {
        switch (state)
        {
            case WorkerStates.Available:
                break;
            case WorkerStates.Working:
                agent.SetDestination(movementPoint);
                walking = true;
                StartCoroutine(DistanceCheck(movementPoint));
                break;
        }
    }

    private IEnumerator DistanceCheck(Vector3 movementPoint)
    {
        Debug.Log(Vector3.Distance(transform.position, movementPoint));
        while (Vector3.Distance(transform.position, movementPoint) > 3f)
        {
            yield return null;
        }
        walking = false;
        agent.isStopped = true;
        Debug.Log("Reached Destination");
    }
}
