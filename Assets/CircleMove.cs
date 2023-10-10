using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMove : MonoBehaviour
{
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime*3;
        var posX = Mathf.Sin(timer) * 15;
        var posY = Mathf.Cos(timer) * 15;

        transform.position = new Vector3(1619+posX, 320+posY, 0);
    }
}
