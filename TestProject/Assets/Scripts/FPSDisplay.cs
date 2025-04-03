using TMPro;
using UnityEngine;
using UnityEngine.UI; // For displaying FPS on UI text

public class FPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI fpsText; // Reference to a UI Text element to display FPS
    private float deltaTime = 0.0f;

    void Update()
    {
        // Calculate time between frames
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

        // Calculate FPS
        float fps = 1.0f / deltaTime;

        // Display the FPS in the UI text element
        if (fpsText != null)
        {
            fpsText.text = "FPS: " + Mathf.Ceil(fps).ToString();
        }
    }
}