using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBallTest : MonoBehaviour
{
    public Vector3 ThrowingPower = new Vector3(0.0f, 5.0f, 0.0f);
    public Rigidbody rb;
    
    bool check = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            check = true;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
         if (check)
         {
             rb.AddForce(ThrowingPower, ForceMode.Impulse);
             check=false;
         }
        
    }
}
