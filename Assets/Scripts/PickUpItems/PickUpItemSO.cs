using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Pickup")]
public class PickUpItemSO : ScriptableObject
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private string objectName;
    [SerializeField] private int maxSize;
    [SerializeField] private Sprite uiSprite;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip useSound;
}
