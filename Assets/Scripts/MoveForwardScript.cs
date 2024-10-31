using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForwardScript : MonoBehaviour
{
    public float speed;

    void Update()
    {
        if (Input.touchCount > 0 || Input.GetKeyDown(KeyCode.T))
        {
            transform.position += transform.forward * Time.deltaTime * speed;
        }
    }
}