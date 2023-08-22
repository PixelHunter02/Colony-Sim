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

    public static List<string> maleNames;
    public static List<string> femaleNames;

    private void Awake()
    {
        if(villagers == null)
            villagers = new List<Villager>();

        
        maleNames ??= new List<string>()
        {
            "James",
            "Jack",
            "Joseph",
            "Liam",
            "Lenny",
            "Louis",
        };
        
        femaleNames ??= new List<string>()
        {
            "Caitlin",
            "Jess",
            "Joanna",
            "Alice",
            "Lucy",
            "Amanda",
        };
        
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
                if (villager.CurrentRole == role && !villager.ignoreQueue)
                {
                    if (villagerToReturn.TasksToQueue.Count > villager.TasksToQueue.Count)
                    {
                        villagerToReturn = villager;
                    }
                }
            }
            else if (villager.CurrentRole == role && !villager.ignoreQueue)
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
                    if (villager.CurrentRole == role && !villager.ignoreQueue)
                    {
                        if (villagerToReturn.TasksToQueue.Count > villager.TasksToQueue.Count||
                            villager.CurrentState is VillagerStates.Idle)
                        {
                            villagerToReturn = villager;
                        }
                    }
                }
                else if (villager.CurrentRole == role && villager.CurrentState is VillagerStates.Idle && !villager.ignoreQueue)
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
                    villager.CurrentState is VillagerStates.Idle && !villager.ignoreQueue)
                {
                    villagerToReturn = villager;
                }
            }
            else if (villager.CurrentState is VillagerStates.Idle && !villager.ignoreQueue) 
            {
                villagerToReturn = villager;
            }
        }
        value = villagerToReturn;
        return villagerToReturn;
    }

    public void GenerateNewVillagerStats(Villager villager)
    {
            villager.VillagerStats.Craft = Random.Range(1, 7);
            villager.VillagerStats.Magic = Random.Range(1, 7);
            villager.VillagerStats.Strength = Random.Range(1, 7);
            villager.HairColour = hairColours[Random.Range(0, hairColours.Length)];

            maleNames = null;
            femaleNames = null;
            
            maleNames ??= new List<string>()
            {
                "James",
                "Jack",
                "Joseph",
                "Liam",
                "Lenny",
                "Louis",
            };
        
            femaleNames ??= new List<string>()
            {
                "Caitlin",
                "Jess",
                "Joanna",
                "Alice",
                "Lucy",
                "Amanda",
            };
            
            var gender = Enum.GetValues(typeof(Gender));
            var position = Random.Range(0,gender.Length-1);
            
            Model newGender = (Model)gender.GetValue(position);
            villager.Gender = newGender;
    }

    public void SpawnVillager(Villager villagerToSpawn, out Villager villagerToReturn)
    {
        var villager = Instantiate(villagerPrefab, Vector3.zero, quaternion.identity);
        var villagerComponent = villager.GetComponent<Villager>();
        villagerComponent.VillagerStats.Craft = villagerToSpawn.VillagerStats.Craft;
        villagerComponent.VillagerStats.Magic = villagerToSpawn.VillagerStats.Magic;
        villagerComponent.VillagerStats.Strength = villagerToSpawn.VillagerStats.Strength;
        villagerComponent.HairColour = villagerToSpawn.HairColour;
        villagerComponent.Gender = villagerToSpawn.Gender;
        villagerComponent.VillagerStats.VillagerName = villagerToSpawn.VillagerStats.VillagerName;
        villagerComponent.CurrentRole = villagerToSpawn.CurrentRole;
        villagerToReturn = villager.GetComponent<Villager>();
    }
}

