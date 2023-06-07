using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.AI;

public class NewSelections : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [SerializeField] private LayerMask selectableLayers;
    public List<GameObject> characters;
    public List<GameObject> selectedCharacters;

    private static NewSelections _instance;
    public static NewSelections Instance {  get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void ClickToSelect(InputAction.CallbackContext context)//onclick
    {
        if(context.phase == InputActionPhase.Started)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100, selectableLayers) && !selectedCharacters.Contains(hit.transform.gameObject)) 
            {
                selectedCharacters.Add(hit.transform.gameObject);
                hit.transform.gameObject.GetComponent<Outline>().enabled = true;
                hit.transform.gameObject.GetComponent<Outline>().OutlineColor = Color.cyan;
                hit.transform.gameObject.GetComponent<Outline>().OutlineWidth = 10;
            }
            else if (Physics.Raycast(ray, out RaycastHit hitt))//if not selecting character
            {
                foreach (GameObject select in selectedCharacters)
                {
                    select.GetComponent<NavMeshAgent>().SetDestination(hitt.point);
                }
            }
        }
    }
}
