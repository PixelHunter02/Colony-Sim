using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class StorageManager : MonoBehaviour
{
    // Locations for Storage
    public static HashSet<Vector3> usedSpaces;
    public static HashSet<Vector3> storageLocations;
    
    private static int _spaceLeft;

    public static List<Item> itemList;

    public Dictionary<StoredItemSO, GameObject> inventorySlot;

    [SerializeField] private GameObject inventoryTemplate;
    [SerializeField] private Transform inventoryMenu;
    private GameManager _gameManager;
    
    Dictionary<StoredItemSO, int> itemsInList;

    public static event Action DrawInventoryAction;
    private void Awake()
    {
        storageLocations = new HashSet<Vector3>();
        usedSpaces = new HashSet<Vector3>();
        itemList = new List<Item>();
        _gameManager = GameManager.Instance;
        itemsInList = new Dictionary<StoredItemSO, int>();
        inventorySlot = new Dictionary<StoredItemSO, GameObject>();
    }

    private void OnEnable()
    {
        Stockpile.OnCreateStorageCellEvent += UpdateStorage;
        DrawInventoryAction += DrawInventory;
    }
    
    private void OnDisable()
    {
        Stockpile.OnCreateStorageCellEvent -= UpdateStorage;
        DrawInventoryAction += DrawInventory;
    }
    
    // Used To Add An Item To Storage
    public void AddToStorage(Item itemToAdd)
    {
        itemList.Add(itemToAdd);
        DrawInventory();
    }

    // Create the buttons for the inventory
    public void DrawInventory()
    {
        itemsInList.Clear();
        foreach (var item in itemList)
        {
            if (!itemsInList.ContainsKey(item.itemSO) && !inventorySlot.ContainsKey(item.itemSO))
            {
                // Create a new slot
                GameObject slot = Instantiate(inventoryTemplate, inventoryMenu);
                if (item.itemSO.canBePlaced)
                {
                    slot.GetComponent<Button>().onClick.AddListener( () => _gameManager.buildingManager.currentBuilding = item.itemSO);
                }
                inventorySlot.TryAdd(item.itemSO,slot);
                var image = slot.transform.GetChild(0);
                var count = image.GetChild(0);

                // Add item to slot
                itemsInList.Add(item.itemSO, 1);
                image.gameObject.SetActive(true);
                count.gameObject.SetActive(true);
                
                // Assign Image and count
                image.GetComponent<Image>().sprite = item.itemSO.uiSprite;
                count.GetComponent<TMP_Text>().text = itemsInList[item.itemSO].ToString();
            }
            else if(!itemsInList.ContainsKey(item.itemSO))
            {
                itemsInList.Add(item.itemSO, 1);
                var slot = inventorySlot[item.itemSO];
                var image = slot.transform.GetChild(0);
                var count = image.GetChild(0);
                
                count.GetComponent<TMP_Text>().text = itemsInList[item.itemSO].ToString();
            }
            else 
            {
                var slot = inventorySlot[item.itemSO];
                var image = slot.transform.GetChild(0);
                var count = image.GetChild(0);
                
                itemsInList[item.itemSO]++;
                count.GetComponent<TMP_Text>().text = itemsInList[item.itemSO].ToString();
            }
        }

        foreach (var item in inventorySlot)
        {
            if (!itemsInList.ContainsKey(item.Key))
            {
                Destroy(item.Value);
            }
        }
        
    }

    public static bool TryFindItemsInInventory(Item itemType, out Item itemToReturn)
    {
        foreach (var item in itemList)
        {
            if (itemType.itemSO == item.itemSO)
            {
                itemToReturn = item;
                return true;
            }
        }

        itemToReturn = null;
        return false;
    }
    
    public static bool TryFindItemsInInventory(Item itemType, int requiredAmount, out List<Item> itemToReturn)
    {
        itemToReturn = new List<Item>();
        int count = 0;
        foreach (var item in itemList)
        {
            if (itemType.itemSO == item.itemSO && count<requiredAmount)
            {
                itemToReturn.Add(item);
                count++;
            }
        }

        if (itemToReturn.Count >= requiredAmount)
        {
            return true;
        }
        
        itemToReturn = null;
        return false;
    }

    public static void EmptyStockpileSpace(Item resourceToRemove)
    {
        storageLocations.Add(resourceToRemove.storageLocation);
        usedSpaces.Remove(resourceToRemove.storageLocation);
        itemList.Remove(resourceToRemove);
        DestroyImmediate(resourceToRemove.go);
        UpdateStorage();
        DrawInventoryAction?.Invoke();
    }
    
    public static void UseStorageSpace(Vector3 location)
    {
        storageLocations.Remove(location);
        usedSpaces.Add(location);
        UpdateStorage();
    }
    
    public static void UpdateStorage() => _spaceLeft = storageLocations.Count;

    public static bool HasStorageSpace() => _spaceLeft > 0;
}
