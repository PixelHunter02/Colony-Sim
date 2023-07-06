using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/StoredResource")]
public class StoredItemSO : ScriptableObject
{
    public GameObject prefab;
    public string objectName;
    public int maxSize;
    public Sprite uiSprite;
    public AudioClip pickupSound;
    public AudioClip useSound;
    public bool canBePlaced;
}

[System.Serializable]
public sealed class Item
{
    public StoredItemSO itemSO;
    public Vector3 storageLocation;
    public GameObject go;
    public int amount;
}