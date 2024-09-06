using System.Collections;
using UnityEngine;

public class ParkingDetector : MonoBehaviour
{
    private bool isParked = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ParkingSpot"))
        {
            isParked = true;
            Debug.Log("Car has entered the parking area.");
        }
    }

    private void Update()
    {
        if (isParked)
        {
            if (CarIsStopped())
            {
                Debug.Log("Level Passed! Car is properly parked.");
                WaitForParkingCompletion();
            }
        }
    }

    private IEnumerator WaitForParkingCompletion()
    {
        yield return new WaitForSeconds(4);

        LevelComplete();
    }

    private bool CarIsStopped()
    {
        Rigidbody carRigibody = GetComponent<Rigidbody>();
        return carRigibody.velocity.magnitude < 0.001f;
    }

    private void LevelComplete()
    {
        Debug.Log("Proceesing to the next level...");
    }
}


