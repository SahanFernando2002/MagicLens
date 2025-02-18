using UnityEngine;

public class DestinationMarker : MonoBehaviour
{
    [Header("Marker Animation")]
    public float spinSpeed = 50f;
    public float enlargeScale = 1.5f;
    public float animationSpeed = 5f;

    private Vector3 originalScale;
    private bool isSelected = false;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (isSelected)
        {
            // Smoothly enlarge and spin the selected marker
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale * enlargeScale, Time.deltaTime * animationSpeed);
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
        }
        else
        {
            // Smoothly return to original size if deselected
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * animationSpeed);
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
    }
}
