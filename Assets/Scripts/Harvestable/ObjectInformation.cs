using System.Collections;
using System.Linq;
using SG_Tasks;
using UnityEngine;

public class ObjectInformation : MonoBehaviour, IStorable
{
    [SerializeField] private StoredItemSO _itemSO;
    public StoredItemSO Item
    {
        get => _itemSO;
    }
    public bool _isStored;
    public Vector3 storageLocation;
    public bool _storageAssigned;

    private void Update()
    {
        if (!CanBeStored())
            return;

        FindAvailableVillager();
    }

    public void AssignStorage()
    {
        _storageAssigned = true;
        var location = StorageManager.storageLocations.ElementAt(0);
        storageLocation = location;
        StorageManager.UseStorageSpace(location);
    }
    
    private void FindAvailableVillager()
    {
        AssignStorage();

        VillagerManager.TryGetVillagerByRole(out Villager villager);
        IEnumerator cr = Tasks.StoreItem(villager, this);
        Debug.Log(villager);
        villager.villagerQueue.Enqueue(cr);
    }
    
    private bool CanBeStored() => StorageManager.HasStorageSpace() && !_isStored && !_storageAssigned;
}
