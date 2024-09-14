using UnityEngine;

public class NpcCarWheel : MonoBehaviour
{
    // Public reference to the WheelCollider component
    [Header("Wheel Settings")]
    public WheelCollider targetWheel;

    // Variables to store the wheel's position and rotation
    private Vector3 wheelPosition = Vector3.zero;
    private Quaternion wheelRotation = Quaternion.identity;

    // Update is called once per frame to sync the visual wheel with the WheelCollider
    private void Update()
    {
        // Get the world position and rotation of the WheelCollider
        targetWheel.GetWorldPose(out wheelPosition, out wheelRotation);

        // Apply the position and rotation to this object's transform
        transform.position = wheelPosition;
        transform.rotation = wheelRotation;
    }
}
