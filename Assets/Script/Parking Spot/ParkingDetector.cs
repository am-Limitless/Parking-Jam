using System.Collections;
using UnityEngine;

public class ParkingDetector : MonoBehaviour
{
    private bool isParked = false;
    private Transform currentParkingSpot;

    [SerializeField] private Transform parkingSpot1;
    [SerializeField] private Transform parkingSpot2;

    [SerializeField] private float positionTolerance = 1.5f;
    [SerializeField] private float angleTolerance = 10f;

    public GameObject winPannel;
    [SerializeField] private GameObject GameUi;

    [SerializeField] private GameObject TimerUI;

    private LevelLoader levelLoader;

    public GameManager gameManager;

    public CarController carController;


    private void Start()
    {
        levelLoader = FindObjectOfType<LevelLoader>();
    }


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
        yield return new WaitForSeconds(2);
        GameUi.SetActive(false);
        carController.EngineOff = false;
        gameManager.levelPassed = true;
        TimerUI.SetActive(false);
        winPannel.SetActive(true);
        //Debug.Log("Level Passed! Car is properly parked.");
        //yield return new WaitForSeconds(3);
        //Debug.Log("Level Completed");
        //LevelComplete();
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
        bool isRotationAligned = angleDifference <= angleTolerance || Mathf.Abs(angleDifference - 180f) <= angleTolerance;

        //Debug.Log($"Position aligned: {isPositionAligned}, Rotation aligned: {isRotationAligned}");
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

    }

    //private void LevelComplete()
    //{
    //    Debug.Log("Proceeding to the next level...");

    //    int currrentSceneIndex = SceneManager.GetActiveScene().buildIndex;

    //    if (levelLoader != null)
    //    {
    //        levelLoader.LoadLevel(currrentSceneIndex + 1);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Levelloader is not assigned!");
    //    }

    //    //if (SceneManager.sceneCountInBuildSettings > currrentSceneIndex + 1)
    //    //{
    //    //    SceneManager.LoadScene(currrentSceneIndex + 1);
    //    //}
    //    //else
    //    //{
    //    //    Debug.Log("Congragualtion!");
    //    //    SceneManager.LoadScene(0);
    //    //}
    //}
}
