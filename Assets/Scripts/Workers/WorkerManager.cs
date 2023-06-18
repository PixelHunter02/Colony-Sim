using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    [SerializeField] private static List<Worker> workers;

    private void Awake()
    {
        workers = new List<Worker>();
        foreach (var worker in FindObjectsOfType(typeof(Worker)))
        {
            workers.Add(worker.GetComponent<Worker>());
        }
    }

    public static List<Worker> GetWorkers()
    {
        return workers;
    }
}
