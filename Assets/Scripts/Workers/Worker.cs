using System.Collections;

using UnityEngine;
using UnityEngine.AI;
using Slider = UnityEngine.UI.Slider;

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
        progressSlider = canvas.transform.Find("Slider").gameObject.GetComponent<Slider>();
    }
    
    public void WorkerStateManagement(WorkerStates state, GameObject target)
    {
        switch (state)
        {
            case WorkerStates.Available:
                break;
            case WorkerStates.Working:
                agent.SetDestination(target.transform.position);
                StartCoroutine(DistanceCheck(target.transform.position,target));
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
        StartCoroutine(BeginHarvest(target.GetComponent<HarvestableObjects>().timeToHarvest));
    }

    private IEnumerator BeginHarvest(float duration)
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
        canvas.SetActive(false);
        progressSlider.value = 0;
        workerStates = WorkerStates.Available;
        agent.ResetPath();
        agent.isStopped = false;
    }
}
