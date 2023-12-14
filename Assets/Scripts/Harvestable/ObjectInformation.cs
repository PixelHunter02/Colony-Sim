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

        StartCoroutine(FindAvailableVillager());
    }

    public void AssignStorage()
    {
        _storageAssigned = true;
        var location = StorageManager.storageLocations.ElementAt(0);
        storageLocation = location;
        StorageManager.UseStorageSpace(location);
    }
    
    private IEnumerator FindAvailableVillager()
    {
        AssignStorage();

        Villager foundVillager = null;
        while (!foundVillager)
        {
            VillagerManager.TryGetVillagerByRole(out Villager villager, transform.position);
            if (villager)
            {
                foundVillager = villager;
            }

            yield return null;
        }
        IEnumerator cr = Tasks.StoreItem(foundVillager, this);
        Debug.Log(foundVillager);
        foundVillager.villagerQueue.Enqueue(cr);
    }
    
    private bool CanBeStored() => StorageManager.HasStorageSpace() && !_isStored && !_storageAssigned;
}
