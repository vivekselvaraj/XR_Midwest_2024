using TMPro;
using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public PostureManager postureManager;  // Reference to the PostureManager
    public GameObject dialogParent;  // Parent GameObject for the dialog UI
    public TextMeshProUGUI dialogText;  // Reference to the TextMeshPro for dialog text

    public Vector3 dialogOffset = new Vector3(0, 0, 0.4f);  // Offset to position dialog in front of the user

    private GameObject activeDialog;  // Active dialog instance

    void OnEnable()
    {
        // Subscribe to head tracking event from the HeadTrackingScript
        HeadTrackingScript.OnHeadDataUpdated += UpdateDialogPosition;
    }

    void OnDisable()
    {
        // Unsubscribe from head tracking event to avoid errors
        HeadTrackingScript.OnHeadDataUpdated -= UpdateDialogPosition;
    }

    // Public method to update posture information
    public string getPostureUpdate()
    {
        Vector3 headPosition = postureManager.GetCurrentHeadPosition();
        Quaternion headRotation = postureManager.GetCurrentHeadRotation();
        string postureStatus = postureManager.GetCurrentPostureStatus();

        string debugInfo = $"Head Position: {headPosition}\n" +
                           $"Head Rotation: {headRotation.eulerAngles}\n" +
                           $"Posture Status: {postureStatus}";

        return debugInfo;
    }
    
    public void ShowDialog(string dialogContent, float displayDuration = 0f)
    {
        // If an active dialog exists, simply update its content and show it
        if (activeDialog != null)
        {
            activeDialog.SetActive(true);  // Show the dialog instead of destroying it

            // Update the dialog text content
            if (dialogText != null)
            {
                dialogText.text = dialogContent;
            }
        }
        else
        {
            // No active dialog exists, create a new one
            CreateNewDialog(dialogContent);
        }

        // Ensure the dialog is positioned correctly when displayed
        Vector3 headPosition = postureManager.GetCurrentHeadPosition();
        Quaternion headRotation = postureManager.GetCurrentHeadRotation();
        UpdateDialogPosition(headPosition, headRotation);

        // If a display duration is provided, start a coroutine to hide the dialog after the duration
        if (displayDuration > 0)
        {
            StartCoroutine(HideDialogAfterTime(displayDuration));
        }
    }

    // Coroutine to hide the dialog after a certain time
    private IEnumerator HideDialogAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        if (activeDialog != null)
        {
            activeDialog.SetActive(false);  // Hide the dialog instead of destroying it
        }
    }

    // Helper method to create and display a new dialog
    private void CreateNewDialog(string dialogContent)
    {
        if (dialogParent != null)
        {
            // Activate the dialog UI and set it to active
            activeDialog = dialogParent;
            activeDialog.SetActive(true);

            // Set the dialog content
            if (dialogText != null)
            {
                dialogText.text = dialogContent;
            }
        }
        else
        {
            Debug.LogError("Dialog parent reference is missing.");
        }
    }

    // Method to update the dialog position based on head position and rotation (called only once when the dialog is created)
    private void UpdateDialogPosition(Vector3 headPosition, Quaternion headRotation)
    {
        if (activeDialog != null)
        {
            // Calculate new dialog position based on the user's head position and rotation
            Vector3 newDialogPosition = headPosition + headRotation * dialogOffset;

            // Update the dialog's position and rotation to stay in place
            activeDialog.transform.position = newDialogPosition;
            activeDialog.transform.rotation = Quaternion.LookRotation(newDialogPosition - headPosition);
        }
    }
}
