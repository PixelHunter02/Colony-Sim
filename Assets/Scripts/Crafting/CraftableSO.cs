using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Scriptable Objects/Crafting Recipe")]
public class CraftableSO : ScriptableObject
{
    public Sprite sprite;
    public Item[] requiredResource;
    public bool instantlyEnterBuildMode;
    public GameObject prefab;
    public StoredItemSO itemToStore;
    public bool isUnlocked;
}

