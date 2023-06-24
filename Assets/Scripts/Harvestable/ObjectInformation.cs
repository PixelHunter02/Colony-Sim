using System.Linq;
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
    public Villager assignedVillager;
    
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
    }
    private void FindAvailableVillager()
    {
        foreach (var villager in VillagerManager.GetVillagers())
        {
            if (villager.CurrentState == VillagerStates.Idle &&
                villager.TryGetComponent(out TaskHandler taskHandler) && !_isHeld && !villager.currentlyHolding && assignedVillager == null)
            {
                AssignStorage();
                assignedVillager = villager;
                StartCoroutine(taskHandler.PickUpResource(villager, this));
                break;
            }
        }
    }
    
    private bool CanBeStored() => StorageManager.HasStorageSpace() && !_isHeld && !_isStored;
}
