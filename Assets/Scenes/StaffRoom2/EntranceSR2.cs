using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EntranceSR2 : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Dropdown destinationDropdown;
    public Button confirmButton;

    [Header("Destination Cubes")]
    public GameObject Entrance;
    public GameObject LecRoom101;
    public GameObject LecRoom103;
    public GameObject StudyArea;
    public GameObject StaffOffice1;

    private void Start()
    {
        // Ensure all cubes are disabled at start
        DisableAllCubes();

        // Add listener for the confirm button
        confirmButton.onClick.AddListener(ConfirmDestination);
    }

    private void ConfirmDestination()
    {
        DisableAllCubes();

        // Log the selected destination to check for typos or mismatches
        string selectedDestination = destinationDropdown.options[destinationDropdown.value].text;
        Debug.Log("Selected destination: " + selectedDestination);

        // Enable the correct cube based on selection
        switch (selectedDestination.Trim())
        {
            case "Entrance":
                Entrance.SetActive(true);
                break;
            case "Lecture Room 101":
                LecRoom101.SetActive(true);
                break;
            case "Lecture Room 103":
                LecRoom103.SetActive(true);
                break;
            case "Study Area":
                StudyArea.SetActive(true);
                break;
            case "Staff Office 1":
                StaffOffice1.SetActive(true);
                break;
            default:
                Debug.LogWarning("Unknown destination selected!");
                break;
        }
    }

    private void DisableAllCubes()
    {
        Entrance.SetActive(false);
        LecRoom101.SetActive(false);
        LecRoom103.SetActive(false);
        StudyArea.SetActive(false);
        StaffOffice1.SetActive(false);
    }
}
