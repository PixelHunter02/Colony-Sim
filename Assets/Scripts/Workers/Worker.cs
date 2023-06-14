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

    /// <summary>
    /// Worker Information
    /// </summary>
    private string workerName;

    public HarvestObjectManager interactingWith;

    [SerializeField] private DefaultWorkerJobs defaultWorkerJobs;

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
                break;
            case WorkerStates.Collecting:
                break;
            case WorkerStates.Sleeping:
                break;
            case WorkerStates.Eating:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
    
    // private IEnumerator BeginHarvest(float duration, GameObject objectToRemove)
    // {
    //     canvas.SetActive(true);
    //     float timer = 0;
    //     while (timer < duration)
    //     {
    //         Debug.Log(timer);
    //         timer += Time.deltaTime;
    //         progressSlider.value = timer / duration;
    //         yield return null;
    //     }
    //     Destroy(objectToRemove);
    //     canvas.SetActive(false);
    //     progressSlider.value = 0;
    //     workerStates = WorkerStates.Available;
    //     agent.ResetPath();
    //     agent.isStopped = false;
    // }

    public IEnumerator MoveToJob(Worker worker,HarvestObjectManager harvestObjectManager)
    {
        if (Vector3.Distance(worker.transform.position, harvestObjectManager.transform.position) > 3f)
        {
            agent.SetDestination(harvestObjectManager.transform.position);
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(MoveToJob(worker, harvestObjectManager));
        }
        else
        {
            agent.isStopped = true;
            defaultWorkerJobs.ChopTrees(harvestObjectManager);
            yield return null;
        }
    }
    
}
