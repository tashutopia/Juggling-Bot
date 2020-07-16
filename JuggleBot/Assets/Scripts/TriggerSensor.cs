using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSensor : MonoBehaviour
{
    public static Vector3 BallPosition;
    private int localCount = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        // Updates the position of the ball passing through the TriggerSensor
        // Only when the totalCount is updated
        if(localCount != JuggleController.totalCount)
        {
            BallPosition = other.transform.position;
            localCount++;
        }
    }
}
