using System;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private Button _stockpile;
    [SerializeField] private Button _inventory;
    [SerializeField] private Button _crafting;
    [SerializeField] private Button _villagerManagement;
    [SerializeField] private Button _building;
    
    private TutorialStage _tutorialStage;

    public TutorialStage TutorialStage
    {
        get => _tutorialStage;
        set
        {
            _tutorialStage = value;
            
            switch (_tutorialStage)
            {
                case TutorialStage.StockpileTutorial:
                    Debug.Log("Hey there! As our village leader you're in charge of maintaining the happiness and safety of our villagers, right now its just you and me, so I'll show you the ropes.");
                    Debug.Log("To begin, click on the mine cart in your tool bar, this is how you enable stockpile mode. In stockpile mode you can click and drag on the ground to create a stockpile. this is where villagers will place items they come across");
                    Debug.Log("Create a stockpile now.");
                    _stockpile.interactable = true;
                    _inventory.interactable = false;
                    _crafting.interactable = false;
                    _villagerManagement.interactable = false;
                    _building.interactable = false;
                    // Add experience to the village heart
                    break;
                case TutorialStage.InventoryTutorial:
                    Debug.Log("Good Job! Now you have created a stockpile you are able to access a list of all your village resources, you can do this by pressing the chest icon.");
                    Debug.Log("Look at your resources now");
                    _stockpile.interactable = false;
                    _inventory.interactable = true;
                    _crafting.interactable = false;
                    _villagerManagement.interactable = false;
                    _building.interactable = false;
                    // Add experience to the village heart
                    break;
                case TutorialStage.VillagerManagementTutorial:
                    Debug.Log("Nice! You may have noticed after doing these tasks our village heart has started glowing, this is because it is ready to power up. Powering up the village heart will allow you to summon more villagers to our colony.");
                    // Debug.Log("Click on the village heart and upgrade it now.");
                    _stockpile.interactable = false;
                    _inventory.interactable = false;
                    _crafting.interactable = false;
                    _villagerManagement.interactable = true;
                    _building.interactable = false;
                    break;
                case TutorialStage.CraftingTutorial:
                    _stockpile.interactable = false;
                    _inventory.interactable = false;
                    _crafting.interactable = true;
                    _villagerManagement.interactable = false;
                    _building.interactable = false;
                    break;
                case TutorialStage.BuildingTutorial:
                    _stockpile.interactable = false;
                    _inventory.interactable = false;
                    _crafting.interactable = false;
                    _villagerManagement.interactable = false;
                    _building.interactable = true;
                    break;
                case TutorialStage.CompletedTutorial:
                    _stockpile.interactable = true;
                    _inventory.interactable = true;
                    _crafting.interactable = true;
                    _villagerManagement.interactable = true;
                    _building.interactable = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

public enum TutorialStage
{
    StockpileTutorial,
    InventoryTutorial,
    VillagerManagementTutorial,
    CraftingTutorial,
    BuildingTutorial,
    CompletedTutorial,
}