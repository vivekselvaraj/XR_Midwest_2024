using UnityEngine;
using System.Collections;

public class PostureManager : MonoBehaviour
{
    public GameObject headRig;
    public DisplaySpawner displaySpawner;  // Reference to DisplaySpawner
    public UIManager uiManager;  // For showing posture feedback to the user

    private Vector3 currentHeadPosition;
    private Quaternion currentHeadRotation;
    private Vector3 optimalPosition;
    private Quaternion optimalRotation;

    // Thresholds for detecting posture deviations
    public float forwardHeadThreshold = 0.075f;  // 2-3 inches forward slouching (0.05 - 0.075 meters)
    public float verticalTiltThreshold = 20.0f;  // Degrees for head tilt (up/down)
    public float lateralTiltThreshold = 15.0f;   // Degrees for head tilt (left/right)

    // For controlling the timing of display adjustments
    private float lastAdjustmentTime = 0f;
    public float adjustmentInterval = 0.5f;  // Adjust every 0.5 seconds

    private bool isCalibrated = false;  // Whether the posture has been calibrated
    private string currentPostureStatus = "Press 'C' to Calibrate Posture";  // Default to good posture

    // Update method to handle real-time posture analysis
    void Update()
    {
        // Start calibration with 'C' key
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(CalibrateAfterDelay(3f));  // Start calibration after 3 seconds for user adjustment
        }

        // Analyze posture if calibrated
        if (isCalibrated)
        {
            Vector3 headPosition = headRig.transform.localPosition;
            Quaternion headRotation = headRig.transform.localRotation;
            AnalyzePosture(headPosition, headRotation);
        }
    }

    // Method to trigger calibration via UI
    public void TriggerCalibration()
    {
        StartCoroutine(CalibrateAfterDelay(3f));  // Start calibration after a 3-second delay
    }

    // Coroutine to wait for a certain time before calibrating the posture
    private IEnumerator CalibrateAfterDelay(float waitTime)
    {
        Debug.Log("Preparing to calibrate. Please adjust posture...");
        uiManager.ShowDialog("Preparing to calibrate. Please adjust posture...", waitTime);

        yield return new WaitForSeconds(waitTime);  // Wait for the user to adjust posture

        // Perform the calibration
        CalibratePosture();
        Debug.Log("Posture calibrated.");
        uiManager.ShowDialog("Posture calibrated.", 3.0f);

        // Trigger display placement after calibration
        if (displaySpawner != null)
        {
            displaySpawner.PlaceDisplays();  // Place displays based on the newly calibrated posture
        }
        else
        {
            Debug.LogError("DisplaySpawner reference is missing.");
        }
    }

    // Calibrate the posture based on the current head position and rotation
    public void CalibratePosture()
    {
        if (headRig != null)
        {
            optimalPosition = headRig.transform.localPosition;  // Set optimal position based on current head position
            optimalRotation = headRig.transform.localRotation;  // Set optimal rotation based on current head rotation
            isCalibrated = true;
            Debug.Log("Optimal posture calibrated: " + optimalPosition + ", " + optimalRotation);
        }
        else
        {
            Debug.LogError("HeadRig reference is missing.");
        }
    }

    // Analyze the user's posture based on head position and rotation, updating posture status
    private void AnalyzePosture(Vector3 headPosition, Quaternion headRotation)
    {
        if (!isCalibrated) return;

        currentHeadPosition = headPosition;
        currentHeadRotation = headRotation;

        // Calculate position deviation
        Vector3 positionDelta = headPosition - optimalPosition;

        // Detect slouching (forward head movement)
        if (positionDelta.z > forwardHeadThreshold)
        {
            currentPostureStatus = "Slouching detected";

            if (Time.time > lastAdjustmentTime + adjustmentInterval)
            {
                displaySpawner.AdjustDisplayForSlouching();  // Trigger display adjustment when slouching is detected
                lastAdjustmentTime = Time.time;
            }
        }
        else if (positionDelta.z < -forwardHeadThreshold)
        {
            currentPostureStatus = "Leaning back detected";
        }
        else
        {
            // Calculate rotation deviation for head tilts
            Quaternion rotationDelta = Quaternion.Inverse(optimalRotation) * headRotation;
            rotationDelta.ToAngleAxis(out float angle, out Vector3 axis);

            // Detect vertical head tilt (up/down)
            if (axis.x != 0 && Mathf.Abs(angle) > verticalTiltThreshold)
            {
                if (axis.x > 0) // Head tilting downward
                {
                    currentPostureStatus = "Tilting head down";
                }
                else if (axis.x < 0) // Head tilting upward
                {
                    currentPostureStatus = "Tilting head up";
                }
            }
            // Detect lateral tilt (sideways tilt)
            else if (axis.z != 0 && Mathf.Abs(angle) > lateralTiltThreshold)
            {
                if (axis.z > 0) // Head tilting right
                {
                    currentPostureStatus = "Leaning to the right";
                }
                else if (axis.z < 0) // Head tilting left
                {
                    currentPostureStatus = "Leaning to the left";
                }
            }
            else
            {
                // Good posture detected, revert display if previously adjusted
                currentPostureStatus = "Good posture";

                // if (Time.time > lastAdjustmentTime + adjustmentInterval)
                // {
                //     StartCoroutine(displaySpawner.RevertDisplayToOptimalPosition());  // Gradual reversion of the display
                //     lastAdjustmentTime = Time.time;
                // }
            }
        }
    }

    // Getters for current and optimal positions and rotations
    public Vector3 GetCurrentHeadPosition()
    {
        return currentHeadPosition;
    }

    public Quaternion GetCurrentHeadRotation()
    {
        return currentHeadRotation;
    }

    public string GetCurrentPostureStatus()
    {
        return currentPostureStatus;
    }

    public Vector3 GetOptimalPosition()
    {
        return optimalPosition;
    }

    public Quaternion GetOptimalRotation()
    {
        return optimalRotation;
    }

    public bool getIsCalibrated()
    {
        return isCalibrated;
    }
}
