using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TaskHandler : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image taskImage;
    [SerializeField] private GameObject status;
    [SerializeField] private Worker _worker;
 
    
    public IEnumerator CRRunTask(HarvestObjectManager harvestObjectManager)
    {
        status.SetActive(true);
        var timer = 0f;
        var timeToCount = harvestObjectManager.harvestableObject.timeToHarvest;
        taskImage.sprite = harvestObjectManager.harvestableObject.taskSprite;
        while (timer < timeToCount)
        {
            timer += Time.deltaTime;
            slider.value = timer / timeToCount;
            yield return null;
        }
        
        taskImage.sprite = harvestObjectManager.harvestableObject.taskCompleteSprite;
        StartCoroutine(harvestObjectManager.CRSpawnHarvestDrops());
        Worker.StopWorker(_worker, false);
        _worker.CurrentState = WorkerStates.Idle;
        yield return new WaitForSeconds(3f);
        status.SetActive(false);
        _worker.interactingWith = null;
        harvestObjectManager.assignedWorker = null;
        
    }

    public IEnumerator CRWalkToTask(Worker worker, HarvestObjectManager harvestObjectManager)
    {
        if (Vector3.Distance(transform.position, harvestObjectManager.transform.position) > 3f)
        {
            if (worker.CurrentState != WorkerStates.Walking)
            {
                // Set the Workers State To Walking
                _worker.CurrentState = WorkerStates.Walking;
            }
            Worker.StopWorker(worker, false);
            Worker.SetWorkerDestination(worker,harvestObjectManager.transform.position);

            yield return new WaitForSeconds(0.3f);
            StartCoroutine(CRWalkToTask(worker, harvestObjectManager));
        }
        else
        {
            Worker.StopWorker(worker,true);
            worker.CurrentState = WorkerStates.Working;
            StartCoroutine(CRRunTask(harvestObjectManager));
            yield return null;
        }
    }

    public void PickupObject(ObjectInformation objectToPickup)
    {
        StartCoroutine(CRWalkToPickup(_worker, objectToPickup));
    }
    
    private IEnumerator CRWalkToPickup(Worker worker, ObjectInformation objectInformation)
    {
        var pickupPosition = objectInformation.transform.position;

        worker.CurrentState = WorkerStates.Walking;
        Worker.StopWorker(worker, false);
        Worker.SetWorkerDestination(worker, pickupPosition);
        
        while (Vector3.Distance(worker.transform.position,pickupPosition) > 3f)
        {
            yield return new WaitForSeconds(0.5f);
        }
        
        objectInformation.gameObject.SetActive(false);
        
        StartCoroutine(CRWalkToStockpile(worker, objectInformation));
    }

    private static IEnumerator CRWalkToStockpile(Worker worker, ObjectInformation objectInformation)
    {
        Worker.SetWorkerDestination(worker, objectInformation.storageLocation);
        worker.CurrentState = WorkerStates.Walking;
        
        // To Check The Distance Between The Worker And The Object Storage Location
        while (Vector3.Distance(worker.transform.position, objectInformation.storageLocation) > 1)
        {
            Debug.Log("In While Loop");
            yield return new WaitForSeconds(0.5f);
        }
        
        worker.CurrentState = WorkerStates.Idle;
     
        MoveObjectToStorage(objectInformation);
    }

    private static void MoveObjectToStorage(ObjectInformation objectInformation)
    {
        objectInformation.transform.position = objectInformation.storageLocation;
        objectInformation.gameObject.SetActive(true);
        objectInformation.transform.rotation = Quaternion.Euler(0,0,0);
        StorageManager.AddToStorage(new Resource{itemSO = objectInformation.Item, amount = 1});
    }
}
