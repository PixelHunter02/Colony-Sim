using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using UnityEditor;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _infoUI;
    private TMP_Text _villagerName;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject villagerManagementUI;

    [SerializeField] private GameObject villagerManagementCell;
    [SerializeField] private Transform villagerManagementContainer;
    

    private void Awake()
    {
        _villagerName = GameObject.Find("SelectedVillagerName").GetComponent<TMP_Text>();
        _infoUI.SetActive(false);
    }

    private void Start()
    {
        UpdateVillagerManagementUI();
    }

    // On Villager Click
    public void ShowWorkerInformation(string workerName)
    {
        _infoUI.SetActive(true);
        _villagerName.text = workerName;
    }

    // Button Set In Inspector
    public void ShowWorkerManagementUI()
    {
        villagerManagementUI.SetActive(!villagerManagementUI.activeSelf);
    }
    
    public void UpdateVillagerManagementUI()
    {
        for (int i = 0; i < WorkerManager.GetWorkers().Count; i++)
        {
            var worker = WorkerManager.GetWorkers()[i];
            Debug.Log(worker.WorkerName);
            var cell = Instantiate(villagerManagementCell, villagerManagementContainer);
            cell.transform.Find("Label").Find("Name").GetComponent<TMP_Text>().text = worker.WorkerName;
            var button = cell.transform.Find("Button").GetComponent<Button>();
            // Add A event to the button at runtime, which takes in a value
            UnityEditor.Events.UnityEventTools.AddIntPersistentListener(button.onClick, OpenRoleManagementUI,i);
            cell.SetActive(true);
        }
    }

    void OpenRoleManagementUI(int worker)
    {
        Debug.Log(WorkerManager.GetWorkers()[worker].WorkerName);
    }

    void SetWorkerRole(int worker)
    {
        Roles.TryParse(EventSystem.current.currentSelectedGameObject.name, out Roles role);
        WorkerManager.GetWorkers()[worker].CurrentRole = role;
    }

    // Button Set In Inspector
    public void ShowInventory()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }
}

