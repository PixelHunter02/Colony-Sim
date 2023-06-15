using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    public Vector3 storedAt;
    [SerializeField] private bool isBeingHeld;
    public Worker heldBy;

    public void Pickup(Worker workerToPickUp)
    {
        isBeingHeld = true;
        heldBy = workerToPickUp;
    }
}
