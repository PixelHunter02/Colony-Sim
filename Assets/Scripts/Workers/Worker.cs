using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    /// <summary>
    /// The Workers Role will give the worker boosted stats in a specifc craft as well as more abilities linked to that craft.
    /// </summary>
    public enum Role 
    {
        Default,
        Lumberjack,
        Farmer,
        Fighter,
    }
    public Role role;
    
    /// <summary>
    /// The workers Tasks represent the task that the worker is currently doing.
    /// </summary>
    public enum Tasks
    {
        None,
        ChoppingTrees,
        MovingResources,
    }
    public Tasks task;

    
    public enum WorkerStates
    {
        Available,
        Working,
        Sleeping,
        Eating,
    }
    public WorkerStates _workerStates;
    
    private NavMeshAgent agent;

    private IEnumerator distanceCheck;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    
    public void WorkerStateManagement(WorkerStates state, Vector3 movementPoint)
    {
        switch (state)
        {
            case WorkerStates.Available:
                break;
            case WorkerStates.Working:
                agent.SetDestination(movementPoint);
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
        agent.isStopped = true;
        Debug.Log("Reached Destination");
    }
}
