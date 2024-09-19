using TMPro;
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

    // Speedometer text UI element
    [SerializeField] private TextMeshProUGUI speedometerText;

    // Score management
    [SerializeField] private int playerScore = 100;

    // Star images in the win menu
    [SerializeField] private GameObject star1;
    [SerializeField] private GameObject star2;
    [SerializeField] private GameObject star3;

    // Driver comments
    [SerializeField] private GameObject BestDriver;
    [SerializeField] private GameObject NormalDriver;
    [SerializeField] private GameObject BadDriver;

    private Rigidbody _rigidbody;
    [SerializeField] private GameObject _centerOfMass;
    public bool EngineOff = true;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_centerOfMass != null)
        {
            _rigidbody.centerOfMass = _centerOfMass.transform.localPosition;
        }
    }

    private void FixedUpdate()
    {
        GetInput();
        LimitSpeed();
        if (EngineOff)
        {
            HandleMotor();
        }
        HandleSteering();
        HandleBraking();
        UpdateWheels();
        LimitSpeedDuringBraking();
        AnitiRoll();
        UpdateSpeedometer();
    }

    private void UpdateSpeedometer()
    {
        float speedInKmh = _rigidbody.velocity.magnitude * 3.6f;

        speedInKmh = Mathf.Clamp(speedInKmh, 0, 150);

        speedometerText.text = Mathf.RoundToInt(speedInKmh).ToString() + " km/h";
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
        //rearLeftWheelCollider.motorTorque = motorTorque;
        //rearRightWheelCollider.motorTorque = motorTorque;
        frontLeftWheelCollider.motorTorque = motorTorque;
        frontRightWheelCollider.motorTorque = motorTorque;
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


    private void AnitiRoll()
    {
        WheelHit hit;
        float travelLeft = 1.0f;
        float travelRight = 1.0f;

        if (frontLeftWheelCollider.GetGroundHit(out hit))
        {
            travelLeft = (-frontLeftWheelCollider.transform.InverseTransformPoint(hit.point).y - frontLeftWheelCollider.radius) / frontLeftWheelCollider.suspensionDistance;
        }

        if (frontRightWheelCollider.GetGroundHit(out hit))
        {
            travelRight = (-frontRightWheelCollider.transform.InverseTransformPoint(hit.point).y - frontRightWheelCollider.radius) / frontRightWheelCollider.suspensionDistance;
        }

        float antiRollforce = (travelLeft - travelRight) * 5000f;

        if (frontLeftWheelCollider.isGrounded)
        {
            _rigidbody.AddForceAtPosition(frontLeftWheelCollider.transform.up * -antiRollforce, frontLeftWheelCollider.transform.position);
        }

        if (frontRightWheelCollider.isGrounded)
        {
            _rigidbody.AddForceAtPosition(frontRightWheelCollider.transform.up * antiRollforce, frontRightWheelCollider.transform.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Terrains")
        {
            DeductPoints();
        }
    }

    public void DeductPoints()
    {
        playerScore -= 10;
        playerScore = Mathf.Max(0, playerScore); // score doesn't go below 0
        ShowStarsBasedOnScore();
    }

    public void ShowStarsBasedOnScore()
    {
        star1.SetActive(false);
        star2.SetActive(false);
        star3.SetActive(false);

        BestDriver.SetActive(false);
        NormalDriver.SetActive(false);
        BadDriver.SetActive(false);

        if (playerScore == 100)
        {
            Debug.Log("Best Driver Message Activated");
            star1.SetActive(true);
            star2.SetActive(true);
            star3.SetActive(true);

            BestDriver.SetActive(true);
            NormalDriver.SetActive(false);
            BadDriver.SetActive(false);

        }
        else if (playerScore >= 50 && playerScore < 100)
        {
            Debug.Log("Normal Driver Message Activated");
            star1.SetActive(true);
            star2.SetActive(true);

            BestDriver.SetActive(false);
            NormalDriver.SetActive(true);
            BadDriver.SetActive(false);
        }
        else if (playerScore < 50)
        {
            Debug.Log("Bad Driver Message Activated");
            star1.SetActive(true);

            BestDriver.SetActive(false);
            NormalDriver.SetActive(false);
            BadDriver.SetActive(true);
        }
    }
}