using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class VillagerManager : MonoBehaviour
{
    public static List<Villager> villagers;

    [SerializeField] private Material[] hairColours;

    [SerializeField] private GameObject villagerPrefab;

    private void Awake()
    {
        if(villagers == null)
            villagers = new List<Villager>();
    }

    public static List<Villager> GetVillagers()
    {
        return villagers;
    }

    public static void AddVillagerToList(Villager villager)
    {
        villagers.Add(villager);
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

    public void GenerateNewVillagerStats(Villager villager)
    {
            villager.Craft = Random.Range(1, 7);
            villager.Magic = Random.Range(1, 7);
            villager.Strength = Random.Range(1, 7);
            
            villager.hairColour = hairColours[Random.Range(0, hairColours.Length)];

            var gender = Enum.GetValues(typeof(Gender));
            var position = Random.Range(0,gender.Length-1);
            Model newGender = (Model)gender.GetValue(position);
            villager.Gender = newGender;

            // villager.GetComponent<Villager>().Craft = Random.Range(1, 7);
    }

    public void SpawnVillager(Villager villagerToSpawn, out Villager villagerToReturn)
    {
        var villager = Instantiate(villagerPrefab, Vector3.zero, quaternion.identity);
        villager.GetComponent<Villager>().VillagerName = villagerToSpawn.VillagerName;
        villager.GetComponent<Villager>().Craft = villagerToSpawn.Craft;
        villager.GetComponent<Villager>().Magic = villagerToSpawn.Magic;
        villager.GetComponent<Villager>().Strength = villagerToSpawn.Strength;
        villager.GetComponent<Villager>().hairColour = villagerToSpawn.hairColour;
        villager.GetComponent<Villager>().Gender = villagerToSpawn.Gender;
        villagerToReturn = villager.GetComponent<Villager>();
    }
}

