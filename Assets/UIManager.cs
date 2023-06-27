using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameManager _gameManager;
    
    [SerializeField] private GameObject _infoUI;
    private TMP_Text _villagerName;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject villagerManagementUI;
    [SerializeField] private GameObject villagerSelectUI;

    [SerializeField] private GameObject villagerManagementCell;
    [SerializeField] private Transform villagerManagementContainer;

    [SerializeField] private Button[] roleButtons;
    [SerializeField] private GameObject roleAssignmentUI;

    private static Dictionary<Villager, string> villagerLog;
    [SerializeField] private TMP_Text villagerLogTMP;

    private TMP_Text roleSelectionTMPText;

    public bool IsOverUI() => UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

    public bool stockpileMode;
    
    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _villagerName = GameObject.Find("SelectedVillagerName").GetComponent<TMP_Text>();
        roleSelectionTMPText = GameObject.Find("RoleText").GetComponent<TMP_Text>();
        villagerLog = new Dictionary<Villager, string>();
        _infoUI.SetActive(false);
        CloseAllUI();
    }

    private void Start()
    {
        UpdateVillagerManagementUI();
    }

    private void Update()
    {
        if (_gameManager.inputManager.EscapePressed())
        {
            CloseAllUI();
        }
    }

    // On Villager Click
    public void ShowVillagerInformation(Villager villager)
    {
        var storedLog = villagerLog.GetValueOrDefault(villager, String.Empty);
        villagerLogTMP.text = storedLog;
        _infoUI.SetActive(true);
        _villagerName.text = villager.VillagerName;
    }

    // Button Set In Inspector
    public void ShowVillagerSelectUI()
    {
        villagerSelectUI.SetActive(!villagerSelectUI.activeSelf);
    }
    
    public void UpdateVillagerManagementUI()
    {
        for (int i = 0; i < VillagerManager.GetVillagers().Count; i++)
        {
            var villager = VillagerManager.GetVillagers()[i];
            var cell = Instantiate(villagerManagementCell, villagerManagementContainer);
            cell.transform.Find("Label").Find("Name").GetComponent<TMP_Text>().text = villager.VillagerName;
            var button = cell.transform.Find("Button").GetComponent<Button>();
            button.GetComponent<ButtonReference>().workerReference = villager;
            button.onClick.AddListener(() => OpenRoleManagementUI(button.GetComponent<ButtonReference>().workerReference));
            Debug.Log(button.GetComponent<ButtonReference>().workerReference);
            cell.SetActive(true);
        }
    }

    void OpenRoleManagementUI(Villager villager)
    {
        foreach (var button in roleButtons)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SetVillagerRole(villager));
        }
        CloseAllUI();
        roleSelectionTMPText.text = "Select Role For: \n" + villager.VillagerName;
        villagerManagementUI.SetActive(true);
        roleAssignmentUI.SetActive(true);
    }

    void SetVillagerRole(Villager villager)
    {
        // var villager = VillagerManager.GetVillagers()[villagerID];

        string originalRole = villager.CurrentRole.ToString();
        Roles.TryParse(EventSystem.current.currentSelectedGameObject.name, out Roles role);
        villager.CurrentRole = role;
        AddToVillagerLog(villager,villager.VillagerName + " has changed from " + originalRole + " to " + role);
        CloseAllUI();
    }
    
    public static void AddToVillagerLog(Villager villager, string newLog)
    {
        var storedLog = UIManager.villagerLog.GetValueOrDefault(villager, String.Empty);
        StringBuilder villagerLog = new StringBuilder(storedLog);
        villagerLog.Append(newLog);
        villagerLog.Append(Environment.NewLine);
        UIManager.villagerLog[villager] = villagerLog.ToString();
        // TODO Add To Villager Log In Scene When Built
    }

    // Button Set In Inspector
    public void ShowInventory()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }
    
    private void CloseAllUI()
    {
        _infoUI.SetActive(false);
        inventoryUI.SetActive(false);
        roleAssignmentUI.SetActive(false);
        villagerSelectUI.SetActive(false);
    }

    public void StockpileModeEnabled()
    {
        stockpileMode = !stockpileMode;
    }

    public void BuildMode()
    {
        _gameManager.cameraMovement.PlayerState = PlayerState.BuildMode;
    }
}

