using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OnHover : MonoBehaviour
{
    [SerializeField] Outline outline;
    [SerializeField] Component selection;

    private void OnMouseEnter()
    {
        if (!selection.GetComponent<NewSelections>().selectedCharacters.Contains(gameObject))
        {
            gameObject.GetComponent<Outline>().enabled = true;
            gameObject.GetComponent<Outline>().OutlineColor = Color.white;
        }
        else if (selection.GetComponent<NewSelections>().selectedCharacters.Contains(gameObject))
        {
            gameObject.GetComponent<Outline>().OutlineColor = Color.cyan;
            gameObject.GetComponent<Outline>().OutlineWidth = 10;
        }
    }
    private void OnMouseExit()
    {
        gameObject.GetComponent<Outline>().OutlineWidth = 4;
        gameObject.GetComponent<Outline>().enabled = false;
        if (selection.GetComponent<NewSelections>().selectedCharacters.Contains(gameObject))
        {
            gameObject.GetComponent<Outline>().enabled = true;
        }
    }
}
