using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class VillagerManager : MonoBehaviour
{
    [SerializeField] private static List<Villager> Villagers;
    // private List<>

    private void Awake()
    {
        Villagers = new List<Villager>();
        foreach (var Villager in FindObjectsOfType(typeof(Villager)))
        {
            Villagers.Add(Villager.GetComponent<Villager>());
        }
    }

    public static List<Villager> GetVillagers()
    {
        return Villagers;
    }
    
    public static bool TryGetVillagerByRole(Roles role, out Villager value)
    {
        Villager villagerToReturn = null;
        foreach (var villager in GetVillagers())
        {
            if (villagerToReturn)
            {
                if (villager.CurrentRole == role)
                {
                    if (villagerToReturn.TasksToQueue.Count > villager.TasksToQueue.Count ||
                        villager.CurrentState is VillagerStates.Idle)
                    {
                        villagerToReturn = villager;
                    }
                }
            }
            else if (villager.CurrentRole == role && villager.CurrentState is VillagerStates.Idle)
            {
                villagerToReturn = villager;
            }
        }
        value = villagerToReturn;
        return villagerToReturn;
    }
    public static bool TryGetVillagerByRole(List<Roles> roles, out Villager value)
    {
        Villager villagerToReturn = null;
        foreach (var role in roles)
        {
            foreach (var villager in GetVillagers())
            {
                if (villagerToReturn)
                {
                    if (villager.CurrentRole == role)
                    {
                        if (villagerToReturn.TasksToQueue.Count > villager.TasksToQueue.Count ||
                            villager.CurrentState is VillagerStates.Idle)
                        {
                            villagerToReturn = villager;
                        }
                    }
                }
                else if (villager.CurrentRole == role && villager.CurrentState is VillagerStates.Idle)
                {
                    villagerToReturn = villager;
                }
            }
        }
        value = villagerToReturn;
        return villagerToReturn;
    }
    
    public static bool TryGetVillagerByRole(out Villager value)
    {
        Villager villagerToReturn = null;

        foreach (var villager in GetVillagers())
        {
            if (villagerToReturn)
            {
                if (villagerToReturn.TasksToQueue.Count > villager.TasksToQueue.Count ||
                    villager.CurrentState is VillagerStates.Idle)
                {
                    villagerToReturn = villager;
                }
            }
            else if (villager.CurrentState is VillagerStates.Idle) 
            {
                villagerToReturn = villager;
            }
        }
        value = villagerToReturn;
        return villagerToReturn;
    }

}

