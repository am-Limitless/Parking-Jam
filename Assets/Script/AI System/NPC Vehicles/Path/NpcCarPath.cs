using System.Collections.Generic;
using UnityEngine;

public class NpcCarPath : MonoBehaviour
{
    // Public field to define the color of the Gizmos lines in the Unity Editor
    [Header("Gizmo Settings")]
    public Color lineColor = Color.white;

    // List to store the node positions of the car path
    private List<Transform> nodes = new List<Transform>();

    // This method is called by Unity when the object is selected in the editor and draws the Gizmos
    private void OnDrawGizmosSelected()
    {
        // Set the color for drawing the Gizmos
        Gizmos.color = lineColor;

        // Get all child transforms, but avoid adding the parent (this object's transform)
        Transform[] pathTransforms = GetComponentsInChildren<Transform>();

        // Clear the nodes list instead of creating a new one for better performance
        nodes.Clear();

        // Add all child transforms except the parent
        foreach (Transform pathTransform in pathTransforms)
        {
            if (pathTransform != transform)
            {
                nodes.Add(pathTransform);
            }
        }

        // Draw lines and spheres between the nodes
        for (int i = 0; i < nodes.Count; i++)
        {
            // Get the position of the current node
            Vector3 currentNodePosition = nodes[i].position;

            // Determine the previous node's position
            Vector3 previousNodePosition;
            if (i > 0)
            {
                previousNodePosition = nodes[i - 1].position;  // Connect to the previous node
            }
            else
            {
                previousNodePosition = nodes[nodes.Count - 1].position;  // Connect the first node to the last node (loop)
            }

            // Draw the line between the previous and current node
            Gizmos.DrawLine(previousNodePosition, currentNodePosition);

            // Draw a wireframe sphere at the current node's position for better visibility
            Gizmos.DrawWireSphere(currentNodePosition, 0.3f);
        }
    }
}
