using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITask
{
    void MoveToLocation(Vector3 location, Action onArrivedAtPosition = null);
}
