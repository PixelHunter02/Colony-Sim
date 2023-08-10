using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VillageHeart : MonoBehaviour,IInteractable
{
    private GameManager _gameManager;
    private int level = 1;
    private float experience = 0;

    [SerializeField] private Slider villagerHeartEXPSlider;

    public float Experience
    {
        get => experience;
        set
        {
            experience = value;
            villagerHeartEXPSlider.value = experience / experienceToNextLevel;
        }
    }
    [SerializeField]private float experienceToNextLevel = 20;

    [SerializeField] private GameObject villagerPrefab;
    private void Awake()
    {
        _gameManager = GameManager.Instance;
        enabled = false;
    }

    public void OnInteraction()
    {
        Debug.Log("You Interacted with the village heart!");
        if (_gameManager.level.tutorialManager.TutorialStage is TutorialStage.VillageHeartTutorial)
        {
            _gameManager.level.tutorialManager.TutorialStage = TutorialStage.CraftingTutorial;
        }
        
        LevelUp();
        
    }

    public bool CanInteract()
    {
        if (Experience >= experienceToNextLevel)
        {
            return true;
        }
        return false;
    }

    public void LevelUp()
    {
        level++;
        experienceToNextLevel += experienceToNextLevel / 10;
        GetComponentInChildren<Renderer>().material.SetFloat("_DecalEmissionIntensity", 0f);
        Experience = 0;
        var villagerGO = Instantiate(villagerPrefab);
        VillagerManager.villagers.Add(villagerGO.GetComponent<Villager>());
    }
}
