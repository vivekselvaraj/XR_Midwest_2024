using System.Collections;
using UnityEngine;

public class HeadTrackingScript : MonoBehaviour
{
    // Reference to head rig (VR headset or camera rig)
    public Transform headRig;

    // Event for sending head position and rotation data to PostureManager
    public delegate void HeadDataUpdated(Vector3 position, Quaternion rotation);
    public static event HeadDataUpdated OnHeadDataUpdated;

    // Polling interval (in seconds)
    public float pollingInterval = 0.1f;

    void Start()
    {
        // Start the coroutine to poll head tracking data every pollingInterval
        StartCoroutine(PollHeadTrackingData());
    }

    // Coroutine to poll head tracking data at regular intervals
    private IEnumerator PollHeadTrackingData()
    {
        while (true)
        {
            // Capture head position and rotation
            Vector3 headPosition = headRig.localPosition;
            Quaternion headRotation = headRig.localRotation;

            // Trigger event to send data to PostureManager
            if (OnHeadDataUpdated != null)
            {
                OnHeadDataUpdated(headPosition, headRotation);
            }

            // Wait for the next polling interval
            yield return new WaitForSeconds(pollingInterval);
        }
    }
}