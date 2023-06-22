using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VillagerManager : MonoBehaviour
{
    [SerializeField] private static List<Villager> Villagers;

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
}
