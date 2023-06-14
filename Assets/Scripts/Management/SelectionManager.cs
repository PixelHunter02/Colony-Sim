// using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;
    public List<GameObject> selected;

    private void Update()
    {
        // Highlight
        if (highlight != null)
        {
            highlight.gameObject.GetComponent<Outline>().enabled = false;
            highlight = null;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //if cursor is over gameobject
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit)) //Make sure you have EventSystem in the hierarchy before using EventSystem
        {
            highlight = raycastHit.transform;
            if (highlight.CompareTag("Selectable") && highlight != selection)
            {
                
                if (highlight.gameObject.GetComponent<Outline>() != null)
                {
                    if (!selected.Contains(raycastHit.transform.gameObject))
                    {
                        highlight.gameObject.GetComponent<Outline>().OutlineColor = Color.white;
                        highlight.gameObject.GetComponent<Outline>().enabled = true;
                        highlight.gameObject.GetComponent<Outline>().OutlineWidth = 2.0f;
                    }
                }

                else//clicked
                {
                    
                    Outline outline = highlight.gameObject.AddComponent<Outline>();
                    outline.enabled = true;
                    highlight.gameObject.GetComponent<Outline>().OutlineWidth = 7.0f;
                    foreach (GameObject select in selected)
                    {
                        if (highlight.gameObject.transform.name == select.transform.name)
                        {
                            print("amoogus");
                        }
                    }
                }
               
            }
        }
        // Selection
        if (Input.GetMouseButtonDown(0))
        {
            if (highlight)
            {
                if (selection != null)
                {
                    selection.gameObject.GetComponent<Outline>().enabled = false;//if nothing selected?
                }
                selection = raycastHit.transform;
                selection.gameObject.GetComponent<Outline>().enabled = true;
                highlight.gameObject.GetComponent<Outline>().OutlineWidth = 7.0f;
                highlight.gameObject.GetComponent<Outline>().OutlineColor = Color.red;
                highlight = null;

                if(!selected.Contains(selection.gameObject))
                {
                    selected.Add(selection.gameObject);
                }
            }
            else
            {
                foreach(GameObject unit in selected)
                {
                    unit.gameObject.GetComponent<Outline>().OutlineColor.Equals(new Color(1f,1f,1f,0f));
                }
                
            }
        }
    }
}


