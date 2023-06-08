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
    private Camera cam;
    
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

        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    public void ClickToSelect(InputAction.CallbackContext context)//onclick
    {
        if(context.phase == InputActionPhase.Started)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100) && !selectedCharacters.Contains(hit.transform.gameObject)) 
            {
                if (hit.transform.tag.Equals("Villagers"))
                {
                    selectedCharacters.Add(hit.transform.gameObject);
                    hit.transform.gameObject.GetComponent<Outline>().enabled = true;
                    hit.transform.gameObject.GetComponent<Outline>().OutlineColor = Color.cyan;
                    hit.transform.gameObject.GetComponent<Outline>().OutlineWidth = 10;
                }
                else if (hit.transform.tag.Equals("HarvestableLumberJack"))
                {
                    for (int i = 0; i < selectedCharacters.Count; i++)
                    {
                        selectedCharacters[i].TryGetComponent<Worker>(out var worker);
                        if (worker._workerStates == Worker.WorkerStates.Available && worker.roles.role == Roles.Role.Lumberjack)
                        {
                            worker._workerStates = Worker.WorkerStates.Working;
                            worker.WorkerStateManagement(worker._workerStates,hit.transform.position,Tasks.Jobs.ChoppingTrees);
                            break;
                        }
                    }
                }
                else if (hit.transform.tag.Equals("Ground"))
                {
                    foreach (GameObject select in selectedCharacters)
                    {
                        if (select.GetComponent<Worker>()._workerStates != Worker.WorkerStates.Working)
                        {
                            select.GetComponent<NavMeshAgent>().SetDestination(hit.point);    
                        }
                    }  
                }


                
            }
            else if (Physics.Raycast(ray, out RaycastHit hitGround,100,3))//if not selecting character
            {
                
            }
        }
    }
}
