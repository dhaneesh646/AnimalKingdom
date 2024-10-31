using UnityEngine;

public class CopyTransform : MonoBehaviour
{
    public Transform sourceTransform; // Reference to the source GameObject's transform

    private void Update()
    {
        if (sourceTransform != null)
        {
            // Copy the rotation from the source GameObject
            transform.rotation = sourceTransform.rotation;

            // Copy the Y-axis position from the source GameObject
            Vector3 newPosition = transform.position;
            newPosition.y = sourceTransform.position.y;
            newPosition.z = sourceTransform.position.z;
            transform.position = newPosition;
        }
        else
        {
            Debug.LogWarning("Source Transform is not assigned.");
        }
    }
}
