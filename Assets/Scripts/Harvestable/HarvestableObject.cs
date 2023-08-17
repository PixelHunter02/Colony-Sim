using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class HarvestableObject : MonoBehaviour, IInteractable
{
    public HarvestableObjectSO harvestableObject;

    private GameManager _gameManager;
    
    Villager villagerToAssign;

    private void OnEnable()
    {
        _gameManager = GameManager.Instance;
    }

    public void OnInteraction()
    {
        if (_gameManager.IsOverUI())
        {
            return;
        }
        
        Coroutine task = StartCoroutine(_gameManager.taskHandler.TaskToAssign(this));
        _gameManager.taskHandler.queuedTasks.Enqueue(task);
    }

    public bool CanInteract()
    {
        if (!_gameManager.level.stockpileMode)
        {
            return true;
        }

        return false;
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
            _gameManager.level.villageHeart.GetComponent<VillageHeart>().Experience += 1 * _gameManager.level.ExpBoostAmount;
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
        Destroy(gameObject);
    }
}
