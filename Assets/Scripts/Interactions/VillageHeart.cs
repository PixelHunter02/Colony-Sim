using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageHeart : MonoBehaviour,IInteractable
{
    private GameManager _gameManager;
    private int level = 1;
    private float experience = 0f;

    public float Experience
    {
        get => experience;
        set
        {
            experience = value;
            Debug.Log("Experience has been updated");
        }
    }
    private float experienceToNextLevel = 20f;

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

        var villagerGO = Instantiate(villagerPrefab);
        VillagerManager.villagers.Add(villagerGO.GetComponent<Villager>());
    }

    public bool CanInteract()
    {
        if (_gameManager.level.tutorialManager.TutorialStage is TutorialStage.VillageHeartTutorial)
        {
            return true;
        }
        return false;
    }
}
