using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchDetector : MonoBehaviour
{
    public static bool BallIsInHand = false;

    // Start is called before the first frame update
    void Start()
    {
        BallIsInHand = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        BallIsInHand = true;
    }

    void OnCollisionStay(Collision collision)
    {

    }

    void OnCollisionExit(Collision collision)
    {
        BallIsInHand = false;
    }

}
