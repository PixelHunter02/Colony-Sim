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

    [SerializeField] private GameObject villageHeart;

    public TutorialStage TutorialStage
    {
        get => _tutorialStage;
        set
        {
            _tutorialStage = value;
            var villager = VillagerManager.GetVillagers()[0];

            switch (_tutorialStage)
            {

                case TutorialStage.VillagerStatsTutorial:
                    // villager.agent.isStopped = true;
                    // villager.transform.LookAt(Camera.main.transform);
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"Hey there! As our village leader you're in charge of maintaining the happiness and safety of our villagers, right now its just you and me, so I'll show you the ropes.");
                    _stockpile.interactable = false;
                    _inventory.interactable = false;
                    _crafting.interactable = false;
                    _villagerManagement.interactable = false;
                    _building.interactable = false;
                    break;
                case TutorialStage.StockpileTutorial:
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"To begin, click on the mine cart in your tool bar, this is how you enable stockpile mode. In stockpile mode you can click and drag on the ground to create a stockpile. this is where villagers will place items they come across");
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"Create a stockpile now.");
                    villager.agent.isStopped = false;
                    // villageHeart.GetComponent<VillageHeart>().Experience += 10;
                    _stockpile.interactable = true;
                    _inventory.interactable = false;
                    _crafting.interactable = false;
                    _villagerManagement.interactable = false;
                    _building.interactable = false;
                    // Add experience to the village heart
                    break;
                case TutorialStage.InventoryTutorial:
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"Good Job! Now you have created a stockpile you are able to access a list of all your village resources, you can do this by pressing the chest icon.");
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"Look at your resources now");
                    // villageHeart.GetComponent<VillageHeart>().Experience += 10;
                    _stockpile.interactable = true;
                    _inventory.interactable = true;
                    _crafting.interactable = false;
                    _villagerManagement.interactable = false;
                    _building.interactable = false;
                    // Add experience to the village heart
                    break;
                case TutorialStage.VillageHeartTutorial:
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"Nice! You may have noticed after doing these tasks our village heart has started glowing, this is because it is ready to power up. Powering up the village heart will allow you to summon more villagers to our colony.");
                    // Debug.Log("Click on the village heart and upgrade it now.");
                    villageHeart.GetComponentInChildren<Renderer>().material.SetFloat("_DecalEmissionIntensity", 10f);
                    _stockpile.interactable = true;
                    _inventory.interactable = true;
                    _crafting.interactable = false;
                    _villagerManagement.interactable = false;
                    _building.interactable = false;
                    break;
                case TutorialStage.CraftingTutorial:
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"Now we have another villager we should give them a role, we dont want them getting bored now do we!");
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"Sadly, we dont have anything we can give them to do, why dont you go ahead and craft an axe in the crafting menu, you can access this by clicking the anvil in your toolbar. This will allow us to assign them the role of lumberjack. We will require 2 sticks and 1 stone to craft an axe, luckily there was some laying on the ground which I went ahead and picked up");
                    _stockpile.interactable = true;
                    _inventory.interactable = true;
                    _crafting.interactable = true;
                    _villagerManagement.interactable = false;
                    _building.interactable = false;
                    break;
                case TutorialStage.VillagerManagementTutorial:
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"Nice! we can access the villager management menu by clicking the scroll in your taskbar.");
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"Why dont you assign your new villager to the lumberjack role using the drop down in the villager management menu.");
                    // Debug.Log("Click on the village heart and upgrade it now.");
                    _stockpile.interactable = true;
                    _inventory.interactable = true;
                    _crafting.interactable = true;
                    _villagerManagement.interactable = true;
                    _building.interactable = false;
                    break;
                case TutorialStage.BuildingTutorial:
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"Finally, we can enter build mode by clicking the hammer on the far right of the task bar, when in this mode click the fence icon and place a blueprint on the ground. You can rotate it with the q and e keys. A fence requires 2 sticks to craft, and will automatically be completed when a crafter is available.");
                    _stockpile.interactable = true;
                    _inventory.interactable = true;
                    _crafting.interactable = true;
                    _villagerManagement.interactable = true;
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
    VillagerStatsTutorial,
    StockpileTutorial,
    InventoryTutorial,
    VillageHeartTutorial,
    CraftingTutorial,
    VillagerManagementTutorial,
    BuildingTutorial,
    CompletedTutorial,
}