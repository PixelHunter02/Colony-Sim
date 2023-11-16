using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    /// <summary>
    /// Audio
    /// </summary>
    private float masterVolume;
    
    private List<AudioSource> villagerAudioSources;

    /// <summary>
    /// Camera
    /// </summary>
    public bool invertY;
    public bool invertX;
    public float rotationSpeed = 10;

    public int CameraXModifier()
    {
        int modifierValue;
        if (invertX)
        {
            modifierValue = -1;
        }
        else
        {
            modifierValue = 1;
        }

        return modifierValue;
    }
}
