using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuUIScript : MonoBehaviour
{
    UIDocument uiDocument;
    VisualElement root;
    private string gameScene = "GameScene";

    [SerializeField] private Villager villager;
    public Material[] hairColours;
    private int currentHairColourIndex;
    private int currentGenderIndex = 1;

    
    #region Button References

    Button playButton;
    Button settingsButton;
    Button exitButton;

    #endregion

    private VisualElement buttonContainer;
    private VisualElement logoContainer;
    private VisualElement mainMenuContainer;
    private VisualElement characterCustomisationContainer;

    /// <summary>
    /// Character Creator
    /// </summary>
    private Button leftHair;
    private Button rightHair;
    private Button leftBody;
    private Button rightBody;
    private Button beginJourneyButton;


    [SerializeField] private CinemachineVirtualCamera characterCreationCamera;
    [SerializeField] private CinemachineVirtualCamera mainMenuCamera;

    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        playButton = root.Q<Button>("PlayButton");
        settingsButton = root.Q<Button>("SettingsButton");
        exitButton = root.Q<Button>("ExitButton");

        playButton.RegisterCallback<ClickEvent>(OnPlayButtonPress);
        settingsButton.RegisterCallback<ClickEvent>(OnSettingsButtonPress);
        exitButton.RegisterCallback<ClickEvent>(OnExitButtonPress);

        buttonContainer = root.Q<VisualElement>("ButtonContainer");
        logoContainer = root.Q<VisualElement>("Title");
        characterCustomisationContainer = root.Q<VisualElement>("CharacterCreation");
        mainMenuContainer = root.Q<VisualElement>("MainMenu");
        
        leftHair = root.Q<Button>("LeftHairButton");
        leftBody = root.Q<Button>("LeftBodyButton");
        rightHair = root.Q<Button>("RightHairButton");
        rightBody = root.Q<Button>("RightBodyButton");
        beginJourneyButton = root.Q<Button>("BeginJourneyButton");
        
        leftHair.RegisterCallback<ClickEvent>(PreviousHair);
        leftBody.RegisterCallback<ClickEvent>(PreviousBody);
        rightHair.RegisterCallback<ClickEvent>(NextHair);
        rightBody.RegisterCallback<ClickEvent>(NextBody);

        villager.VillagerCustomisation.HairColour = hairColours[0];
        villager.VillagerCustomisation.Gender = Model.Woman;
        beginJourneyButton.RegisterCallback<ClickEvent>(BeginJourney);
    }

    private void BeginJourney(ClickEvent evt)
    {
        Debug.Log("Clicked begin journey");
        VillagerManager.AddVillagerToList(villager);
        SceneManager.LoadScene(gameScene);
    }

    private void Start()
    {
        StartCoroutine(ShowMainMenuItems());
    }

    private void OnPlayButtonPress(ClickEvent evt)
    {
        print("start button pressed");
        GoToCharacterCreation();
    }
    private void OnSettingsButtonPress(ClickEvent evt)
    {
        print("settings button pressed");
    }
    private void OnExitButtonPress(ClickEvent evt)
    {
        print("exit button pressed");
        Application.Quit();
    }

    private void GoToCharacterCreation()
    {
        mainMenuCamera.enabled = false;
        characterCreationCamera.enabled = true;
        mainMenuCamera.GetComponentInParent<MainMenuCameraRotate>().enabled = false;
        logoContainer.AddToClassList("TitleImageHidden");
        buttonContainer.AddToClassList("ButtonsHidden");
        
        characterCustomisationContainer.RemoveFromClassList("CharacterCreationMenuHidden");
    }

    private IEnumerator ShowMainMenuItems()
    {
        yield return new WaitForSeconds(0.1f);
        logoContainer.RemoveFromClassList("TitleImageHidden");
        buttonContainer.RemoveFromClassList("ButtonsHidden");
    }

    #region Character Creator

    private void PreviousHair(ClickEvent evt)
    {
        currentHairColourIndex--;
        if (currentHairColourIndex < 0)
        {
            currentHairColourIndex = hairColours.Length-1;
        }

        villager.VillagerCustomisation.HairColour = hairColours[currentHairColourIndex];
    }
    
    private void NextHair(ClickEvent evt)
    {
        currentHairColourIndex++;
        if ( currentHairColourIndex > hairColours.Length-1)
        {
            currentHairColourIndex = 0;
        }

        villager.VillagerCustomisation.HairColour = hairColours[currentHairColourIndex];
    }
    
    private void PreviousBody(ClickEvent evt)
    {
        var gender = Enum.GetValues(typeof(Model));
        currentGenderIndex++;
        if (currentGenderIndex > gender.Length-1)
        {
            currentGenderIndex = 0;
        }
        villager.VillagerCustomisation.Gender = (Model)gender.GetValue(currentGenderIndex);
        // _inputField.text = villager.VillagerStats.VillagerName;
    }
    
    private void NextBody(ClickEvent evt)
    {
        var gender = Enum.GetValues(typeof(Model));
        currentGenderIndex++;
        if (currentGenderIndex > gender.Length-1)
        {
            currentGenderIndex = 0;
        }
        villager.VillagerCustomisation.Gender = (Model)gender.GetValue(currentGenderIndex);
    }
    
    #endregion
}
