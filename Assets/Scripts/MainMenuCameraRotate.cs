using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuCameraRotate : MonoBehaviour
{
    void FixedUpdate()
    {
        transform.localPosition += transform.right * -0.008f;
        transform.Rotate(new Vector3(0,0.05f,0),Space.World);
    }
}
