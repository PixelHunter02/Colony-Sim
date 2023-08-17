using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        current = this;
    }

    public event Action onNightTimeStart;
    public void NightTimeStart()
    {
        if (onNightTimeStart !=null)
        {
            onNightTimeStart();
        }
    }

    public event Action onNightTimeEnd;
    public void NightTimeEnd()
    {
        if (onNightTimeEnd != null)
        {
            onNightTimeEnd();
        }
    }
}
