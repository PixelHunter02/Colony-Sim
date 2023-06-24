using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TaskHandler : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image taskImage;
    [SerializeField] private GameObject status;
    private GameManager _gameManager;

    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Villager Walks To Task
    public IEnumerator VillagerWalksToTask(Villager assignedVillager, HarvestObjectManager task)
    {
        Debug.Log("walking to task");
        BeginWalking(assignedVillager,task);
        
        // Check the distance between the Villager and the task
        while (Vector3.Distance(assignedVillager.transform.position, task.transform.position) > 3)
        {
            yield return null;
        }
        
        // Stop The Villager
        Villager.StopVillager(assignedVillager,true);
        
        StartCoroutine(VillagerDoesTaskCR(assignedVillager, task));
    }

    // Villager Does Task
    private IEnumerator VillagerDoesTaskCR(Villager assignedVillager, HarvestObjectManager task)
    {
        Debug.Log("Doing Task");
        // Set Slider To be Visible
        status.SetActive(true);
        taskImage.sprite = task.harvestableObject.taskSprite;
        
        // Set The Villager To The Working State
        assignedVillager.CurrentState = VillagerStates.Working;

        // Get The Current Progress Of The Task
        float timer = 0;
        var duration = task.harvestableObject.timeToHarvest;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            var progress = timer / duration;
            slider.value = progress;
            yield return null;
        }

        // Show The Task Has Been Completed
        taskImage.sprite = task.harvestableObject.taskCompleteSprite;
        StartCoroutine(task.CRSpawnHarvestDrops());
        
        //Set The Villager To Its  Idle State
        Villager.StopVillager(assignedVillager, false);
        assignedVillager.CurrentState = VillagerStates.Idle;
        
        // Disable The Canvas And Un-Assign Tasks
        yield return new WaitForSeconds(1.5f);
        assignedVillager.interactingWith = null;
        task.assignedVillager = null;
        status.SetActive(false);
    }

    public void PickUpResource(Villager assignedVillager, ObjectInformation resourceToPickUp)
    {
        StartCoroutine(VillagerWalksToResourceCR(assignedVillager, resourceToPickUp));
    }
    
    // Worker Walks To Item
    private IEnumerator VillagerWalksToResourceCR(Villager assignedVillager, ObjectInformation resourceToPickUp)
    {
        Debug.Log("walking to resource");
        BeginWalking(assignedVillager, resourceToPickUp);

        while (Vector3.Distance(assignedVillager.transform.position, resourceToPickUp.transform.position) > 3f)
        {
            yield return null;
        }    
        
        // Stop The Villager
        Villager.StopVillager(assignedVillager,true);
        assignedVillager.CurrentState = VillagerStates.Idle;

        StartCoroutine(VillagerPicksUpItemCR(assignedVillager, resourceToPickUp));
    }
    
    //Worker Picks Up Item
    private IEnumerator VillagerPicksUpItemCR(Villager assignedVillager, ObjectInformation resourceToPickUp)
    {
        Debug.Log("Picking Up Resource");
        assignedVillager.currentlyHolding = resourceToPickUp.Item;
        resourceToPickUp._isHeld = true;

        StartCoroutine(VillagerWalksToStockpilePointCR(assignedVillager, resourceToPickUp));
        yield return null;
    }
    
    //Worker walks to Stockpile Point
    private IEnumerator VillagerWalksToStockpilePointCR(Villager assignedVillager, ObjectInformation resourceToPickUp)
    {
        Debug.Log("Walking To Stockpile");
        // Set the villagers state to walking if not already.
        if (assignedVillager.CurrentState != VillagerStates.Walking && Vector3.Distance(assignedVillager.transform.position, resourceToPickUp.storageLocation) > 3f)
        {
            assignedVillager.CurrentState = VillagerStates.Walking;
        }

        yield return new WaitForSeconds(0.1f);
        // Allow The Villager to move and set a destination.
        Villager.StopVillager(assignedVillager,false);
        Villager.SetVillagerDestination(assignedVillager, resourceToPickUp.storageLocation);   
        
        while (Vector3.Distance(assignedVillager.transform.position, resourceToPickUp.storageLocation) >= 3f)
        {
            yield return null;
        }
        
        // Stop The Villager
        Villager.StopVillager(assignedVillager,true);
        assignedVillager.CurrentState = VillagerStates.Idle;
        
        MoveObjectToStorage(assignedVillager, resourceToPickUp);
    }


    //Worker puts down Item
    private void MoveObjectToStorage(Villager assignedVillager, ObjectInformation objectInformation)
    {
        objectInformation.transform.position = objectInformation.storageLocation;
        objectInformation.gameObject.SetActive(true);
        objectInformation.transform.rotation = Quaternion.Euler(0,0,0);
        objectInformation._isHeld = false;
        assignedVillager.currentlyHolding = null;
        objectInformation._isStored = true;
        _gameManager.storageManager.AddToStorage(new Resource{itemSO = objectInformation.Item, amount = 1});
    }

    private static void BeginWalking(Villager assignedVillager, Component location)
    {
        // Set the villagers state to walking if not already.
        if (assignedVillager.CurrentState != VillagerStates.Walking && Vector3.Distance(assignedVillager.transform.position, location.transform.position) > 3f)
        {
            assignedVillager.CurrentState = VillagerStates.Walking;
        }
        
        // Allow The Villager to move and set a destination.
        Villager.StopVillager(assignedVillager,false);
        Villager.SetVillagerDestination(assignedVillager, location.transform.position);    
    }
}
