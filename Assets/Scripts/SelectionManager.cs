using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;
    public GameObject[] selected;

    // Update is called once per frame
    private void Start()
    {
    }
    void Update()
    {
        // Highlight
        if (highlight != null)
        {
            highlight.gameObject.GetComponent<Outline>().enabled = false;
            highlight = null;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit)) //Make sure you have EventSystem in the hierarchy before using EventSystem
        {
            highlight = raycastHit.transform;
            if (highlight.CompareTag("Selectable") && highlight != selection)
            {
                if (highlight.gameObject.GetComponent<Outline>() != null)//hover
                {
                    foreach (GameObject select in selected)
                    {
                        if (highlight.gameObject.transform.name == select.transform.name)//if its already in there remove it
                        {
                            highlight.gameObject.GetComponent<Outline>().enabled = false;
                            print("deselecting");
                        }
                        else
                        {
                            //selected .Add(highlight.gameObject);
                        }
                    }
                    highlight.gameObject.GetComponent<Outline>().OutlineColor = Color.red;
                    highlight.gameObject.GetComponent<Outline>().enabled = true;
                }
                else//clicked
                {
                    Outline outline = highlight.gameObject.AddComponent<Outline>();
                    outline.enabled = true;
                    highlight.gameObject.GetComponent<Outline>().OutlineWidth = 7.0f;
                    foreach(GameObject select in selected)
                    {
                        if (highlight.gameObject.transform.name == select.transform.name)
                        {
                            //add it to the array
                            print("amoogus");
                        }
                    }
                }
                highlight.gameObject.GetComponent<Outline>().OutlineColor = Color.white;
            }
            //else if (an interactable), dont deselect and instead use the selected array to interact. This is for you Sean <3
            else
            {
                highlight = null;
                //array is nulled
            }
        }

        // Selection
        if (Input.GetMouseButtonDown(0))
        {
            if (highlight)
            {
                if (selection != null)
                {
                    selection.gameObject.GetComponent<Outline>().enabled = false;
                }
                selection = raycastHit.transform;
                selection.gameObject.GetComponent<Outline>().enabled = true;
                highlight = null;
            }
            else
            {
                if (selection)
                {
                    selection.gameObject.GetComponent<Outline>().enabled = false;
                    selection = null;
                }
            }
        }
    }
}
