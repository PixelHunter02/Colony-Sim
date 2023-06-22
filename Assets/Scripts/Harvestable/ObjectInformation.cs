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
    public bool _isStored;
    public bool _isHeld;
    public Vector3 storageLocation;
    
    public void OnInteraction()
    {
        if(!CanBeStored())
            return;

        FindAvailableVillager();
        // AssignStorage();
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
    private void FindAvailableVillager()
    {
        foreach (var villager in VillagerManager.GetVillagers())
        {
            if (villager.CurrentState == VillagerStates.Idle &&
                villager.TryGetComponent(out TaskHandler taskHandler) && !_isHeld && !villager.currentlyHolding)
            {
                AssignStorage();
                taskHandler.PickUpResource(villager, this);
                break;
            }
        }
    }
    
    private bool CanBeStored() => StorageManager.HasStorageSpace() && !_isHeld && !_isStored;
}
