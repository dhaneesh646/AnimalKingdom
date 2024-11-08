using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plyerfollower : MonoBehaviour
{
    public Transform target;
    void Update()
    {
        if (target != null)
        {
            if (target != null)
            {
                // Create a new position for the following GameObject
                Vector3 newPosition = new Vector3(target.position.x, 0f, target.position.z);

                // Set the position of the following GameObject
                transform.position = newPosition;
            }
        }
    }
}
