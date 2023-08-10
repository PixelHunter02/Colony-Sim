using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/StoredResource")]
public class StoredItemSO : ScriptableObject
{
    public GameObject prefab;
    public string objectName;
    public Sprite uiSprite;
    public bool canBePlaced;
    public List<Item> craftingRecipe;
    public string itemDescrition;
    public Roles assignRole;
}

[System.Serializable]
public sealed class Item
{
    public StoredItemSO itemSO;
    public Vector3 storageLocation;
    public GameObject go;
    public int amount;
}
//
// [System.Serializable]
// public class CraftingRecipe
// {
//     public  requiredResource;
// }