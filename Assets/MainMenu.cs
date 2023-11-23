// using System;
// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.Serialization;
//
// public class MainMenu : MonoBehaviour
// {
//     /// <summary>
//     /// Scenes
//     /// </summary>
//     public string gameScene;
//     
//     /// <summary>
//     /// Canvas's
//     /// </summary>
//     [FormerlySerializedAs("characterCreation")] public GameObject characterCreationCanvas;
//     [FormerlySerializedAs("mainMenu")] public GameObject mainMenuCanvas;
//     
//     /// <summary>
//     /// Customisations
//     /// </summary>
//     public Material[] hairColours;
//     private int currentHairColourIndex;
//     private int currentGenderIndex = 1;
//     
//     /// <summary>
//     /// Villager
//     /// </summary>
//     [FormerlySerializedAs("_villager")] [SerializeField] private Villager villager;
//
//     /// <summary>
//     /// Stats
//     /// </summary>
//     [SerializeField] private TMP_InputField _inputField;
//
//     private void Awake()
//     {
//         villager.VillagerCustomisation.HairColour = hairColours[0];
//         mainMenuCanvas.SetActive(true);
//         characterCreationCanvas.SetActive(false);
//
//         //Name Setup
//         _inputField.onEndEdit.AddListener(villager.EditName);
//         mainMenuCanvas.SetActive(false);
//     }
//
//     
//     public void BeginCharacterCreation()
//     {
//         characterCreationCanvas.SetActive(true);
//         mainMenuCanvas.SetActive(false);
//         villager.VillagerCustomisation.Gender = Model.Woman;
//         villager.CurrentRole = Roles.Leader;
//         _inputField.text = villager.VillagerStats.VillagerName;
//     }
//
//     public void NextHairColour()
//     {
//         currentHairColourIndex++;
//         if ( currentHairColourIndex > hairColours.Length-1)
//         {
//             currentHairColourIndex = 0;
//         }
//
//         villager.VillagerCustomisation.HairColour = hairColours[currentHairColourIndex];
//     }
//     public void PreviousHairColour()
//     {
//         currentHairColourIndex--;
//         if (currentHairColourIndex < 0)
//         {
//             currentHairColourIndex = hairColours.Length-1;
//         }
//
//         villager.VillagerCustomisation.HairColour = hairColours[currentHairColourIndex];
//     }
//     public void ChangeGender()
//     {
//         var gender = Enum.GetValues(typeof(Model));
//         currentGenderIndex++;
//         if (currentGenderIndex > gender.Length-1)
//         {
//             currentGenderIndex = 0;
//         }
//         villager.VillagerCustomisation.Gender = (Model)gender.GetValue(currentGenderIndex);
//         _inputField.text = villager.VillagerStats.VillagerName;
//     }
//     
//     public void ContinueToGame()
//     {
//             VillagerManager.AddVillagerToList(villager);
//             SceneManager.LoadScene(gameScene);
//     }
// }
