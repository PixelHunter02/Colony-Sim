using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class HarvestableObject : MonoBehaviour, IInteractable
{
    public HarvestableObjectSO harvestableObject;

    private GameManager _gameManager;
    public ParticleSystem harvestParticle;
    
    Villager villagerToAssign;

    public delegate void OnHarvestCompleted();
    public static event OnHarvestCompleted onHarvestCompletedEvent;

    public Transform standPoint;
    private UIToolkitManager uIToolkitManager;

    private void Start()
    {
        _gameManager = GameManager.Instance;
        uIToolkitManager = GameObject.Find("UIToolkit").GetComponent<UIToolkitManager>();
    }
    
    public static event Action TutorialStageEight;


    public void OnInteraction()
    {
        if (uIToolkitManager.IsPointerOverUI(_gameManager.inputManager.playerInputActions.UI.Point.ReadValue<Vector2>()))
        {
            return; 
        }
        
        TutorialStageEight?.Invoke();

        VillagerManager.TryGetVillagerByRole(harvestableObject.canInteract, out Villager villager, transform.position);
        
        Debug.Log(villager.VillagerStats.VillagerName);
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
        harvestParticle.Stop();
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
