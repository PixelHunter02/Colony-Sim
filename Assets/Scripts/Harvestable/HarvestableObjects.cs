using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class HarvestableObjects : MonoBehaviour
{
    [Tooltip("The Type Of Resource Being Harvested")]
    public enum HarvestType
    {
        Tree,
        Stone,
        Berries,
        Pickup,
    }

    public HarvestType harvestType;
    
    [Tooltip("Time To Harvest In Seconds")]
    public float timeToHarvest;
    
    [Tooltip("List Of Roles That Can Interact With The Harvestable")]
    public List<Worker.Role> canInteract;

    public GameObject wood;

    private void OnDestroy()
    {
        Instantiate(wood, new Vector3(transform.position.x, -1.5f, transform.position.z),quaternion.identity);
    }
}
