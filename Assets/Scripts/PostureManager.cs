using UnityEngine;
using System.Collections;

public class PostureManager : MonoBehaviour
{
    public GameObject headRig;
    public DisplaySpawner displaySpawner;  // Reference to DisplaySpawner
    public UIManager uiManager;

    private Vector3 currentHeadPosition;
    private Quaternion currentHeadRotation;
    private Vector3 optimalPosition;
    private Quaternion optimalRotation;

    // Thresholds for detecting deviations
    public float forwardHeadThreshold = 0.075f;  // 2-3 inches forward slouching (0.05 - 0.075 meters)
    public float verticalTiltThreshold = 20.0f;  // Degrees, for head tilt (up/down)
    public float lateralTiltThreshold = 15.0f;   // Degrees, for head side tilts (left/right)
    
    private bool isCalibrated = false;
    private string currentPostureStatus = "Press C to Calibrate Posture";  // Default to good posture

    void Update()
    {
        // Debug key press to trigger calibration
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(CalibrateAfterDelay(3f));  // Start the coroutine with 3 seconds delay for debug
        }
        
        // Analyze posture in real-time
        if (isCalibrated)
        {
            Vector3 headPosition = headRig.transform.localPosition;
            Quaternion headRotation = headRig.transform.localRotation;
            AnalyzePosture(headPosition, headRotation);
        }
    }

    // Method to be called by the UI button
    public void TriggerCalibration()
    {
        StartCoroutine(CalibrateAfterDelay(3f));  // Start the calibration coroutine with 3 seconds delay
    }

    // Coroutine to wait before calibrating
    private IEnumerator CalibrateAfterDelay(float waitTime)
    {
        Debug.Log("Preparing to calibrate. Please adjust posture...");
        uiManager.ShowDialog("Preparing to calibrate. Please adjust posture...", 3.0f);
        
        yield return new WaitForSeconds(waitTime);  // Wait for the specified time

        // Now calibrate the posture
        CalibratePosture();
        
        Debug.Log("Posture calibrated.");
        uiManager.ShowDialog("Posture calibrated.", 3.0f);

        // Trigger the display spawner to place the displays
        if (displaySpawner != null)
        {
            displaySpawner.PlaceDisplays();
        }
        else
        {
            Debug.LogError("DisplaySpawner reference is missing.");
        }
    }

    // Calibrate the posture based on the current head position and rotation from HeadTrackingScript
    public void CalibratePosture()
    {
        if (headRig != null)
        {
            // Use the current head position and rotation from the HeadTrackingScript
            optimalPosition = headRig.transform.localPosition;
            optimalRotation = headRig.transform.localRotation;
            isCalibrated = true;
            Debug.Log("Optimal posture calibrated: " + optimalPosition + ", " + optimalRotation);
        }
        else
        {
            Debug.LogError("HeadRig reference is missing.");
        }
    }

    // Analyze posture based on head data and update posture status
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
        }
        else if (positionDelta.z < -forwardHeadThreshold)
        {
            currentPostureStatus = "Leaning back detected";
        }
        else
        {
            // Calculate rotation deviation
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
                currentPostureStatus = "Good posture";
            }
        }
    }

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
