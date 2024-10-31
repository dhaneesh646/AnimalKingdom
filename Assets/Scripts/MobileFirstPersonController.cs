using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileFirstPersonController : MonoBehaviour
{
    public float speed = 5.0f;
    public AudioSource Gamemusic;
    private CharacterController characterController;

    private bool isWalking = false;
    private bool isHoldingForTwoSeconds = false;
    private float holdStartTime = 0f;
    private float holdDurationThreshold = 2f;

    void Start()
    {
        Gamemusic.Play();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Check if the player is touching the mobile screen
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Only respond to the first touch
            if (touch.phase == TouchPhase.Began)
            {
                isWalking = true;
                isHoldingForTwoSeconds = false;
                holdStartTime = Time.time;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isWalking = false;

                if (Time.time - holdStartTime >= holdDurationThreshold)
                {
                    isHoldingForTwoSeconds = true;
                }
            }
        }
        else
        {
            isWalking = false;
            isHoldingForTwoSeconds = false;
        }

        // Move the player
        if (isHoldingForTwoSeconds || Input.GetKeyDown(KeyCode.T) || isWalking)
        {
            // Get the forward direction of the player
            Vector3 forward = transform.TransformDirection(Vector3.forward);

            // Calculate the desired movement based on the speed and time
            Vector3 moveDirection = forward * speed * Time.deltaTime;

            // Move the character controller
            characterController.Move(moveDirection);
        }
    }
}