using UnityEngine;
using System.Collections;

public class DisplaySpawner : MonoBehaviour
{
    public GameObject displaysPrefab;  // Prefab containing the grouped displays (center, left, right)
    public PostureManager postureManager;  // Reference to the PostureManager

    public float viewingDistance = 0.75f;  // Distance from user (75 cm)
    public float verticalGazeAngleDegrees = 15.0f;  // Downward gaze angle for ergonomics
    public float yIncrement = 0.01f;  // Small movement increment (1 cm)
    public float maxYOffset = 0.3f;   // Maximum allowed upward movement (30 cm)

    private Vector3 optimalPosition;  // Optimal position from PostureManager
    private GameObject currentDisplays;  // Reference to the current displays instance
    private bool isReverting = false;  // Track if the display is currently reverting

    void Update()
    {
        // Calibrate and place the displays when 'C' is pressed and posture is calibrated
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
            optimalPosition = postureManager.GetOptimalPosition();
            Quaternion optimalRotation = postureManager.GetOptimalRotation();

            Vector3 displaysPosition = CalculateDisplayPosition(optimalPosition, optimalRotation);

            // Instantiate the displays parent object at the calculated position
            currentDisplays = Instantiate(displaysPrefab, displaysPosition, optimalRotation);
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

    // Adjust display by moving it slightly upwards when slouching is detected
    public void AdjustDisplayForSlouching()
    {
        if (currentDisplays != null && !isReverting)
        {
            Vector3 currentPosition = currentDisplays.transform.position;

            // Check if we have not exceeded the maximum upward offset
            if (currentPosition.y < optimalPosition.y + maxYOffset)
            {
                // Move the display upward by a small increment
                Vector3 adjustedPosition = currentPosition + new Vector3(0, yIncrement, 0);
                currentDisplays.transform.position = adjustedPosition;

                Debug.Log("Display moved up due to slouching.");
            }
        }
    }

    // Coroutine to gradually revert the display to the optimal position
    public IEnumerator RevertDisplayToOptimalPosition()
    {
        if (currentDisplays != null && !isReverting)
        {
            isReverting = true;  // Mark as reverting to prevent conflicting adjustments

            Vector3 currentPosition = currentDisplays.transform.position;

            // Gradually move the display back to the original optimal position
            while (currentPosition.y > optimalPosition.y)
            {
                currentPosition = currentDisplays.transform.position;

                // Move the display downward by a small increment
                Vector3 adjustedPosition = currentPosition - new Vector3(0, yIncrement, 0);
                currentDisplays.transform.position = adjustedPosition;

                // Check if we are close enough to the original position to stop adjusting
                if (currentPosition.y > optimalPosition.y)
                {
                    currentDisplays.transform.position = optimalPosition;
                    break;
                }

                // Wait for the next frame
                yield return null;
            }

            isReverting = false;  // Reversion complete
            Debug.Log("Display reverted to optimal position.");
        }
    }
}
