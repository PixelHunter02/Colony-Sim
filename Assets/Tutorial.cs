using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private Button _stockpile;
    [SerializeField] private Button _inventory;
    [SerializeField] private Button _crafting;
    [SerializeField] private Button _villagerManagement;
    [SerializeField] private Button _building;
    
    private TutorialStage _tutorialStage;
    private GameManager _gameManager;

    [SerializeField] private TMP_Text tutorialPrompt;

    public GameObject wasdGO;
    public Image wKey;
    public Image aKey;
    public Image sKey;
    public Image dKey;
    public Sprite wSpriteLight;
    public Sprite aSpriteLight;
    public Sprite sSpriteLight;
    public Sprite dSpriteLight;

    private bool wPressed;
    private bool aPressed;
    private bool sPressed;
    private bool dPressed;

    public Image mouse;
    public Sprite middleMouseClick;
    private bool middleMousePressed;

    public Sprite rightMouseClick;
    private bool rightMousePressed;

    public Sprite mouseBlank;
    private void Start()
    {
        _gameManager = GameManager.Instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && !wPressed)
        {
            wKey.sprite = wSpriteLight;
            wPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.A) && !aPressed)
        {
            aKey.sprite = aSpriteLight;
            aPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.S) && !sPressed)
        {
            sKey.sprite = sSpriteLight;
            sPressed = true;
        }
        if (Input.GetKeyDown(KeyCode.D) && !dPressed)
        {
            dKey.sprite = dSpriteLight;
            dPressed = true;
        }

        if (wPressed && sPressed && aPressed && dPressed && TutorialStage == TutorialStage.KeyboardMovementTutorial)
        {
            wKey.transform.DOScale(Vector3.zero, 0.5f);
            sKey.transform.DOScale(Vector3.zero, 0.5f);
            aKey.transform.DOScale(Vector3.zero, 0.5f);
            dKey.transform.DOScale(Vector3.zero, 0.5f);

            TutorialStage = TutorialStage.MouseMovementTutorial;
        }

        if (Input.GetKeyDown(KeyCode.Mouse2) && !middleMousePressed)
        {
            mouse.sprite = middleMouseClick;
            middleMousePressed = true;
        }

        if (Input.GetKeyUp(KeyCode.Mouse2) && TutorialStage == TutorialStage.MouseMovementTutorial)
        {
            mouse.sprite = mouseBlank;
            TutorialStage = TutorialStage.MouseRotateTutorial;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && !rightMousePressed)
        {
            mouse.sprite = rightMouseClick;
            rightMousePressed = true;
        }
        
        if (Input.GetKeyUp(KeyCode.Mouse1) && TutorialStage == TutorialStage.MouseRotateTutorial)
        {
            TutorialStage = TutorialStage.StockpileTutorial;
        }
    }


    public TutorialStage TutorialStage
    {
        get => _tutorialStage;
        set
        {
            _tutorialStage = value;
            var villager = VillagerManager.GetVillagers()[0];

            switch (_tutorialStage)
            {
                case TutorialStage.KeyboardMovementTutorial:
                    // villager.VillagerStats.CurrentEmotion = Emotion.Instruction;
                    villager.agent.isStopped = true;
                    wasdGO.SetActive(true);
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"Hey there! As our village leader you're in charge of maintaining the happiness and safety of our villagers, right now its just you and me, so I'll show you the ropes.");
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"You can use the W, A, S and D keys to move the camera, give it a go.", Emotion.Instruction);
                    break;
                
                case TutorialStage.MouseMovementTutorial:
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"Nice Job! You can use the mouse to pan the camera by holding the middle mouse button and dragging, while you're at it, why dont you try scrolling the mouse wheel!", Emotion.Instruction);
                    mouse.transform.DOScale(Vector3.one, 0.5f);
                    _gameManager.level.villageHeart.GetComponent<VillageHeart>().Experience += 2;
                    break;
                
                case TutorialStage.MouseRotateTutorial:
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"Lets rotate your camera, you can do this by right clicking and dragging your mouse. Give that a go, then we will move on to some more fun stuff!",Emotion.Instruction);
                    _gameManager.level.villageHeart.GetComponent<VillageHeart>().Experience += 2;
                    break;

                // case TutorialStage.VillagerStatsTutorial:
                //     mouse.transform.DOScale(Vector3.zero, 0.5f);
                //     Level.AddToVillagerLog(villager,"");
                //     Level.AddToVillagerLog(villager,"Nice job! You can use the middle mouse button and drag to pan the camera, or the right mouse button and drag to rotate. Why dont you try that now?");
                //     // tutorialPrompt.text = "1/x Click on the Villager.";
                //     _stockpile.interactable = false;
                //     _inventory.interactable = false;
                //     _crafting.interactable = false;
                //     _villagerManagement.interactable = false;
                //     _building.interactable = false;
                //     break;
                case TutorialStage.StockpileTutorial:
                    mouse.transform.DOScale(Vector3.zero, 0.5f);
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"Lets click on the mine cart in your tool bar, this is how you enable stockpile mode. In stockpile mode you can click and drag on the ground to create a stockpile. this is where villagers will place items they come across");
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"Go ahead and create a stockpile now.",Emotion.Instruction);
                    villager.agent.isStopped = false;
                    _stockpile.interactable = true;
                    _inventory.interactable = false;
                    _crafting.interactable = false;
                    _villagerManagement.interactable = false;
                    _building.interactable = false;
                    _gameManager.level.villageHeart.GetComponent<VillageHeart>().Experience += 2;
                    break;
                case TutorialStage.InventoryTutorial:
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"Good Job! Now you have created a stockpile you are able to access a list of all your village resources, you can do this by pressing the chest icon.");
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"Have a look as I collect the resources laying on the ground.", Emotion.Instruction);
                    _stockpile.interactable = true;
                    _inventory.interactable = true;
                    _crafting.interactable = false;
                    _villagerManagement.interactable = false;
                    _building.interactable = false;

                    _gameManager.level.villageHeart.GetComponent<VillageHeart>().Experience += 2;
                    break;

                case TutorialStage.CraftingTutorial:
                    _gameManager.level.villageHeart.GetComponentInChildren<Renderer>().material.SetFloat("_DecalEmissionIntensity", 0f);
                    Level.AddToVillagerLog(villager, "");
                    Level.AddToVillagerLog(villager, "Awesome work! next up is the crafting menu, if you click on the anvil button in your toolbar, your crafting menu will be opened.");
                    Level.AddToVillagerLog(villager, "");
                    Level.AddToVillagerLog(villager, "In the crafting menu you can select an item to be crafted and click the button to add it to the queue, if you have the required resources. Go ahead and craft an axe.", Emotion.Instruction);
                    _stockpile.interactable = true;
                    _inventory.interactable = true;
                    _crafting.interactable = true;
                    _villagerManagement.interactable = false;
                    _building.interactable = false;
                    _gameManager.level.villageHeart.GetComponent<VillageHeart>().Experience += 2;
                    break;

                case TutorialStage.VillageHeartTutorial:
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"You're doing great so far! We now have an axe but no one to use it. After doing those tasks your village heart has leveled up! go ahead and click on it to open the upgrade menu, then click the level up button", Emotion.Instruction);                  
                    _stockpile.interactable = true;
                    _inventory.interactable = true;
                    _crafting.interactable = false;
                    _villagerManagement.interactable = false;
                    _building.interactable = false;

                    //Add Experience To The Village Heart
                    _gameManager.level.villageHeart.GetComponent<VillageHeart>().Experience += 10;
                    break;
                
                case TutorialStage.VillagerManagementTutorial:
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"Nice! we can access the villager management menu by clicking the scroll in your taskbar.");
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"Why dont you assign your new villager to the lumberjack role using the drop down in the villager management menu.", Emotion.Instruction);
                    // Debug.Log("Click on the village heart and upgrade it now.");
                    _stockpile.interactable = true;
                    _inventory.interactable = true;
                    _crafting.interactable = true;
                    _villagerManagement.interactable = true;
                    _building.interactable = false;
                    break;
                case TutorialStage.BuildingTutorial:
                    Level.AddToVillagerLog(villager,"");
                    Level.AddToVillagerLog(villager,"Finally, we can enter build mode by clicking the hammer on the far right of the task bar, when in this mode click the fence icon and place a blueprint on the ground. You can rotate it with the q and e keys. A fence requires 2 sticks to craft, and will automatically be completed when a crafter is available.",Emotion.Instruction );
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
    KeyboardMovementTutorial,
    MouseMovementTutorial,
    MouseRotateTutorial,
    VillagerStatsTutorial,
    StockpileTutorial,
    InventoryTutorial,
    CraftingTutorial,
    VillageHeartTutorial,
    VillagerManagementTutorial,
    BuildingTutorial,
    CompletedTutorial,
}