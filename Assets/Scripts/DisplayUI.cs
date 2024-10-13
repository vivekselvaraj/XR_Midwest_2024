using TMPro;
using UnityEngine;

public class DisplayDebugUpdater : MonoBehaviour
{
    public UIManager uiManager;  // Reference to the UIManager to call getPostureUpdate()
    public TextMeshProUGUI debugText;  // Reference to the Text component (child) to update the debug string

    void Start()
    {
        // Ensure the debugText reference is assigned (in the Inspector or programmatically)
        if (debugText == null)
        {
            Debug.LogError("Debug Text component is not assigned.");
        }
    }

    void Update()
    {
        if (uiManager != null && debugText != null)
        {
            // Get the posture update string from UIManager and update the text component
            string postureUpdate = uiManager.getPostureUpdate();
            debugText.text = postureUpdate;
        }
    }
}