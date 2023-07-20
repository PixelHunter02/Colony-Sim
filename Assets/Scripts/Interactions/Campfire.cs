using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour,IInteractable
{
    public void OnInteraction()
    {
        Debug.Log("Opening CampfireUI");
    }
}
