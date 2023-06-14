using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
public class OnHover : MonoBehaviour
{
    private Outline outline;
    private NewSelections selection;

    private void Awake()
    {
        //selection = GameObject.Find("SelectionManager").gameObject.GetComponent<NewSelections>();
        outline = gameObject.GetComponent<Outline>();
    }

    private void OnMouseEnter()
    {
        gameObject.GetComponent<Outline>().enabled = true;
    }
    private void OnMouseExit()
    {
        gameObject.GetComponent<Outline>().enabled = false;
    }

    // private void OnMouseEnter()
    // {
    //     if (!selection.selectedCharacters.Contains(gameObject))
    //     {
    //         gameObject.GetComponent<Outline>().enabled = true;
    //         gameObject.GetComponent<Outline>().OutlineColor = Color.white;
    //     }
    //     else if (selection.selectedCharacters.Contains(gameObject))
    //     {
    //         gameObject.GetComponent<Outline>().OutlineColor = Color.cyan;
    //         gameObject.GetComponent<Outline>().OutlineWidth = 10;
    //     }
    // }
    // private void OnMouseExit()
    // {
    //     gameObject.GetComponent<Outline>().OutlineWidth = 4;
    //     gameObject.GetComponent<Outline>().enabled = false;
    //     if (selection.selectedCharacters.Contains(gameObject))
    //     {
    //         gameObject.GetComponent<Outline>().enabled = true;
    //     }
    // }
}
