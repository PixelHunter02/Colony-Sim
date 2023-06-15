using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Worker : MonoBehaviour
{
    /// <summary>
    /// The Workers Role will give the worker boosted stats in a specifc craft as well as more abilities linked to that craft.
    /// </summary>
    public enum Roles 
    {
        Default,
        Lumberjack,
        Farmer,
        Fighter,
        Miner,
    }
    [SerializeField] private Roles role;

    public enum WorkerStates
    {
        Idle,
        Working,
        Sleeping,
    }
    [SerializeField] private WorkerStates _currentState;

    private NavMeshAgent agent;

    /// <summary>
    /// Worker Information
    /// </summary>
    [SerializeField] private string workerName;

    public HarvestObjectManager interactingWith;

    [SerializeField] private TaskHandler taskHandler;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public static void ChangeWorkerState(Worker worker, WorkerStates newState)
    {
        worker._currentState = newState;
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
