using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class HarvestableObject : MonoBehaviour, IInteractable
{
    public HarvestableObjectSO harvestableObject;

    private GameManager _gameManager;
    
    Villager villagerToAssign;

    public delegate void OnHarvestCompleted();
    public static event OnHarvestCompleted onHarvestCompletedEvent;
    
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

        VillagerManager.TryGetVillagerByRole(harvestableObject.canInteract, out Villager villager);
        
        IEnumerator cr = _gameManager.taskHandler.RunTaskCR(villager, this);
        villager.villagerQueue.Enqueue(cr);
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
        var position = transform.position;
        var rotation = Quaternion.identity;

        var numberToSpawn = Random.Range(min, max);
        for (var i = 0; i <= numberToSpawn; i++)
        {
            var drop = Instantiate(harvestableObject.prefabToSpawn, position, rotation);
            var x = Random.Range(-180, 180);
            var z = Random.Range(-180, 180);
            var pushDirection = new Vector3(x, 50, z);
            drop.TryGetComponent(out Rigidbody rb);
            rb.AddForce(pushDirection*pushIntensity);
            _gameManager.level.villageHeart.GetComponent<VillageHeart>().Experience += 1 * _gameManager.level.ExpBoostAmount;
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}
