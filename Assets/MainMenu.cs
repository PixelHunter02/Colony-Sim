using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject characterCreation;
    public GameObject mainMenu;
    public Material[] hairColours;

    public void BeginCharacterCreation()
    {
        characterCreation.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void ChangeHairColour()
    {
        
    }

    public void ChangeGender()
    {
        
    }
}
