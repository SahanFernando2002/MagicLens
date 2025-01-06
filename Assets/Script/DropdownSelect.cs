using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DropdownSelect : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    public void OnConfirmButtonClick()
    {
        int selectedLocation = dropdown.value;
        switch (selectedLocation)
        {
            case 0:
                SceneManager.LoadScene("Entrance");
                break;
            case 1:
                SceneManager.LoadScene("LectureRoom101");
                break;
            case 2:
                SceneManager.LoadScene("LectureRoom103");
                break;
            case 3:
                SceneManager.LoadScene("StudyArea");
                break;
            case 4:
                SceneManager.LoadScene("StaffOffice1");
                break;
            case 5:
                SceneManager.LoadScene("StaffRoom2");
                break;
    
            default:
                Debug.LogError("Invalid location selection.");
                break;
        }
    }
}
