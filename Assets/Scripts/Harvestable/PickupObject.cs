using UnityEngine;

public class PickupObject : MonoBehaviour, IInteractable
{
    public Vector3 storedAt;
    [SerializeField] private bool isBeingHeld;
    public Worker heldBy;

    public void Pickup(Worker workerToPickUp)
    {
        isBeingHeld = true;
        heldBy = workerToPickUp;
    }

    public void Interact()
    {
        if(!StorageManager.HasStorageSpace())
            return;
        
        // Run through each worker for an available worker who is of the correct role.
        foreach (var worker in WorkerManager.GetWorkers())
        {
            if (Worker.GetWorkerState(worker) != WorkerStates.Idle ||
                !worker.TryGetComponent(out TaskHandler taskHandler) || heldBy) continue;
            Pickup(worker);
            StorageManager.StoreItem(this);
            StartCoroutine(taskHandler.CRWalkToPickup(worker,this));
            break;
        }
    }
}
