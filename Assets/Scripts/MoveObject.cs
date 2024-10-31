using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    
    public float moveSpeed;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (Input.touchCount > 0 || Input.GetKeyDown(KeyCode.T))
        {
            
            rb.velocity = transform.forward * moveSpeed;
        }
    }
}
