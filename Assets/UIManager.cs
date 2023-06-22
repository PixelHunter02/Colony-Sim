using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    private GameManager _gameManager;
    
    [SerializeField] private GameObject _infoUI;
    private TMP_Text _villagerName;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject villagerManagementUI;

    [SerializeField] private GameObject villagerManagementCell;
    [SerializeField] private Transform villagerManagementContainer;

    [SerializeField] private Button[] roleButtons;
    [SerializeField] private GameObject roleAssignmentUI;

    public static Dictionary<Villager, string> villagerLog;

    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _villagerName = GameObject.Find("SelectedVillagerName").GetComponent<TMP_Text>();
        villagerLog = new Dictionary<Villager, string>();
        _infoUI.SetActive(false);
    }

    private void Start()
    {
        UpdateVillagerManagementUI();
    }

    // On Villager Click
    public void ShowVillagerInformation(Villager villager)
    {
        var storedLog = villagerLog.GetValueOrDefault(villager, String.Empty);
        _infoUI.SetActive(true);
        _villagerName.text = villager.VillagerName;
    }

    // Button Set In Inspector
    public void ShowVillagerManagementUI()
    {
        villagerManagementUI.SetActive(!villagerManagementUI.activeSelf);
    }
    
    public void UpdateVillagerManagementUI()
    {
        for (int i = 0; i < VillagerManager.GetVillagers().Count; i++)
        {
            var villager = VillagerManager.GetVillagers()[i];
            var cell = Instantiate(villagerManagementCell, villagerManagementContainer);
            cell.transform.Find("Label").Find("Name").GetComponent<TMP_Text>().text = villager.VillagerName;
            var button = cell.transform.Find("Button").GetComponent<Button>();
            // Add A event to the button at runtime, which takes in a value
            if (button.onClick.GetPersistentEventCount() >= 1)
            {
                UnityEditor.Events.UnityEventTools.RemovePersistentListener(button.onClick,0);
            }
            UnityEditor.Events.UnityEventTools.AddIntPersistentListener(button.onClick, OpenRoleManagementUI,i);
            cell.SetActive(true);
        }
    }

    void OpenRoleManagementUI(int villagerID)
    {
        foreach (var button in roleButtons)
        {
            if (button.onClick.GetPersistentEventCount() >= 1)
            {
                UnityEditor.Events.UnityEventTools.RemovePersistentListener(button.onClick,0);
            }
            UnityEditor.Events.UnityEventTools.AddIntPersistentListener(button.onClick, SetVillagerRole,villagerID);
        }
        CloseAllUI();
        villagerManagementUI.SetActive(true);
        roleAssignmentUI.SetActive(true);
    }

    void SetVillagerRole(int villagerID)
    {
        var villager = VillagerManager.GetVillagers()[villagerID];

        string originalRole = villager.CurrentRole.ToString();
        Roles.TryParse(EventSystem.current.currentSelectedGameObject.name, out Roles role);
        villager.CurrentRole = role;
        AddToVillagerLog(villager,villager.VillagerName + " has changed from " + originalRole + " to " + role);

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
        villagerManagementUI.SetActive(false);
        roleAssignmentUI.SetActive(false);
    }
}

