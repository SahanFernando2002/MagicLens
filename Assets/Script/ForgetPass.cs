using UnityEngine;
using TMPro;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Extensions;

public class ForgotPass : MonoBehaviour
{
    public TMP_InputField emailInput;  
    public TMP_Text feedbackText;  
    public Button resetPasswordButton;  

    private FirebaseAuth auth;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        resetPasswordButton.onClick.AddListener(() => SendPasswordResetEmail(emailInput.text));
    }

    void SendPasswordResetEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            feedbackText.text = "Please enter your email.";
            return;
        }

        auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                feedbackText.text = "Failed to send reset email: " + task.Exception?.GetBaseException().Message;
                Debug.LogError("Error sending reset email: " + task.Exception?.ToString());
                return;
            }

            feedbackText.text = "Password reset email sent! Please check your inbox.";
            Debug.Log("Password reset email sent to: " + email);

            // Redirect to login scene after 2 seconds
            Invoke("GoToLoginScene", 2f);
        });
    }

    void GoToLoginScene()
    {
        SceneManager.LoadScene("Login");
    }
}
