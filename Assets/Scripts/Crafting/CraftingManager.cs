using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    
    private GameManager _gameManager;

    [SerializeField] private StoredItemSO[] craftingRecipes;
    public StoredItemSO[] CraftingRecipes => craftingRecipes;
    
    /// <summary>
    /// Assigned in the inspector
    /// </summary>
    [SerializeField] private LayerMask pickupLayermask;

    public Queue<IEnumerator> craftingQueue;
    private List<Roles> craftingRoles;

    public Dictionary<IEnumerator,GameObject> craftingQueueDictionary;
    
    private void Awake()
    {
        craftingQueue = new Queue<IEnumerator>();
        craftingQueueDictionary = new Dictionary<IEnumerator, GameObject>();
        _gameManager = GameManager.Instance; 
        craftingRoles = new List<Roles>() { Roles.Crafter, Roles.Leader };
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        StartCoroutine(RunCraftingQueue());
    }

    private IEnumerator RunCraftingQueue()
    {
        while (craftingQueue?.Count > 0)
        {
            if (VillagerManager.TryGetVillagerByRole(craftingRoles, out Villager villager)){
                var queueToRemove = craftingQueue.Peek();
                yield return StartCoroutine(craftingQueue.Dequeue());
                Destroy(craftingQueueDictionary[queueToRemove]);
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        StartCoroutine(RunCraftingQueue());
    }
    
    public IEnumerator BeginCrafting(StoredItemSO craftingRecipe)
    {
        if (VillagerManager.TryGetVillagerByRole(craftingRoles, out Villager villager))
        {
            villager.StopAllCoroutines();
            List<Item> resourcesToRemove = new List<Item>();

            foreach (var required in craftingRecipe.craftingRecipe)
            {
                if (StorageManager.TryFindItemsInInventory(required, required.amount, out List<Item> resources))
                {
                    foreach (var resource in resources)
                    {
                        Debug.Log(resource.itemSO.objectName);
                        resourcesToRemove.Add(resource);
                    }
                }
                else
                {
                    Debug.Log("You dont have required resources");
                    yield break;
                }
            }
            
            Debug.Log("Has Resources");

            foreach (var item in resourcesToRemove)
            {
                yield return StartCoroutine(PickUpItems(villager, item));
            }

            yield return StartCoroutine(WalkToVillageHeart(villager,craftingRecipe));
            
            StorageManager.UpdateStorage();
            villager.CurrentState = VillagerStates.Idle;
            villager.StartCoroutine(villager.RandomWalk(4));
        }
        else
        {
            Debug.Log("Cant find anyone");
        }
    }

    private IEnumerator PickUpItems(Villager assignedVillager, Item location)
    {
        // Allow The Villager to move and set a destination.
        Villager.StopVillager(assignedVillager,false);
        Villager.SetVillagerDestination(assignedVillager, location.storageLocation);
        assignedVillager.CurrentState = VillagerStates.Walking;
       
        // while (!Physics.Raycast(new Vector3(assignedVillager.transform.position.x,assignedVillager.transform.position.y+1.5f,assignedVillager.transform.position.z),Vector3.down*2,10,pickupLayermask))
        while (Vector3.Distance(assignedVillager.transform.position,location.storageLocation) > 1f)
        {
            yield return null;
        }
        assignedVillager.CurrentState = VillagerStates.Pickup;
        Debug.Log("Hit");
        Villager.StopVillager(assignedVillager, true);
        yield return new WaitForSeconds(1f);
        
        StorageManager.EmptyStockpileSpace(location);
    }

    private IEnumerator WalkToVillageHeart(Villager assignedVillager, StoredItemSO craftingRecipe)
    {
        var villageHeart = FindAnyObjectByType(typeof(VillageHeart)).GameObject();
        Villager.StopVillager(assignedVillager,false);
        Villager.SetVillagerDestination(assignedVillager, villageHeart.transform.position);
        assignedVillager.CurrentState = VillagerStates.Walking;
       
        while (Vector3.Distance(assignedVillager.transform.position, villageHeart.transform.position) > 2)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2);
        
        
        var location = StorageManager.storageLocations.ElementAt(0);
        StorageManager.UseStorageSpace(location);

        var craftedItem = Instantiate(craftingRecipe.prefab, location, quaternion.identity);
        craftedItem.SetActive(false);
        var itemToAdd = new Item()
        {
            itemSO = craftingRecipe, storageLocation = location, go = craftedItem
        }; 
        _gameManager.storageManager.AddToStorage(itemToAdd);
        yield return StartCoroutine(PlaceCraftedItem(assignedVillager, itemToAdd));
        craftedItem.SetActive(true);

        if (_gameManager.level.tutorialManager.TutorialStage == TutorialStage.CraftingTutorial)
        {
            _gameManager.level.tutorialManager.TutorialStage = TutorialStage.VillageHeartTutorial;
        }
    }
    
    private IEnumerator PlaceCraftedItem(Villager assignedVillager, Item location)
    {
        // Allow The Villager to move and set a destination.
        Villager.StopVillager(assignedVillager,false);
        Villager.SetVillagerDestination(assignedVillager, location.storageLocation);
        assignedVillager.CurrentState = VillagerStates.Walking;
       
        while (Vector3.Distance(assignedVillager.transform.position,location.storageLocation) > 1f)
        {
            yield return null;
        }
        assignedVillager.CurrentState = VillagerStates.Pickup;
        Villager.StopVillager(assignedVillager, true);
        
        
        _gameManager.level.villageHeart.GetComponent<VillageHeart>().Experience += assignedVillager.Craft;
        yield return new WaitForSeconds(1f);
    }
}
