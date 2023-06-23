using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStateChange : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        StartCoroutine(Flicker());
    }

    private IEnumerator Flicker()
    {
        yield return new WaitForSeconds(0.5f);

    }
}
