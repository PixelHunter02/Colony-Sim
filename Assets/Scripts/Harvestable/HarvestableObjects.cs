using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestableObjects : MonoBehaviour
{
    [Tooltip("Time To Harvest In Seconds")]
    public float timeToHarvest;
    
    [Tooltip("List Of Roles That Can Interact With The Harvestable")]
    public List<Worker.Role> canInteract;
}
