using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class HarvestObjectManager : MonoBehaviour, IInteractable
{
    public HarvestableObjectSO harvestableObject;
    
    public Worker assignedWorker;
    
    public void Interact()
    {
        // Run through each worker for an available worker who is of the correct role.
        foreach (var worker in WorkerManager.GetWorkers())
        {
            if (!harvestableObject.canInteract.Contains(Worker.GetWorkerRole(worker)) || worker.interactingWith != null || assignedWorker != null) 
                continue;
            
            if (worker.TryGetComponent(out TaskHandler taskHandler) && Worker.GetWorkerState(worker) == WorkerStates.Idle)
            {
                Worker.SetInteractingWith(worker, this);
                taskHandler.StartCoroutine(taskHandler.CRWalkToTask(worker, this));
            }
            break;
        }
    }
    
    public IEnumerator CRSpawnHarvestDrops()
    {
        const float pushIntensity = 0.5f;
        const float timeBetweenSpawns = 0.3f;
        var min = harvestableObject.minDropAmount;
        var max = harvestableObject.maxDropAmount;

        var numberToSpawn = Random.Range(min, max);
        for (var i = 0; i <= numberToSpawn; i++)
        {
            var drop = Instantiate(harvestableObject.prefabToSpawn, transform.position, Quaternion.identity);
            var x = Random.Range(-180, 180);
            var z = Random.Range(-180, 180);
            var pushDirection = new Vector3(x, 50, z);
            drop.TryGetComponent(out Rigidbody rb);
            rb.AddForce(pushDirection*pushIntensity);
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}
