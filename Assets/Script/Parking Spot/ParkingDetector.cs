using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ParkingDetector : MonoBehaviour
{
    private bool isParked = false;
    private Transform currentParkingSpot;

    [SerializeField] private Transform parkingSpot1;
    [SerializeField] private Transform parkingSpot2;
    [SerializeField] private Transform parkingSpot3;

    [SerializeField] private float positionTolerance = 1.5f;
    [SerializeField] private float angleTolerance = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ParkingSpot"))
        {
            SetCurrentParkingSpot(other.transform);
            isParked = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ParkingSpot"))
        {
            Debug.Log("Car has left the parking spot.");
            isParked = false;
            currentParkingSpot = null;
        }
    }

    private void Update()
    {
        if (isParked && currentParkingSpot != null)
        {
            if (CarIsStopped() && CarIsProperlyAligned())
            {
                Debug.Log("Level Passed! Car is properly parked.");
                StartCoroutine(WaitForParkingCompletion());
            }
        }
    }

    private IEnumerator WaitForParkingCompletion()
    {
        isParked = false;
        yield return new WaitForSeconds(3);
        Debug.Log("Level Passed! Car is properly parked.");
        LevelComplete();
    }

    private bool CarIsStopped()
    {
        Rigidbody carRigidbody = GetComponent<Rigidbody>();
        return carRigidbody.velocity.magnitude < 0.001f;
    }

    private bool CarIsProperlyAligned()
    {
        if (currentParkingSpot == null) return false;

        float distanceToSpot = Vector3.Distance(transform.position, currentParkingSpot.position);
        bool isPositionAligned = distanceToSpot <= positionTolerance;

        float angleDifference = Vector3.Angle(transform.forward, currentParkingSpot.forward);
        bool isRotationAligned = angleDifference <= angleTolerance;

        Debug.Log($"Position aligned: {isPositionAligned}, Rotation aligned: {isRotationAligned}");
        return isPositionAligned && isRotationAligned;
    }

    private void SetCurrentParkingSpot(Transform parkingSpot)
    {
        if (parkingSpot == parkingSpot1)
        {
            currentParkingSpot = parkingSpot1;
            Debug.Log("Car has entered Parking Spot 1.");
        }
        else if (parkingSpot == parkingSpot2)
        {
            currentParkingSpot = parkingSpot2;
            Debug.Log("Car has entered Parking Spot 2.");
        }
        else if (parkingSpot == parkingSpot3)
        {
            currentParkingSpot = parkingSpot3;
            Debug.Log("Car has entered Parking Spot 3.");
        }
    }

    private void LevelComplete()
    {
        Debug.Log("Proceeding to the next level...");
        SceneManager.LoadScene("Level 2");
    }
}
