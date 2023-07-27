using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    /// <summary>
    /// Audio
    /// </summary>
    public float musicVolume;
    public float villagerVolume;
    public float interactionVolume;

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
