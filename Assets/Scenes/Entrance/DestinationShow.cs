using UnityEngine;
using TMPro;

public class DestinationShow : MonoBehaviour
{
    [Header("Destination Objects")]
    public GameObject LecRoom101;
    public GameObject LecRoom103;
    public GameObject StudyArea;
    public GameObject StaffOffice1;
    public GameObject StaffRoom2;

    [Header("Marker Prefab")]
    public GameObject markerPrefab; // Red location icon with a TextMeshPro label

    private GameObject currentMarker;
    private Transform[] destinations;
    private string[] destinationNames = { "Lecture Room 101", "Lecture Room 103", "Study Area", "Staff Office 1", "Staff Room 2" };
    private int currentIndex = 0;

    private void Start()
    {
        // Store destinations in an array for easy cycling
        destinations = new Transform[]
        {
            LecRoom101.transform,
            LecRoom103.transform,
            StudyArea.transform,
            StaffOffice1.transform,
            StaffRoom2.transform
        };

        // Instantiate the marker but hide it initially
        currentMarker = Instantiate(markerPrefab);
        currentMarker.SetActive(false);

        // Show the initial marker
        UpdateMarker();
    }

    public void CycleLeft()
    {
        currentIndex = (currentIndex - 1 + destinations.Length) % destinations.Length;
        UpdateMarker();
    }

    public void CycleRight()
    {
        currentIndex = (currentIndex + 1) % destinations.Length;
        UpdateMarker();
    }

    private void UpdateMarker()
    {
        // Show the marker at the current destination
        currentMarker.SetActive(true);
        currentMarker.transform.position = destinations[currentIndex].position + Vector3.up * 0.5f; // Adjust height above NavMesh

        // Update the text label
        TMP_Text label = currentMarker.GetComponentInChildren<TMP_Text>();
        if (label != null)
        {
            label.text = destinationNames[currentIndex];
        }
    }
}
