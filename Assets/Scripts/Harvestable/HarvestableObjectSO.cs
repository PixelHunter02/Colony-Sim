using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Harvestable Object", fileName = "Harvestable Object")]
public class HarvestableObjectSO : ScriptableObject
{
    [Tooltip("Time To Harvest In Seconds")]
    public float timeToHarvest;
    
    [Tooltip("List Of Roles That Can Interact With The Harvestable")]
    public List<Worker.Roles> canInteract;

    [Tooltip("The prefab of the Harvestable Object")]
    public GameObject prefab;

    [Tooltip("The Type Of Harvesting To Be Done")]
    public HarvestType harvestType; 
    public enum HarvestType
    {
        Chop,
        Forage,
        Pickup
    }
}
