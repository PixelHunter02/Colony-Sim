using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;

public class UIToolkitManager : MonoBehaviour
{
    // Core
    GameManager gameManager;
    private UIDocument _uiDocument;
    VisualElement root;
    [SerializeField] private AudioSource _audioSource;
    private UIDocument settingsUI;

    // Windows
    private VisualElement villagerManagementWindow;
    private VisualElement inventoryWindow;
    private VisualElement craftingMenuWindow;
    private VisualElement buildingManagementWindow;
    private VisualElement tutorialWindow;
    private VisualElement topBar;
    private VisualElement villageHeartWindow;

    // Templates
    [SerializeField] private VisualTreeAsset villagerManagementTemplate;
    [SerializeField] private VisualTreeAsset craftingIconTemplate;
    [SerializeField] private VisualTreeAsset inventoryIconTemplate;
    [SerializeField] private VisualTreeAsset craftingQueueTemplate;
    
    // Storage
    private List<StoredItemSO> craftingMenuButtons;
    private Dictionary<StoredItemSO, int> itemsInInventory;
    private Dictionary<StoredItemSO, TemplateContainer> itemSlot;
    private Dictionary<Villager, VisualElement> villagerInformation;

    private Dictionary<StoredItemSO, Button> buildButtons;

    // Audio
    [SerializeField] private AudioClip buttonSound;
    
    // Sliders
    public Slider villageHeartExp;
    
    // Top Bar
    private EnumField topRoleSelectField;
    private Villager topBarVillager;
    
    
    public static Action DrawInventoryAction;
    private void OnEnable()
    {
        DrawInventoryAction += UpdateStorage;
        Level.AddToVillagerLogAction += ShowVillagerInformation;
    }

    private void OnDisable()
    {
        DrawInventoryAction -= UpdateStorage;
        Level.AddToVillagerLogAction -= ShowVillagerInformation;
    }

    private void Start()
    {
        _uiDocument = GetComponent<UIDocument>();
        root = _uiDocument.rootVisualElement; 
        
        // Crafting Button
        var craftingButton = root.Q<Button>("CraftingButton");
        craftingMenuWindow = root.Q<TemplateContainer>("CraftingWindow");
        craftingButton.RegisterCallback<ClickEvent>(OnCraftingMenuButtonClicked);
        craftingMenuWindow.Q<Button>("CloseButton").RegisterCallback<ClickEvent>(CloseCraftingMenu);
        
        // Villager Management
        var villagerManagementButton = root.Q<Button>("VillagerManagementButton");
        villagerManagementWindow = root.Q<TemplateContainer>("VillagerManagementWindow");
        villagerManagementButton.RegisterCallback<ClickEvent>(OnVillagerManagementClicked);
        villagerManagementWindow.Q<Button>("CloseButton").RegisterCallback<ClickEvent>(CloseVillagerManagement);

        // Inventory Button
        var inventoryButton = root.Q<Button>("InventoryButton");
        inventoryWindow = root.Q<VisualElement>("InventoryWindow");
        inventoryButton.RegisterCallback<ClickEvent>(OnInventoryButtonClicked);
        inventoryWindow.Q<Button>("CloseButton").RegisterCallback<ClickEvent>(CloseInventory);

        // Stockpile Button
        var stockpileButton = root.Q<Button>("StockpileButton");
        stockpileButton.RegisterCallback<ClickEvent>(OnStockpileClicked);

        // Building Button
        var buildingButton = root.Q<Button>("BuildingButton");
        buildingButton.RegisterCallback<ClickEvent>(OnBuildingButtonClicked);
        root.Q<Button>("CloseBuildMode").RegisterCallback<ClickEvent>(evt =>
        {
            root.Q("ButtonContainer").RemoveFromClassList("BuildModeEnabled");
        } );
        // buildingManagementWindow = root.Q<TemplateContainer>("Building")
        
        // Tutorial Button
        tutorialWindow = root.Q("Tutorial");
        
        // Top Bar
        topBar = root.Q("TopBar");
        topBar.Q<Button>("CloseVillagerDialogue").RegisterCallback<ClickEvent>(evt =>
        {
            topBar.RemoveFromClassList("TopBarDown");
        });

        topRoleSelectField = topBar.Q<EnumField>("RoleSelector");
        topRoleSelectField.RegisterCallback<ChangeEvent<Enum>>(evt =>RoleSelectTopBar(evt));
        
        // Village Heart
        var sliderBar = new VisualElement();
        villageHeartWindow = root.Q<VisualElement>("VillageHeart");
        var slider = root.Q("VillageHeartExpSlider");
        slider.Q("unity-dragger").Add(sliderBar);
        slider.SetEnabled(false);
        sliderBar.AddToClassList("VillageHeartBar");
        villageHeartWindow.Q<Button>("CloseButton").RegisterCallback<ClickEvent>(evt =>
        {
            CloseVillageHeart();
        });
        
        root.Q<Button>("LevelUpButton").RegisterCallback<ClickEvent>(evt =>
        {
            gameManager.level.villageHeart.GetComponent<VillageHeart>().LevelUp();
        });
        
        
        Init();

        foreach (var button in root.Query(className: "Button").ToList())
        {
            button.RegisterCallback<MouseEnterEvent>(evt => PlayAudio(evt, buttonSound));
        }
    }

    private void Update()
    {
        topBar.Q<TextElement>("Time").text = System.DateTime.UtcNow.ToLocalTime().ToString("HH:mm");

    }

    #region VillagerManagement
    private void OnVillagerManagementClicked(ClickEvent evt)
    {   
        
        float delay = 1;
        villagerManagementWindow.style.display = DisplayStyle.Flex;
        villagerManagementWindow.AddToClassList("WindowUp");
        topBar.RemoveFromClassList("TopBarDown");

        // Add Villager Information to Menu.
        foreach(var villager in VillagerManager.GetVillagers())
        {
            if (villagerInformation.ContainsKey(villager))
            {
                // villagerInformation[villager].RemoveFromClassList("PanelHidden");
                StartCoroutine(BringUpInformation(villagerInformation[villager].Q("Container"), delay));
                continue;
            }
            
            // Create the Template.                                                     
            var template = villagerManagementTemplate.Instantiate();

            // Set up the Enum Field
            template.Q<EnumField>("RoleSelector").Init(villager.CurrentRole);
            
            // Delay the spawning in of panels
            StartCoroutine(AddTemplateToGrid(template, delay, villager));
  
            // Assign the render texture.
            template.Q<VisualElement>("Portrait").style.backgroundImage = new StyleBackground(Background.FromRenderTexture(villager._portraitRenderTexture));
            delay += 0.2f;
        }
    }

    private void CloseVillagerManagement(ClickEvent clickEvent)
    {
        foreach (var template in villagerInformation)
        {
            template.Value.Q("Container").AddToClassList("PanelHidden");
        }
        villagerManagementWindow.RemoveFromClassList("WindowUp");
    }

    private IEnumerator AddTemplateToGrid(VisualElement template, float delay, Villager villager)
    {
        var templateContainer = villagerManagementWindow.Q<ScrollView>("TemplateContainer");
        templateContainer.Add(template);
        villagerInformation.Add(villager, template);
        template.style.marginLeft = 20;
        template.style.marginRight = 20;
        template.style.marginTop = 20;
        template.style.marginBottom = 20;
        
        var roleSelectField = template.Q<EnumField>();
        // Add an event to the role change enum to be fired on change.
        roleSelectField.RegisterCallback<ChangeEvent<string>>((evt) =>
        {
            Enum.TryParse(evt.newValue.ToString(), out Roles role);
            // villager.CurrentRole = role;
            StartCoroutine(RoleChanged((int)role, villager));
        });
        
        template.Q("Container").AddToClassList("PanelVisible");
        yield return new WaitForSeconds(delay);
        template.Q("Container").RemoveFromClassList("PanelHidden");
    }
    
    private IEnumerator BringUpInformation(VisualElement template, float delay)
    {
        yield return new WaitForSeconds(delay);
        template.RemoveFromClassList("PanelHidden");
    }
    
    private IEnumerator RoleChanged(int value, Villager villager)
    {
        foreach (var item in StorageManager.itemList)
        {
            if ((int)item.itemSO.assignRole == value)
            {
                StorageManager.EmptyStockpileSpace(item);
                villager.CurrentRole = (Roles)value;
                yield break;
            }
        }
    }
    #endregion

    #region Crafting

    private void OnCraftingMenuButtonClicked(ClickEvent clickEvent)
    {
        OpenCraftingMenu();
        topBar.RemoveFromClassList("TopBarDown");
    }

    private void OpenCraftingMenu()
    {
        craftingMenuWindow.AddToClassList("WindowUp");
        UpdateRecipes();
    }

    private void UpdateRecipes()
    {
        // Update the recipes to display
        for (int i = 0; i < gameManager.craftingManager.CraftingRecipes.Length; i++)
        {
            // Make sure the elements arent in the container already
            if (craftingMenuButtons.Contains(gameManager.craftingManager.CraftingRecipes[i]))
            {
                continue;
            }
            if(gameManager.level.villageHeart.GetComponent<VillageHeart>().Level < gameManager.craftingManager.CraftingRecipes[i].levelUnlocked)
            {
                continue;
            }
        
            // Assign the container
            var craftingContainer = craftingMenuWindow.Q<ScrollView>("TemplateContainer");

            // create the template for the new item
            var template = craftingIconTemplate.Instantiate();
            
            // Assign image and name
            template.Q<VisualElement>("CraftingIcon").style.backgroundImage = gameManager.craftingManager.CraftingRecipes[i].uiSprite.texture;
            template.Q<Label>("LabelText").text = gameManager.craftingManager.CraftingRecipes[i].objectName;

            var item = gameManager.craftingManager.CraftingRecipes[i];
            
            // Add Effect to show the name of the item on mouse over
            template.RegisterCallback<MouseEnterEvent>(evt => ShowLabel(evt, template));
            template.RegisterCallback<MouseLeaveEvent>(evt => HideLabel(evt, template));
            template.Q<Button>("CraftButton").RegisterCallback<ClickEvent>(evt => AddToCraftingQueue(evt, item));

            // Add the template to the document.
            craftingContainer.Add(template);
            
            // Add Item To List
            craftingMenuButtons.Add(gameManager.craftingManager.CraftingRecipes[i]);
        }
    }
    
    private void AddToCraftingQueue(ClickEvent clickEvent, StoredItemSO craftingRecipe)
    {
        
        var template = craftingQueueTemplate.Instantiate();
        
        template.Q<VisualElement>("CraftingIcon").style.backgroundImage = craftingRecipe.uiSprite.texture;
        craftingMenuWindow.Q<ScrollView>("QueueList").Add(template);
        
        VillagerManager.TryGetVillagerByRole(out Villager villager);
        var craftingItem = gameManager.craftingManager.BeginCrafting(villager, craftingRecipe);
        // gameManager.craftingManager.craftingQueueDictionary.Add(craftingItem,queuedItem);
        // _gameManager.craftingManager.craftingQueue.Enqueue(craftingItem);
        villager.villagerQueue.Enqueue(craftingItem);
    }

    private void ShowLabel(MouseEnterEvent mouseEnterEvent, TemplateContainer template)
    {
        template.Q<VisualElement>("LabelBox").AddToClassList("LabelUp");
    }
    
    private void HideLabel(MouseLeaveEvent mouseLeaveEvent, TemplateContainer template)
    {
        template.Q<VisualElement>("LabelBox").RemoveFromClassList("LabelUp");
    }
    
    private void CloseCraftingMenu(ClickEvent clickEvent)
    {
        craftingMenuWindow.RemoveFromClassList("WindowUp");
    }
    
    #endregion

    #region Inventory

    private void OnInventoryButtonClicked(ClickEvent clickEvent)
    {
        OpenInventory();
        topBar.RemoveFromClassList("TopBarDown");
        
        if (gameManager.level.tutorialManager.TutorialStage == TutorialStage.InventoryTutorial)
        {
            gameManager.level.tutorialManager.TutorialStage = TutorialStage.CraftingTutorial;
        }
    }

    private void OpenInventory()
    {
        inventoryWindow.AddToClassList("WindowUp");
        UpdateStorage();
    }

    private void UpdateStorage()
    {
        itemsInInventory.Clear();
        foreach (var item in StorageManager.itemList)
        {
            Debug.Log(item.itemSO);
            if (!itemsInInventory.ContainsKey(item.itemSO) && !itemSlot.ContainsKey(item.itemSO))
            {
                Debug.Log("CreatingSlot");
                // Create a new slot
                var template = inventoryIconTemplate.Instantiate();
                Debug.Log(itemSlot.TryAdd(item.itemSO,template));
                itemSlot.TryAdd(item.itemSO,template);
                
                var image = template.Q<VisualElement>("ItemIcon");
                var count = template.Q<Label>("ItemCount");
                var name = template.Q<Label>("LabelText");

                itemsInInventory.Add(item.itemSO, 1);
                
                // Assign Image and count
                image.style.backgroundImage = item.itemSO.uiSprite.texture;
                count.text = "1";
                name.text = item.itemSO.objectName;
                
                inventoryWindow.Q<ScrollView>("TemplateContainer").Add(template);
                template.RegisterCallback<MouseEnterEvent>(evt => InventoryShowLabel(evt, template));
                template.RegisterCallback<MouseLeaveEvent>(evt => InventoryHideLabel(evt, template));
                template.style.paddingBottom = 20;
                template.style.paddingTop = 20;
                template.style.paddingLeft = 20;
                template.style.paddingRight = 20;
            }
            else if(!itemsInInventory.ContainsKey(item.itemSO))
            {
                itemsInInventory.Add(item.itemSO, 1);
                var template = itemSlot[item.itemSO];
                var image = template.Q<VisualElement>("ItemIcon");
                var count = template.Q<Label>("ItemCount");

                image.style.backgroundImage = item.itemSO.uiSprite.texture; 
                count.text = itemsInInventory[item.itemSO].ToString();
            }
            else 
            {
                var slot = itemSlot[item.itemSO];
                var image = slot.Q<VisualElement>("ItemIcon");
                var count = slot.Q<Label>("ItemCount");
                
                itemsInInventory[item.itemSO]++;
                count.text = itemsInInventory[item.itemSO].ToString();
            }
        }
        
        List<StoredItemSO> itemsToBeRemoved = new List<StoredItemSO>();
        // foreach (var item in itemSlot)
        // {
        //     if (!itemsInInventory.ContainsKey(item.Key))
        //     {
        //         inventoryWindow.Remove(item.Value);
        //         itemsToBeRemoved.Add(item.Key);
        //     }
        // }

        foreach (var itemSo in itemsToBeRemoved)
        {
            itemSlot.Remove(itemSo);
        }   
    }

    private void InventoryShowLabel(MouseEnterEvent mouseEnterEvent, TemplateContainer template)
    {
        template.Q<VisualElement>("LabelBox").AddToClassList("LabelUp");
        template.Q<VisualElement>("ItemCount").AddToClassList("CountUp");
    }
    private void InventoryHideLabel(MouseLeaveEvent mouseLeaveEvent, TemplateContainer template)
    {
        template.Q<VisualElement>("LabelBox").RemoveFromClassList("LabelUp");
        template.Q<VisualElement>("ItemCount").RemoveFromClassList("CountUp");
    }
    private void CloseInventory(ClickEvent clickEvent)
    {
        inventoryWindow.RemoveFromClassList("WindowUp");
    }

    #endregion

    #region Stockpile

    private void OnStockpileClicked(ClickEvent clickEvent)
    {
        gameManager.level.StockpileModeEnabled();
    }

    #endregion

    #region Building

    private void OnBuildingButtonClicked(ClickEvent evt)
    {
        root.Q("ButtonContainer").AddToClassList("BuildModeEnabled");
        foreach (var building in gameManager.buildingManager.buildings)
        {
            if (!buildButtons.ContainsKey(building))
            {
                var button = new Button(() => Debug.Log($"Building {building.objectName}"));
                button.RegisterCallback<MouseEnterEvent>(evt => PlayAudio(evt, buttonSound));
                var scrollview = root.Q<ScrollView>("BuildScrollView");
                scrollview.contentContainer.Add(button);
                float contentContainerSize = 10;
                foreach (var VARIABLE in scrollview.contentContainer.hierarchy.Children())
                {
                    if (VARIABLE.ClassListContains("Button"))
                    {
                        contentContainerSize += 10;
                    }
            
                }
                scrollview.contentContainer.style.width = Length.Percent(contentContainerSize);
                buildButtons.Add(building,button);
                // scrollview.style.width = Length.Percent(99.99f);
                // scrollview.style.width = Length.Percent(100f);
                // scrollview.style.height = Length.Percent(99.99f);
                // scrollview.style.height = Length.Percent(100f);

                button.AddToClassList("Button");
                button.style.backgroundImage = new StyleBackground(building.uiSprite);
                button.style.alignSelf = Align.FlexEnd;
                // root.Q<ScrollView>("BuildScrollView").;
                // scrollview.contentContainer.style.width = Length.Percent(100);
                Debug.Log(root.Q<ScrollView>("BuildScrollView").contentContainer.style.width.value.value);
            }

        } 
            
        StartCoroutine(delayedResize());

    }

    private IEnumerator delayedResize()
    {
        yield return new WaitForSeconds(0.1f);
        root.Q<ScrollView>("BuildScrollView").contentContainer.style.width =  Length.Percent(100);
    }
    
    #endregion

    #region VillagerInformationTop

    public void ShowVillagerInformation(Villager villager)
    {
        topBar.AddToClassList("TopBarDown");
        topBar.Q<VisualElement>("Portrait").style.backgroundImage = new StyleBackground(Background.FromRenderTexture(villager._portraitRenderTexture));
        topBar.Q<TextElement>("NameTag").text = villager.VillagerStats.VillagerName;
        topBar.Q<EnumField>("RoleSelector").Init(villager.CurrentRole);

        topBarVillager = villager;

        topBar.Q<TextElement>("VillagerLog").text = Level.GetVillagerLog(villager);
        // scrollView.scrollOffset = scrollView.contentContainer.layout.max - scrollView.contentViewport.layout.size;

        // topBar.Q<ScrollView>().scrollOffset = topBar.Q<ScrollView>().contentContainer.layout.max  - topBar.Q<ScrollView>().contentViewport.layout.size;
        topBar.Q<ScrollView>().contentContainer.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            topBar.Q<ScrollView>().verticalScroller.value = topBar.Q<ScrollView>().verticalScroller.highValue;
        });
    }

    public void RoleSelectTopBar(ChangeEvent<Enum>evt)
    {
        Enum.TryParse(evt.newValue.ToString(), out Roles role);
        topBarVillager.CurrentRole = role;
    }

    #endregion

    #region VillageHeart

    public void OpenVillageHeart()
    {
        villageHeartWindow.AddToClassList("WindowUp");
    }

    public void CloseVillageHeart()
    {
        villageHeartWindow.RemoveFromClassList("WindowUp");
    }

    #endregion

    private void Init()
    {
        gameManager = GameManager.Instance;    
        villagerManagementWindow.RemoveFromClassList("WindowUp");
        inventoryWindow.RemoveFromClassList("WindowUp");
        craftingMenuWindow.RemoveFromClassList("WindowUp");
        tutorialWindow.RemoveFromClassList("WindowUp");
        villageHeartWindow.RemoveFromClassList("WindowUp");
        craftingMenuButtons = new List<StoredItemSO>();
        itemsInInventory = new Dictionary<StoredItemSO, int>();
        itemSlot = new Dictionary<StoredItemSO, TemplateContainer>();
        villagerInformation = new Dictionary<Villager, VisualElement>();
        buildButtons = new Dictionary<StoredItemSO, Button>();
        villageHeartExp = root.Q<Slider>("VillageHeartExpSlider");
    }

    private void PlayAudio(MouseEnterEvent mouseEnterEvent, AudioClip audio)
    {
        _audioSource.clip = audio;
        _audioSource.Play();
   }
    
    public bool IsPointerOverUI ( Vector2 screenPos )
    {
        Vector2 pointerUiPos = new Vector2{ x = screenPos.x , y = Screen.height - screenPos.y };
        List<VisualElement> picked = new List<VisualElement>();
        _uiDocument.rootVisualElement.panel.PickAll( pointerUiPos , picked );
        foreach( var ve in picked )
            if( ve!=null )
            {
                Color32 bcol = ve.resolvedStyle.backgroundColor;
                if( bcol.a!=0 && ve.enabledInHierarchy )
                    return true;
            }
        return false;
    }
}
