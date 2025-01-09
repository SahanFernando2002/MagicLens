using UnityEngine;
using TMPro;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Extensions;

public class ForgotPass : MonoBehaviour
{
    public TMP_InputField emailInput;  
    public Button resetPasswordButton;  

    // Pop-up Panel
    public GameObject alertPanel;
    public TMP_Text alertText;

    private FirebaseAuth auth;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        resetPasswordButton.onClick.AddListener(() => SendPasswordResetEmail(emailInput.text));

        alertPanel.SetActive(false); // Ensure the alert panel starts hidden
    }

    void SendPasswordResetEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            ShowAlert("Please enter your email.");
            return;
        }

        auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                ShowAlert("Failed to send reset email: " + task.Exception?.GetBaseException().Message);
                Debug.LogError("Error sending reset email: " + task.Exception?.ToString());
                return;
            }

            ShowAlert("Password reset email sent! Please check your inbox.");
            Debug.Log("Password reset email sent to: " + email);

            // Redirect to login scene after 2 seconds
            Invoke("GoToLoginScene", 2f);
        });
    }

    void GoToLoginScene()
    {
        SceneManager.LoadScene("Login");
    }

    // Show the alert message in the pop-up panel
    void ShowAlert(string message)
    {
        alertPanel.SetActive(true);
        alertText.text = message;

        CancelInvoke(nameof(HideAlert));
        Invoke(nameof(HideAlert), 3f); // Auto-hide after 3 seconds
    }

    void HideAlert()
    {
        alertPanel.SetActive(false);
    }
}
