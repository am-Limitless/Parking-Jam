using System.Collections.Generic;
using UnityEngine;

public class NpcCarAI : MonoBehaviour
{
    public Transform path;

    private List<Transform> nodes;

    private int currentNode = 0;

    public float maxSteerAngle = 45f;

    public float maxMotorTorque = 50f;

    public Vector3 centerOfMassOffset;

    public bool isBraking = false;

    public float currentSpeed;
    public float maxSpeed = 100f;

    public Texture2D textureNormal;
    public Texture2D textureBraking;
    public Renderer carRendere;

    public WheelCollider wheelFL;
    public WheelCollider wheelFR;

    private void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMassOffset;

        Transform[] pathTranforms = path.GetComponentsInChildren<Transform>();

        nodes = new List<Transform>();

        for (int i = 0; i < pathTranforms.Length; i++)
        {
            if (pathTranforms[i] != path.transform)
            {
                nodes.Add(pathTranforms[i]);
            }
        }
    }

    private void FixedUpdate()
    {
        ApplySteer();
        Drive();
        CheckWaypointDistance();
        Braking();
    }



    private void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);

        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;

        wheelFL.steerAngle = newSteer;
        wheelFR.steerAngle = newSteer;
    }

    private void Drive()
    {
        currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 100;

        if (currentSpeed < maxSpeed)
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

    private void Braking()
    {
        if (isBraking)
        {
            carRendere.material.mainTexture = textureBraking;
        }
        else
        {
            carRendere.material.mainTexture = textureNormal;
        }
    }
}
