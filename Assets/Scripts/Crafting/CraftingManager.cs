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

    [SerializeField] private CraftableSO[] craftingRecipes;
    public CraftableSO[] CraftingRecipes => craftingRecipes;
    
    /// <summary>
    /// Assigned in the inspector
    /// </summary>
    [SerializeField] private LayerMask pickupLayermask;

    private void Awake()
    {
        _gameManager = GameManager.Instance;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
    }
    
    public IEnumerator BeginCrafting(CraftableSO craftingRecipe)
    {
        if (VillagerManager.TryGetVillagerByRole(Roles.Crafter, out Villager villager))
        {
            List<Item> resourcesToRemove = new List<Item>();

            foreach (var required in craftingRecipe.requiredResource)
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

            yield return StartCoroutine(WalkToCraftingBench(villager,craftingRecipe));
            
            StorageManager.UpdateStorage();
        }
    }

    private IEnumerator PickUpItems(Villager assignedVillager, Item location)
    {
        // Allow The Villager to move and set a destination.
        Villager.StopVillager(assignedVillager,false);
        Villager.SetVillagerDestination(assignedVillager, location.storageLocation);
        assignedVillager.CurrentState = VillagerStates.Walking;
       
        while (!Physics.Raycast(new Vector3(assignedVillager.transform.position.x,assignedVillager.transform.position.y+1.5f,assignedVillager.transform.position.z),Vector3.down*2,10,pickupLayermask))
        {
            yield return null;
        }
        assignedVillager.CurrentState = VillagerStates.Pickup;
        Debug.Log("Hit");
        Villager.StopVillager(assignedVillager, true);
        yield return new WaitForSeconds(1f);
        
        StorageManager.EmptyStockpileSpace(location);
    }

    private IEnumerator WalkToCraftingBench(Villager assignedVillager, CraftableSO craftingRecipe)
    {
        var craftingBench = FindAnyObjectByType(typeof(CraftingBench)).GameObject();
        Villager.StopVillager(assignedVillager,false);
        Villager.SetVillagerDestination(assignedVillager, craftingBench.transform.position);
        assignedVillager.CurrentState = VillagerStates.Walking;
       
        while (Vector3.Distance(assignedVillager.transform.position, craftingBench.transform.position) > 2)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2);
        
        
        var location = StorageManager.storageLocations.ElementAt(0);
        StorageManager.UseStorageSpace(location);

        var craftedItem = Instantiate(craftingRecipe.itemToStore.prefab, location, quaternion.identity);
        craftedItem.SetActive(false);
        var itemToAdd = new Item()
        {
            itemSO = craftingRecipe.itemToStore, storageLocation = location, go = craftedItem
        }; 
        _gameManager.storageManager.AddToStorage(itemToAdd);
        yield return StartCoroutine(PlaceCraftedItem(assignedVillager, itemToAdd));
        craftedItem.SetActive(true);
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
        yield return new WaitForSeconds(1f);
    }
}
