using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycle : MonoBehaviour
{
    [SerializeField] private float dayLength;
    [SerializeField] private float nightLength;
    [SerializeField] private float speed;
    [SerializeField] private float currentTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += 1 * Time.deltaTime;
        transform.Rotate(new Vector3(1, 0, 0) * speed * Time.deltaTime);
        //transform.r = new Vector3(transform.rotation.x + Time.deltaTime * speed, 0, 0);

        // print(transform.localEulerAngles);
        if (gameObject.transform.localEulerAngles.x > 180 || gameObject.transform.localEulerAngles.x < 0)
        {
            print("goodbye light");
            gameObject.transform.GetComponent<Light>().enabled = false; 
        }
        else
        {
            gameObject.transform.GetComponent<Light>().enabled = true;
        }
        //https://www.youtube.com/watch?v=VZqqrNShOg0&ab_channel=JTAGames
    }
}
