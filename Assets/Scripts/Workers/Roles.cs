using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Roles : MonoBehaviour
{
    public enum Role 
    {
        Default,
        Lumberjack,
        Farmer,
        Fighter,
    }

    public Role role;

    private void Update()
    {
        switch (role)
        {
            case Role.Default:
                break;
            case Role.Lumberjack:
                break;
            case Role.Farmer:
                break;
            case Role.Fighter:
                break;
        }
    }
}
