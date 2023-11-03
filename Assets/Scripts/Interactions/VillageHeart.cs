using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VillageHeart : MonoBehaviour,IInteractable
{
    private GameManager _gameManager;
    private int level = 1;

    [SerializeField] private ParticleSystem particleBurst;
    [SerializeField] private ParticleSystem readyParticle;
    [SerializeField] private AudioSource levelUpAudio;

    public int Level
    {
        get => level;
        private set
        {
            level = value;

        }
    }
    private float experience = 0;

    [SerializeField] private Slider villagerHeartEXPSlider;

    public GameObject villagerHeartMenu;
    public Slider villageHeartMenuSlider;
    public TMP_Text villageHeartExpText;
    public TMP_Text villageHeartLevelText;

    public float Experience
    {
        get => experience;
        set
        {
            experience = value;
            if (ReadyToLevelUp())
            {
                print("ready to lvl up");
                readyParticle.Play();
            }
            else
            {
                readyParticle.Stop();
            }
            
            villagerHeartEXPSlider.value = experience / experienceToNextLevel;
            villageHeartMenuSlider.value = experience / experienceToNextLevel;

            GameObject.Find("UIToolkit").GetComponent<UIToolkitManager>().villageHeartExp.value =
                experience / experienceToNextLevel;
            villageHeartExpText.text = $"{Experience}/{experienceToNextLevel}";
        }
    }
    
    [SerializeField]private float experienceToNextLevel = 20;

    [SerializeField] private GameObject villagerPrefab;
    private void Awake()
    {
        _gameManager = GameManager.Instance;
        enabled = false;
        villagerHeartEXPSlider.value = experience / experienceToNextLevel;
        villageHeartMenuSlider.value = experience / experienceToNextLevel;
        GameObject.Find("UIToolkit").GetComponent<UIToolkitManager>().villageHeartExp.value =
            experience / experienceToNextLevel;
        villageHeartExpText.text = $"{Experience}/{experienceToNextLevel}";
        
    }

    public void OnInteraction()
    {
        // if(_gameManager.IsOverUI())
        // {
        //     return;
        // }

        Debug.Log("You Interacted with the village heart!");
        if (_gameManager.level.tutorialManager.TutorialStage is TutorialStage.VillageHeartTutorial)
        {
            _gameManager.level.tutorialManager.TutorialStage = TutorialStage.VillagerManagementTutorial;
        }

        villagerHeartMenu.SetActive(true);
        // LevelUp();

    }

    public bool CanInteract()
    {
        return true;
    }

    public bool ReadyToLevelUp()
    {
        if (Experience >= experienceToNextLevel)
        {
            return true;
        }
        return false;
    }
    
    public void LevelUp()
    {
        if(!ReadyToLevelUp())
            return;
        Level++;
        villageHeartLevelText.text = $"Village Heart Level: {Level}";
        Experience -= experienceToNextLevel;
        experienceToNextLevel += experienceToNextLevel / 10;
        GetComponentInChildren<Renderer>().material.SetFloat("_DecalEmissionIntensity", 0f);
        var villagerGO = Instantiate(villagerPrefab);
        VillagerManager.villagers.Add(villagerGO.GetComponent<Villager>());
        _gameManager.level.CloseAllUI();
        particleBurst.Emit(100);
        levelUpAudio.Play();
    }
}
