using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Harvestable Object", fileName = "Harvestable Object")]
public class HarvestableObjectSO : ScriptableObject
{
    [Tooltip("Time To Harvest In Seconds")]
    public float timeToHarvest;
    
    [Tooltip("List Of Roles That Can Interact With The Harvestable")]
    public List<Worker.Roles> canInteract;

    [Tooltip("The prefab of the resource gained on harvest")]
    public GameObject prefabToSpawn;

    [Tooltip("The maximum drop amount from the HarvestObject")]
    public int maxDropAmount;
    
    [Tooltip("The minimum drop amount from the HarvestObject")]
    public int minDropAmount;

    [Tooltip("The Sprite To Display While The Task is in progress")]
    public Sprite taskSprite;
    [Tooltip("The Sprite To Display While The Task is Complete")]
    public Sprite taskCompleteSprite;
    
    [Tooltip("The Type Of Harvesting To Be Done")]
    public HarvestType harvestType; 
    public enum HarvestType
    {
        Chop,
        Forage,
        Pickup
    }
}
