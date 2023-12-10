using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

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
    public static float rotationSpeedModifier = 1;
    public static float zoomSpeedModifier = 1;
    public static float mousePanSpeedModifier = 1;
    public static float keyboardMoveSpeedModifier = 1;

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
