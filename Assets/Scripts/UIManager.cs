using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text health;
    [SerializeField] private TMP_Text magic;
    [SerializeField] private TMP_Text craft; 
    [SerializeField] private TMP_Text strength;

    [SerializeField] private GameObject villagerManagementUI;
    [SerializeField] private GameObject villagerManagementTemplate;
    [SerializeField] private Transform villagerManagementContainer;
    public Dictionary<Villager, GameObject> templateDictionary;

    public Slider zoomSlider;
    [SerializeField] private CinemachineCameraOffset _cinemachineCameraOffset;
    [SerializeField] private CinemachineVirtualCamera _cinemachineVCam;

    GameManager gameManager;
    public Transform roleManagementContainer;

    private void Awake()
    {
        templateDictionary = new Dictionary<Villager, GameObject>();
        gameManager = GameManager.Instance;
    }

    public void SetVillagerStatsUI(Villager villager)
    {
        health.text = villager.VillagerStats.Health.ToString();
        magic.text = villager.VillagerStats.Magic.ToString();
        craft.text = villager.VillagerStats.Craft.ToString();
        strength.text = villager.VillagerStats.Strength.ToString();
    }

    public void OpenVillagerMenu()
    {
        villagerManagementUI.SetActive(true);
        foreach (var villager in VillagerManager.GetVillagers())
        {
            if (templateDictionary.ContainsKey(villager))
            {
                var templateGO = templateDictionary[villager];
                templateGO.SetActive(true);
                var template = templateGO.transform;
                template.GetChild(0).GetComponent<RawImage>().texture = villager._portraitRenderTexture;
                template.Find("NameBorder").GetChild(0).GetComponent<TMP_Text>().text = villager.VillagerStats.VillagerName;
                template.Find("StatsBorder").Find("Health").GetChild(0).GetComponent<TMP_Text>().text =
                    villager.VillagerStats.Health.ToString();
                template.Find("StatsBorder").Find("Strength").GetChild(0).GetComponent<TMP_Text>().text =
                    villager.VillagerStats.Strength.ToString();
                template.Find("StatsBorder").Find("Magic").GetChild(0).GetComponent<TMP_Text>().text =
                    villager.VillagerStats.Magic.ToString();
                template.Find("StatsBorder").Find("Craft").GetChild(0).GetComponent<TMP_Text>().text =
                    villager.VillagerStats.Craft.ToString();
                var dropdown = template.Find("Dropdown").GetComponent<TMP_Dropdown>();
                var goToButton = template.Find("Portrait").GetComponent<Button>();
                goToButton.onClick.AddListener(delegate { GoToVillager(villager); });
                dropdown.value = (int)villager.CurrentRole;
                dropdown.onValueChanged.AddListener(delegate { RoleChanged(dropdown.value,villager); });
            }
            else
            {
                var template = Instantiate(villagerManagementTemplate, villagerManagementContainer).transform;
                template.GetChild(0).GetComponent<RawImage>().texture = villager._portraitRenderTexture;
                template.Find("NameBorder").GetChild(0).GetComponent<TMP_Text>().text = villager.VillagerStats.VillagerName;
                template.Find("StatsBorder").Find("Health").GetChild(0).GetComponent<TMP_Text>().text =
                    villager.VillagerStats.Health.ToString();
                template.Find("StatsBorder").Find("Strength").GetChild(0).GetComponent<TMP_Text>().text =
                    villager.VillagerStats.Strength.ToString();
                template.Find("StatsBorder").Find("Magic").GetChild(0).GetComponent<TMP_Text>().text =
                    villager.VillagerStats.Magic.ToString();
                template.Find("StatsBorder").Find("Craft").GetChild(0).GetComponent<TMP_Text>().text =
                    villager.VillagerStats.Craft.ToString();
                var dropdown = template.Find("Dropdown").GetComponent<TMP_Dropdown>();
                var goToButton = template.Find("Portrait").GetComponent<Button>();
                goToButton.onClick.AddListener(delegate { GoToVillager(villager); });
                dropdown.value = (int)villager.CurrentRole;
                if (dropdown.value != (int)Roles.Leader)
                {
                    dropdown.onValueChanged.AddListener(delegate { StartCoroutine(RoleChanged(dropdown.value,villager)); });
                }
                else
                {
                    dropdown.interactable = false;
                }
                templateDictionary.Add(villager,template.gameObject);    
            }
        }
    }

    private IEnumerator RoleChanged(int value, Villager villager)
    {
        foreach (var item in StorageManager.itemList)
        {
            if ((int)item.itemSO.assignRole == value)
            {
                villager.ignoreQueue = true;
                villager.StopAllCoroutines();
                yield return gameManager.taskHandler.WalkToLocationCR(villager, item.storageLocation);
                villager.CurrentRole = (Roles)value;
                villager.CurrentState = VillagerStates.AssigningRole;
                StorageManager.EmptyStockpileSpace(item);
                yield return new WaitForSeconds(villager._animator.GetCurrentAnimatorStateInfo(0).length);
                villager.CurrentState = VillagerStates.Idle;
                villager.ignoreQueue = false;
                yield break;
            }
        }
    }

    private void GoToVillager(Villager villager)
    {
        gameManager.level.followObject.transform.position = new Vector3(villager.transform.position.x, 0, villager.transform.position.z);
    }
    
    public void Zoom()
    {
        var zoomValue = zoomSlider.value;
        zoomSlider.value = zoomValue;
        
        const int minimumZoomValue = -5;
        const int maximumZoomValue = 0;
        _cinemachineCameraOffset.m_Offset.z = -zoomValue;
        _cinemachineCameraOffset.m_Offset.z =
            Mathf.Clamp(_cinemachineCameraOffset.m_Offset.z, minimumZoomValue, maximumZoomValue);
        var transposerOffset = _cinemachineVCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y;
        transposerOffset = zoomValue * 2;
        _cinemachineVCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y = Mathf.Clamp(transposerOffset, 3, 15);
    }
}
