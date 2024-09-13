using System.Collections.Generic;
using UnityEngine;

public class NpcCarAI : MonoBehaviour
{
    [Header("Path")]
    [SerializeField] private Transform path;

    private List<Transform> nodes = new List<Transform>();
    private int currentNode = 0;

    [Header("Car Settings")]
    [SerializeField] private float maxSteerAngle = 45f;
    [SerializeField] private float maxMotorTorque = 50f;
    [SerializeField] private float maxBrakeTorque = 150f;
    [SerializeField] private Vector3 centerOfMassOffset;
    [SerializeField] private float maxSpeed = 100f;
    private float currentSpeed;

    [Header("Braking")]
    public bool isBraking = false;
    [SerializeField] private Texture2D textureNormal;
    [SerializeField] private Texture2D textureBraking;

    [SerializeField] private Renderer carRenderer;

    [Header("Wheels")]
    [SerializeField] private WheelCollider wheelFL;
    [SerializeField] private WheelCollider wheelFR;
    [SerializeField] private WheelCollider wheelRR;
    [SerializeField] private WheelCollider wheelRL;

    private Rigidbody rb;

    private void Start()
    {
        // Cache the Rigidbody for better performance
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMassOffset;

        // Initialize the path nodes
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        foreach (var t in pathTransforms)
        {
            if (t != path.transform)  // Skip the parent path object
            {
                nodes.Add(t);
            }
        }
    }

    private void FixedUpdate()
    {
        ApplySteer();
        Drive();
        CheckWaypointDistance();
        HandleBraking();
    }

    /// <summary>
    /// Applies steering based on the direction towards the next waypoint.
    /// </summary>
    private void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        float steerAngle = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;

        wheelFL.steerAngle = steerAngle;
        wheelFR.steerAngle = steerAngle;
    }

    /// <summary>
    /// Controls the car's driving behavior based on speed and braking state.
    /// </summary>
    private void Drive()
    {
        currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 100;

        if (currentSpeed < maxSpeed && !isBraking)
        {
            wheelFL.motorTorque = maxMotorTorque;
            wheelFR.motorTorque = maxMotorTorque;
        }
        else
        {
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;
        }
    }

    /// <summary>
    /// Checks the distance to the next waypoint and switches to the next one if close enough.
    /// </summary>
    private void CheckWaypointDistance()
    {

        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 0.5f)
        {
            if (currentNode == nodes.Count - 1)
            {
                currentNode = 0;
            }
            else
            {
                currentNode++;
            }
        }
    }

    /// <summary>
    /// Handles braking and visual texture changes based on the braking state.
    /// </summary>
    private void HandleBraking()
    {
        if (isBraking)
        {
            SetCarTexture(textureBraking);
            wheelRL.brakeTorque = maxBrakeTorque;
            wheelRR.brakeTorque = maxBrakeTorque;
        }
        else
        {
            SetCarTexture(textureNormal);
            wheelRL.brakeTorque = 0;
            wheelRR.brakeTorque = 0;
        }
    }

    /// <summary>
    /// Sets the texture of the car using MaterialPropertyBlock for performance.
    /// </summary>
    /// <param name="texture">The texture to apply.</param>
    private void SetCarTexture(Texture2D texture)
    {
        carRenderer.material.mainTexture = texture;
    }
}
