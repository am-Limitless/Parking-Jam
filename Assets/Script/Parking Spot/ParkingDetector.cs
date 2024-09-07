using System.Collections;
using UnityEngine;

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
            if (other.transform == parkingSpot1)
            {
                currentParkingSpot = parkingSpot1;
                Debug.Log("Car has entered Parking Spot 1.");
            }
            else if (other.transform == parkingSpot2)
            {
                currentParkingSpot = parkingSpot2;
                Debug.Log("Car has entered Parking Spot 2.");
            }
            else if (other.transform == parkingSpot3)
            {
                currentParkingSpot = parkingSpot3;
                Debug.Log("Car has entered Parking Spot 3.");
            }
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
            if (CarIsStopped() && CarIsProperlyAligned()) ;
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
        Rigidbody carRigibody = GetComponent<Rigidbody>();
        return carRigibody.velocity.magnitude < 0.001f;
    }

    private bool CarIsProperlyAligned()
    {
        if (currentParkingSpot == null)
        {
            return false;
        }

        float distanceToSpot = Vector3.Distance(transform.position, currentParkingSpot.position);
        bool isPositionAligned = distanceToSpot <= positionTolerance;

        float angleDifference = Vector3.Angle(transform.forward, currentParkingSpot.forward);
        bool isRotationAligned = angleDifference <= angleTolerance;

        Debug.Log($"Position aligned: {isPositionAligned}, Rotation aligned: {isRotationAligned}");

        return isPositionAligned && isRotationAligned;
    }

    private void LevelComplete()
    {
        Debug.Log("Proceesing to the next level...");
    }
}


