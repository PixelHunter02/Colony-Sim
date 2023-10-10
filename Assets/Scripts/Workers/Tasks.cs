using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG_Tasks
{
    public class Tasks : MonoBehaviour
    {
        public static IEnumerator WalkToLocation(Villager villager, Vector3 position, Action onComplete = null)
        {
            Villager.StopVillager(villager,false);
            Villager.SetVillagerDestination(villager, position);
            villager.CurrentState = VillagerStates.Walking;


            while (Vector3.Distance(villager.transform.position, position) > 0.5f )
            {
                yield return null;
            }
            
            villager.CurrentState = VillagerStates.Idle;

            onComplete?.Invoke();
        }

        public static IEnumerator PickUpItem(Villager villager, ObjectInformation item, Action onPickup = null)
        {
            villager.CurrentState = VillagerStates.Walking;
            yield return GameManager.Instance.StartCoroutine(WalkToLocation(villager, item.transform.position));

            Villager.StopVillager(villager,true);
            villager.CurrentState = VillagerStates.Pickup;
            yield return new WaitForSeconds(0.5f);
            Villager.StopVillager(villager,false);
            item.gameObject.SetActive(false);

            onPickup?.Invoke();
        }

        public static IEnumerator StoreItem(Villager villager, ObjectInformation item, Action onStorage = null)
        {
            yield return GameManager.Instance.StartCoroutine(PickUpItem(villager, item));
            yield return GameManager.Instance.StartCoroutine(WalkToLocation(villager, item.storageLocation));
            yield return GameManager.Instance.StartCoroutine(PlaceItem(villager, item));
        }
        
        public static IEnumerator PlaceItem(Villager villager, ObjectInformation objectInformation)
        {
            villager.CurrentState = VillagerStates.Pickup;
            yield return new WaitForSeconds(0.5f);
            
            objectInformation.transform.position = objectInformation.storageLocation;
            objectInformation.gameObject.SetActive(true);
            objectInformation.transform.rotation = Quaternion.Euler(0,0,0);
            objectInformation._isStored = true;
            GameManager.Instance.storageManager.AddToStorage(new Item{itemSO = objectInformation.Item, amount = 1, storageLocation = objectInformation.storageLocation, go = objectInformation.gameObject});
            villager.CurrentState = VillagerStates.Idle;
        }
    } 
}

