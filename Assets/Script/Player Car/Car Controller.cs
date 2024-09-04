using UnityEngine;

public class CarController : MonoBehaviour
{
    // Inputs
    private float _horizontalInput;
    private float _verticalInput;
    private bool _isBraking;

    // Car controls
    private float _currentSteerAngle;
    private float _currentBrakeForce;

    // Seriaalized fields for easy tuning in the Unity Editor
    [SerializeField] private float engineForce;
    [SerializeField] private float brakeForce;
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private float maxSpeed;

    // Wheel colliders for handling car physics
    [SerializeField] WheelCollider frontLeftWheelCollider;
    [SerializeField] WheelCollider frontRightWheelCollider;
    [SerializeField] WheelCollider rearLeftWheelCollider;
    [SerializeField] WheelCollider rearRightWheelCollider;

    // Wheel transforms for updating visual position and rotation
    [SerializeField] Transform frontLeftWheelTransform;
    [SerializeField] Transform frontRightWheelTransform;
    [SerializeField] Transform rearLeftWheelTransform;
    [SerializeField] Transform rearRightWheelTransform;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    private void FixedUpdate()
    {
        GetInput();
        LimitSpeed();
        HandleMotor();
        HandleSteering();
        HandleBraking();
        UpdateWheels();
        LimitSpeedDuringBraking();
    }

    // Get user input for steering, acceleration, and braking
    private void GetInput()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        _isBraking = Input.GetKey(KeyCode.Space);
    }

    // Limit the car's speed to the maximum speed set in the editor
    private void LimitSpeed()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxSpeed);
    }

    // Apply motor torque to the front wheels based on vertical input (acceleration/reverse)
    private void HandleMotor()
    {
        float motorTorque = engineForce * _verticalInput;
        frontLeftWheelCollider.motorTorque = motorTorque;
        frontRightWheelCollider.motorTorque = motorTorque;
    }

    // Apply steering to the front wheels based on horizontal input (turning)
    private void HandleSteering()
    {
        _currentSteerAngle = maxSteerAngle * _horizontalInput;
        frontLeftWheelCollider.steerAngle = _currentSteerAngle;
        frontRightWheelCollider.steerAngle = _currentSteerAngle;
    }

    // Apply braking force when the spacebar is pressed
    private void HandleBraking()
    {
        _currentBrakeForce = _isBraking ? brakeForce : 0f;
        ApplyBrakingToWheels(_currentBrakeForce);
    }

    // Apply brake force to all wheels
    private void ApplyBrakingToWheels(float brakeForce)
    {
        frontLeftWheelCollider.brakeTorque = brakeForce;
        frontRightWheelCollider.brakeTorque = brakeForce;
        rearLeftWheelCollider.brakeTorque = brakeForce;
        rearRightWheelCollider.brakeTorque = brakeForce;
    }

    // Update the position and rotation of each wheel to match the WheelCollider
    private void UpdateWheels()
    {
        UpdateWheelPose(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateWheelPose(frontRightWheelCollider, frontRightWheelTransform);
        UpdateWheelPose(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateWheelPose(rearRightWheelCollider, rearRightWheelTransform);
    }

    // Update the wheel's transform to match the WheelCollider's pose
    private void UpdateWheelPose(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 position;
        Quaternion rotation;

        wheelCollider.GetWorldPose(out position, out rotation);
        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
    }

    // Limit the car's speed during braking to avoid sliding
    private void LimitSpeedDuringBraking()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (_isBraking && rigidbody.velocity.magnitude > 20f)
        {
            rigidbody.velocity = rigidbody.velocity.normalized * 20f;
        }
    }
}