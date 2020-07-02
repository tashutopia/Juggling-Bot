using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(move(transform.position + 5f*Vector3.up));
        }
    }

    // Update is called once per frame
    IEnumerator move(Vector3 destination)
    {
        while (transform.position != destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, 15f * Time.deltaTime);
            yield return null;
        }
    }
}
