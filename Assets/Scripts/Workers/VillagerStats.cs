using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Villager))]
public class VillagerStats : MonoBehaviour
{
    private Villager _villager;

    private void Start()
    {
        _villager = GetComponent<Villager>();
        modifiedMaxHealth = Mathf.CeilToInt(20 + (0.3f * Strength));
        Health = modifiedMaxHealth;
    }

    private int health;
    private int maxHealth;
    int modifiedMaxHealth;
    public int Health
    {
        get => health;
        set
        {
            health = value;
            Debug.Log($"{_villagerName}'s health has changed to {value}");
            switch (value)
            {
                case <= 0:
                    _villager.OnDeath();
                    Debug.Log("Died");
                    break;
            }
        }
    }
    
    private string _villagerName;
    public string VillagerName
    {
        get => _villagerName;
        set
        {
            _villagerName = value;
            Debug.Log($"A New Name Has Been Assigned! The new value is {_villagerName}");
        }
    }
    
    private int _strength;
    public int Strength
    {
        get => _strength;
        set
        {
            _strength = value;
            
            Debug.Log($"A New Strength Value Has Been Assigned! The new value is {_strength}");
        }
    }
    
    private int _craft;
    public int Craft
    {
        get => _craft;
        set
        {
            _craft = value;
            Debug.Log($"A New Craft Value Has Been Assigned! The new value is {_craft}");
        }
    }
    
    private int _magic;
    public int Magic
    {
        get => _magic;
        set
        {
            _magic = value;
            Debug.Log($"A New Magic Value Has Been Assigned! The new value is {_magic}");
        }
    }
    
    private int hunger;
    public int Hunger
    {
        get => hunger;
        set
        {
            Debug.Log("Hunger Has Been Modified");
            hunger = value;
        }
    }
}
