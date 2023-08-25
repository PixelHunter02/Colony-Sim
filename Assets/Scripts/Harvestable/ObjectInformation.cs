using System.Collections;
using System.Linq;
using UnityEngine;

public class ObjectInformation : MonoBehaviour, IStorable
{
    [SerializeField] private StoredItemSO _itemSO;
    public StoredItemSO Item
    {
        get => _itemSO;
    }
    public bool _isStored;
    public bool _isHeld;
    public Vector3 storageLocation;
    public Villager assignedVillager;
    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = GameManager.Instance;
    }

    private void Update()
    {
        if (!CanBeStored())
            return;

        FindAvailableVillager();
    }

    public void AssignStorage()
    {
        var location = StorageManager.storageLocations.ElementAt(0);
        storageLocation = location;
        StorageManager.UseStorageSpace(location);
    }
    
    private void FindAvailableVillager()
    {
        AssignStorage();
        _isHeld = true;

        var villager = VillagerManager.villagers[0];
        IEnumerator cr = _gameManager.taskHandler.RunTaskCR(villager, this);
        villager.villagerQueue.Enqueue(cr);

        //Coroutine cr = StartCoroutine(_gameManager.taskHandler.TaskToAssign(this));
        //_gameManager.taskHandler.queuedTasks.Enqueue(cr);

    }
    
    private bool CanBeStored() => StorageManager.HasStorageSpace() && !_isHeld && !_isStored;
}
