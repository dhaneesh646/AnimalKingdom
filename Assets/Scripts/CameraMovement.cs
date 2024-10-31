using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JMRSDK.Toolkit.UI;

public class CameraMovement : MonoBehaviour
{
    public Transform[] targetAreas; // Array of target areas
    public float moveSpeed = 5f; // Speed of camera movement
    //public Button MoveButton;
    private int currentTargetIndex = 0; // Index of the current target area
    private bool isMoving = false;

    [SerializeField] private GameObject dockmanager;
    //public JMRUIButton button;

    private void Start()
    {
        // Find the TextMeshPro button in your scene and add an onClick listener
       // MoveButton.onClick.AddListener(MoveCamera);
       // button.onButtonClick.AddListener(MyMethod);
    }

    private void Update()
    {
        // If the camera is currently moving, you can add additional logic here
        if (Input.GetKeyDown(KeyCode.X))
        {
            dockmanager.SetActive(true);
        }
    }

    public void MoveCamera()
    {
        if (!isMoving)
        {
            // Move to the next target area
            currentTargetIndex++;
            if (currentTargetIndex >= targetAreas.Length)
            {
                currentTargetIndex = 0; // Loop back to the first target area
            }

            StartCoroutine(MoveToTargetArea(targetAreas[currentTargetIndex]));
        }
    }

    private IEnumerator MoveToTargetArea(Transform targetArea)
    {
        isMoving = true;

        // Store the camera's initial position and rotation
        Vector3 initialPosition = transform.position;
        Quaternion initialRotation = transform.rotation;

        float journeyLength = Vector3.Distance(initialPosition, targetArea.position);
        float startTime = Time.time;

        while (Time.time - startTime < journeyLength / moveSpeed)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float journeyFraction = distanceCovered / journeyLength;

            // Interpolate camera position and rotation towards the target
            transform.position = Vector3.Lerp(initialPosition, targetArea.position, journeyFraction);
            transform.rotation = Quaternion.Slerp(initialRotation, targetArea.rotation, journeyFraction);

            yield return null;
        }

        // Ensure the camera reaches the exact target position and rotation
        transform.position = targetArea.position;
        transform.rotation = targetArea.rotation;

        isMoving = false;
    }
}
