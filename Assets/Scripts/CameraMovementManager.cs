using UnityEngine;
using UnityEngine.UI;
public class CameraMovementManager : MonoBehaviour
{
    public GameObject[] waypoints; // Array to store your waypoints (game objects)
    public float cameraSpeed = 5.0f; // Camera movement speed
    private int currentWaypointIndex = 0; // Index to track the current waypoint
    public Button NextButton;

    private void Start()
    {
        if (waypoints.Length > 0)
        {
            MoveCameraToWaypoint(currentWaypointIndex);
        }
    }

    private void changecamera()
    {
        NextButton.onClick.AddListener(NextWaypoint);
    }
    private void Update()
    {
        // Check for button press, e.g., when the "Next" button is pressed
        if (Input.GetKeyDown(KeyCode.Space)) // Change KeyCode to the desired button
        {
            // Move to the next waypoint
            NextWaypoint();
        }
    }

    public void NextWaypoint()
    {
        // Increment the waypoint index
        currentWaypointIndex++;

        // Check if we've reached the end of the array
        if (currentWaypointIndex >= waypoints.Length)
        {
            currentWaypointIndex = 0; // Loop back to the first waypoint
        }

        // Move the camera to the next waypoint
        MoveCameraToWaypoint(currentWaypointIndex);
    }

    private void MoveCameraToWaypoint(int index)
    {
        if (index >= 0 && index < waypoints.Length)
        {
            // Calculate the target position (centered on the waypoint)
            Vector3 targetPosition = waypoints[index].transform.position;

            // Smoothly move the camera towards the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * cameraSpeed);
        }
    }
}
