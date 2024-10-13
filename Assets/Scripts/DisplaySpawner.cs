using UnityEngine;

public class DisplaySpawner : MonoBehaviour
{
    public GameObject displaysPrefab;  // Prefab containing the grouped displays (center, left, right)
    public PostureManager postureManager;  // Reference to the PostureManager

    public float viewingDistance = 0.75f;  // Distance from user (75 cm)
    public float verticalGazeAngleDegrees = 15.0f;  // Downward gaze angle for ergonomics

    private GameObject currentDisplays;  // Reference to the current displays instance

    void Update()
    {
        // Calibrate and place the displays when 'C' is pressed
        if (Input.GetKeyDown(KeyCode.C) && postureManager.getIsCalibrated())
        {
            PlaceDisplays();
        }
    }

    // Place the 'displays' parent object based on the user's calibrated posture
    public void PlaceDisplays()
    {
        // Destroy existing displays if they exist
        if (currentDisplays != null)
        {
            Destroy(currentDisplays);
        }

        if (postureManager != null && postureManager.GetOptimalPosition() != Vector3.zero)
        {
            Vector3 optimalPosition = postureManager.GetOptimalPosition();
            Quaternion optimalRotation = postureManager.GetOptimalRotation();

            Vector3 displaysPosition = CalculateDisplayPosition(optimalPosition, optimalRotation);

            // Instantiate the displays parent object at the calculated position
            currentDisplays = Instantiate(displaysPrefab, displaysPosition, optimalRotation);

            // Activate the displays
            currentDisplays.SetActive(true);

            Debug.Log("Displays placed at ergonomic position.");
        }
        else
        {
            Debug.LogError("PostureManager reference or calibration is missing. Cannot place displays.");
        }
    }

    // Calculate the display group's position based on the user's good posture
    private Vector3 CalculateDisplayPosition(Vector3 optimalPosition, Quaternion optimalRotation)
    {
        float gazeAngleRadians = Mathf.Deg2Rad * verticalGazeAngleDegrees;
        float verticalOffset = Mathf.Tan(gazeAngleRadians) * viewingDistance;

        Vector3 displayPosition = optimalPosition + (optimalRotation * Vector3.forward) * viewingDistance;
        displayPosition -= new Vector3(0, verticalOffset, 0);

        return displayPosition;
    }
}
