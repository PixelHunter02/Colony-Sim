using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Pickup")]
public class PickUpItemSO : ScriptableObject
{
    public GameObject prefab;
    public string objectName;
    public int maxSize;
    public Sprite uiSprite;
    public AudioClip pickupSound;
    public AudioClip useSound;
}
