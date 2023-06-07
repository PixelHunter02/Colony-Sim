using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tasks : MonoBehaviour
{
    public enum WorkerStates
    {
        Available,
        Working,
        Sleeping,
        Eating,
    }

    public WorkerStates _workerStates;
    
}
