using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class HarvestObjectManager : MonoBehaviour, IInteractable
{
    public HarvestableObjectSO harvestableObject;
    
    [FormerlySerializedAs("assignedWorker")] public Villager assignedVillager;
    
    public void OnInteraction()
    {
        // Run through each worker for an available worker who is of the correct role.
        foreach (var worker in VillagerManager.GetVillagers())
        {
            if (!harvestableObject.canInteract.Contains(worker.CurrentRole) || worker.interactingWith != null || assignedVillager != null) 
                continue;
            
            if (worker.TryGetComponent(out TaskHandler taskHandler) && worker.CurrentState == VillagerStates.Idle)
            {
                worker.interactingWith = this;
                StartCoroutine(taskHandler.VillagerWalksToTask(worker,this));
                // taskHandler.StartCoroutine(taskHandler.CRWalkToTask(worker, this));
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
