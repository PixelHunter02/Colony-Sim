using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
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
    public Roles role;
    
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
    
    /// <summary>
    /// The Workers State, I.e working or sleeping
    /// </summary>
    public enum WorkerStates
    {
        Available,
        Working,
        Sleeping,
        Eating,
        Collecting
    }
    public WorkerStates workerStates;
    
    private NavMeshAgent agent;

    /// <summary>
    /// UI
    /// </summary>
    private GameObject canvas;
    private Slider progressSlider;
    

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        canvas = transform.Find("Canvas").gameObject;
        progressSlider = canvas.transform.Find("Slider").GetComponent<Slider>();
    }

    public void WorkerRole()
    {
        switch (role)
        {
            case Roles.Default:
                
                break;
            case Roles.Farmer:
                break;
            case Roles.Fighter:
                break;
            case Roles.Miner:
                break;
            case Roles.Lumberjack:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public void WorkerStateManagement(WorkerStates state, GameObject target)
    {
        switch (state)
        {
            case WorkerStates.Available:
                break;
            case WorkerStates.Working:
                var position = target.transform.position;
                agent.SetDestination(position);
                StartCoroutine(DistanceCheck(position,target));
                break;
            case WorkerStates.Collecting:
                agent.SetDestination(target.transform.position);
                break;
        }
    }

    private IEnumerator DistanceCheck(Vector3 movementPoint,GameObject target)
    {
        Debug.Log(Vector3.Distance(transform.position, movementPoint));
        while (Vector3.Distance(transform.position, movementPoint) > 3f)
        {
            yield return null;
        }
        agent.isStopped = true;
        Debug.Log("Reached Destination");
        StartCoroutine(BeginHarvest(target.GetComponent<ObjectManager>()._harvestableObject.timeToHarvest, target.gameObject));
    }

    private IEnumerator BeginHarvest(float duration, GameObject objectToRemove)
    {
        canvas.SetActive(true);
        float timer = 0;
        while (timer < duration)
        {
            Debug.Log(timer);
            timer += Time.deltaTime;
            progressSlider.value = timer / duration;
            yield return null;
        }
        Destroy(objectToRemove);
        canvas.SetActive(false);
        progressSlider.value = 0;
        workerStates = WorkerStates.Available;
        agent.ResetPath();
        agent.isStopped = false;
    }
}
