using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public GameObject InitialTarget;

    // Start is called before the first frame update
    void Start()
    {

        transform.position = InitialTarget.transform.position + new Vector3(0, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
