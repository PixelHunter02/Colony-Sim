using System.Linq;
using UnityEditor;
using UnityEngine;

public class ObjectInformation : MonoBehaviour, IStorable, IInteractable
{
    [SerializeField] private PickUpItemSO _itemSO;
    public PickUpItemSO Item
    {
        get => _itemSO;
    }
    private bool _isStored;
    private bool _isHeld;
    public Vector3 storageLocation;
    
    public void OnInteraction()
    {
        if(!CanBeStored())
            return;    
        
        AssignStorage();
        FindAvailableWorker();
    }
    
    public void AssignStorage()
    {
        var location = StorageManager.storageLocations.ElementAt(0);
        storageLocation = location;
        StorageManager.storageLocations.Remove(location);
        StorageManager.usedSpaces.Add(location);
        StorageManager.UpdateStorage();
        _isStored = true;
    }
    private void FindAvailableWorker()
    {
        foreach (var worker in WorkerManager.GetWorkers())
        {
            if (worker.CurrentState == WorkerStates.Idle &&
                worker.TryGetComponent(out TaskHandler taskHandler) && !_isHeld)
            {
                taskHandler.PickupObject(this);
                break;
            }
        }
    }
    
    private bool CanBeStored() => StorageManager.HasStorageSpace() && !_isHeld && !_isStored;
}
