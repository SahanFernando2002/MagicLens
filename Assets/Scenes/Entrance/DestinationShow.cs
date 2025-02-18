using UnityEngine;
using TMPro;
using UnityEngine.UI;  // For RawImage
using System.Collections;

public class DestinationShow : MonoBehaviour
{
    public GameObject[] destinationMarkers;  // Reference to all destination markers
    public TMP_Dropdown destinationDropdown;  // Reference to the TMP Dropdown UI element

    // Manually assign RawImage and Text components for each destination
    public RawImage[] rawImages;           // Array of RawImages for the destination markers
    public TextMeshProUGUI[] texts;        // Array of TextMeshProUGUI for the destination markers

    // Animation durations
    public float scaleDuration = 0.5f;  // Duration for scaling
    public float rotationDuration = 2f; // Duration for rotation
    public float scaleFactor = 1.5f;    // Scale factor when selected

    public void OnSearchClicked()
    {
        // Get the selected option from the TMP Dropdown
        string selectedDestination = destinationDropdown.options[destinationDropdown.value].text;

        Debug.Log("Search Button Clicked! Selected destination: " + selectedDestination);

        // Print all marker names for debugging
        foreach (GameObject marker in destinationMarkers)
        {
            Debug.Log("Available marker: " + marker.name);
        }

        // Try to find the corresponding marker by name
        GameObject destinationMarker = System.Array.Find(destinationMarkers, marker => marker.name == selectedDestination);

        if (destinationMarker != null)
        {
            Debug.Log("Found marker for: " + selectedDestination);
            
            // Disable all markers first
            DisableAllMarkers();

            // Enable the selected marker
            destinationMarker.SetActive(true);

            // Find the corresponding RawImage and Text components manually
            int index = System.Array.IndexOf(destinationMarkers, destinationMarker);
            RawImage rawImage = rawImages[index];    // Manually assigned RawImage
            TextMeshProUGUI text = texts[index];      // Manually assigned TextMeshProUGUI

            // Perform manual animations on RawImage and Text components
            StartCoroutine(AnimateMarker(rawImage, text, destinationMarker));
        }
        else
        {
            Debug.LogWarning("No marker found for destination: " + selectedDestination);
        }
    }

    private void DisableAllMarkers()
    {
        foreach (GameObject marker in destinationMarkers)
        {
            marker.SetActive(false);
        }
    }

    private IEnumerator AnimateMarker(RawImage rawImage, TextMeshProUGUI text, GameObject marker)
    {
        // Scale animation for RawImage and Text
        Vector3 originalScale = marker.transform.localScale;
        Vector3 targetScale = originalScale * scaleFactor;  // Enlarge the marker by scaleFactor
        float elapsedTime = 0;

        // Smoothly scale the marker
        while (elapsedTime < scaleDuration)
        {
            marker.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        marker.transform.localScale = targetScale;

        // Rotate the marker
        elapsedTime = 0;
        float rotationSpeed = 360f;  // Rotate 360 degrees per second

        while (elapsedTime < rotationDuration)
        {
            marker.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Make sure the final rotation is applied
        marker.transform.Rotate(Vector3.up * rotationSpeed * (rotationDuration - elapsedTime));

        // Animation for RawImage (optional color change effect)
        Color originalColor = rawImage.color;
        Color targetColor = Color.green;  // Change to green when selected
        elapsedTime = 0;

        while (elapsedTime < scaleDuration)
        {
            rawImage.color = Color.Lerp(originalColor, targetColor, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rawImage.color = targetColor;

        // Animation for Text (optional color change effect)
        Color originalTextColor = text.color;
        Color targetTextColor = Color.red;  // Change text color to red when selected
        elapsedTime = 0;

        while (elapsedTime < scaleDuration)
        {
            text.color = Color.Lerp(originalTextColor, targetTextColor, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        text.color = targetTextColor;
    }
}
