using System.Collections.Generic;
using UnityEngine;

public class NpcCarAI : MonoBehaviour
{
    [Header("Path")]
    [SerializeField] private Transform path;

    private List<Transform> nodes = new List<Transform>();
    [SerializeField] private int currentNode = 0;

    [Header("Car Settings")]
    [SerializeField] private float maxSteerAngle = 45f;
    public float turnSpeed = 5f;
    [SerializeField] private float maxMotorTorque = 50f;
    [SerializeField] private float maxBrakeTorque = 150f;
    [SerializeField] private Vector3 centerOfMassOffset;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float distancePoint = 0.5f;

    [Header("Braking")]
    public bool isBraking = false;
    [SerializeField] private Texture2D textureNormal;
    [SerializeField] private Texture2D textureBraking;
    [SerializeField] private Renderer carRenderer;

    [Header("Wheels")]
    [SerializeField] private WheelCollider wheelFL;
    [SerializeField] private WheelCollider wheelFR;
    [SerializeField] private WheelCollider wheelRL;
    [SerializeField] private WheelCollider wheelRR;


    [Header("Sensors")]
    public float sensorLength = 6f;
    public Vector3 frontCenterSensorPos = new Vector3(0f, 0.2f, 0.5f);
    public float frontSideSensorPos = 1.3f;
    public float frontSensorAngle = 30f;
    [SerializeField] private bool avoiding = false;
    private float targetSteerAngle = 0f;


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
        Sensors();
        ApplySteer();
        Drive();
        CheckWaypointDistance();
        HandleBraking();
        LerpToSteerAngle();
    }

    private void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * frontCenterSensorPos.z;
        sensorStartPos += transform.up * frontCenterSensorPos.y;
        float avoidMultiplier = 0f;
        avoiding = false;
        isBraking = false;

        // Front right sensor
        sensorStartPos += transform.right * frontSideSensorPos;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if (!hit.collider.CompareTag("Terrains"))
            {
                Debug.DrawLine(sensorStartPos, hit.point, color: Color.red);
                avoiding = true;
                avoidMultiplier -= 1f;

                if (hit.distance < sensorLength)
                {
                    isBraking = true;
                }
            }
        }
        // Front right angle sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            if (!hit.collider.CompareTag("Terrains"))
            {
                Debug.DrawLine(sensorStartPos, hit.point, color: Color.red);
                avoiding = true;
                avoiding = true;
                avoidMultiplier -= 0.5f;

                //if (hit.distance < sensorLength)
                //{
                //    isBraking = true;
                //}
            }
        }


        // Front left sensor
        sensorStartPos -= transform.right * frontSideSensorPos * 2;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if (!hit.collider.CompareTag("Terrains"))
            {
                Debug.DrawLine(sensorStartPos, hit.point, color: Color.red);
                avoiding = true;
                avoiding = true;
                avoidMultiplier += 1f;

                if (hit.distance < sensorLength)
                {
                    isBraking = true;
                }
            }
        }
        // Front left angle sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            if (!hit.collider.CompareTag("Terrains"))
            {
                Debug.DrawLine(sensorStartPos, hit.point, color: Color.red);
                avoiding = true;
                avoiding = true;
                avoidMultiplier += 0.5f;

                //if (hit.distance < sensorLength)
                //{
                //    isBraking = true;
                //}
            }
        }

        // Front center sensor
        if (avoidMultiplier == 0)
        {
            if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
            {
                if (!hit.collider.CompareTag("Terrains"))
                {
                    Debug.DrawLine(sensorStartPos, hit.point, color: Color.red);
                    avoiding = true;
                    if (hit.normal.x < 0)
                    {
                        avoidMultiplier = -1;
                    }
                    else
                    {
                        avoidMultiplier = 1;
                    }

                    if (hit.distance < sensorLength)
                    {
                        isBraking = true;
                    }
                }
            }

        }


        if (avoiding)
        {
            targetSteerAngle = maxSteerAngle * avoidMultiplier;
        }

    }


    /// <summary>
    /// Applies steering based on the direction towards the next waypoint.
    /// </summary>
    private void ApplySteer()
    {

        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        float steerAngle = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;

        targetSteerAngle = steerAngle;
        //wheelFL.steerAngle = steerAngle;
        //wheelFR.steerAngle = steerAngle;
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

        if (isBraking)
        {
            wheelFR.brakeTorque = maxBrakeTorque;
            wheelFL.brakeTorque = maxBrakeTorque;
        }
        else
        {
            wheelFR.brakeTorque = 0;
            wheelFL.brakeTorque = 0;
        }
    }

    /// <summary>
    /// Checks the distance to the next waypoint and switches to the next one if close enough.
    /// </summary>
    private void CheckWaypointDistance()
    {

        if (Vector3.Distance(transform.position, nodes[currentNode].position) < distancePoint)
        {
            if (currentNode == nodes.Count - 1)
            {
                currentNode = 0;
            }
            else
            {
                currentNode++;
                //Debug.Log(currentNode);
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

    private void LerpToSteerAngle()
    {
        wheelFL.steerAngle = Mathf.Lerp(wheelFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        wheelFR.steerAngle = Mathf.Lerp(wheelFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
    }
}
