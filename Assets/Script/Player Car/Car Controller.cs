using UnityEngine;

public class CarController : MonoBehaviour
{
    // Inputs
    private float _horizontalInput;
    private float _verticalInput;
    private bool _isHandBraking;

    // Car controls
    private float _currentSteerAngle;
    private float _currentBrakeForce;

    // Seriaalized fields for easy tuning in the Unity Editor
    [SerializeField] private float engineForce;
    [SerializeField] private float brakeForce;
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float downForce;
    [SerializeField] private float wheelbase;
    [SerializeField] private float trackWidth;

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

    // AudioSource for engine sound
    [SerializeField] private AudioSource engineAudioSource;
    [SerializeField] private float minPitch = 0.5f;
    [SerializeField] private float maxPitch = 3.0f;

    private Rigidbody _rigidbody;
    [SerializeField] private GameObject _centerOfMass;

    private void FixedUpdate()
    {
        GetInput();
        LimitSpeed();
        HandleMotor();
        HandleSteering();
        HandleBraking();
        UpdateWheels();
        LimitSpeedDuringBraking();
        UpdateEngineSound();
        AddDownForce();
    }

    // Get user input for steering, acceleration, and braking
    private void GetInput()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        _isHandBraking = Input.GetKey(KeyCode.Space);
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
        rearLeftWheelCollider.motorTorque = motorTorque;
        rearRightWheelCollider.motorTorque = motorTorque;
    }

    // Apply steering to the front wheels based on horizontal input (turning)
    private void HandleSteering()
    {
        float turnRadius = wheelbase / Mathf.Tan(Mathf.Deg2Rad * maxSteerAngle * _horizontalInput);
        float insideWheelAngle = Mathf.Atan(wheelbase / (turnRadius - trackWidth / 2)) * Mathf.Rad2Deg;
        float outsideWheelAngle = Mathf.Atan(wheelbase / (turnRadius + trackWidth / 2)) * Mathf.Rad2Deg;

        frontLeftWheelCollider.steerAngle = insideWheelAngle;
        frontRightWheelCollider.steerAngle = outsideWheelAngle;

        _currentSteerAngle = Mathf.Clamp(maxSteerAngle * _horizontalInput, -maxSteerAngle, maxSteerAngle);


        //_currentSteerAngle = maxSteerAngle * _horizontalInput;
        //frontLeftWheelCollider.steerAngle = _currentSteerAngle;
        //frontRightWheelCollider.steerAngle = _currentSteerAngle;
    }

    // Apply braking force when the spacebar is pressed
    private void HandleBraking()
    {
        _currentBrakeForce = _isHandBraking ? brakeForce : 0f;
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
        if (_isHandBraking && rigidbody.velocity.magnitude > 20f)
        {
            rigidbody.velocity = rigidbody.velocity.normalized * 20f;
        }
    }

    // Update the pitch of the engine sound based on the car's speed
    private void UpdateEngineSound()
    {
        float speed = _rigidbody.velocity.magnitude;
        engineAudioSource.pitch = Mathf.Lerp(minPitch, maxPitch, _verticalInput);

    }

    private void AddDownForce()
    {
        _rigidbody.AddForce(-transform.up * downForce * _rigidbody.velocity.magnitude);
        _rigidbody.centerOfMass = _centerOfMass.transform.position;
    }
}