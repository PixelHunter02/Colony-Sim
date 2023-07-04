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
}

[System.Serializable]
public sealed class Item

{
    public StoredItemSO itemSO;
    public int amount;
}