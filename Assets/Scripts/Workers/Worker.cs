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

    private NavMeshAgent agent;

    /// <summary>
    /// UI
    /// </summary>
    private GameObject canvas;
    private Slider progressSlider;

    /// <summary>
    /// Worker Information
    /// </summary>
    [SerializeField] private string workerName;

    public HarvestObjectManager interactingWith;

    [SerializeField] private DefaultWorkerJobs defaultWorkerJobs;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        canvas = transform.Find("Canvas").gameObject;
        progressSlider = canvas.transform.Find("Slider").GetComponent<Slider>();
    }
    
    public IEnumerator MoveToJob(Worker worker, HarvestObjectManager harvestObjectManager)
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
            defaultWorkerJobs.BeginHarvest(harvestObjectManager);
            yield return null;
        }
    }
    
}
