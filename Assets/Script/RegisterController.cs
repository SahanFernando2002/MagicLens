using UnityEngine;
using TMPro;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using Firebase.Extensions;
using UnityEngine.UI;
using Firebase;

public class RegisterController : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public Button registerButton;

    private FirebaseAuth auth;

    // Pop-up Panel
    public GameObject alertPanel;
    public TMP_Text alertText;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        registerButton.onClick.AddListener(() => AttemptRegister());
        alertPanel.SetActive(false); // Ensure the alert panel starts hidden
    }

    void AttemptRegister()
    {
        string name = nameInput.text.Trim();
        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();
        string confirmPassword = confirmPasswordInput.text.Trim();

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            ShowAlert("Please fill in all fields.");
            return;
        }

        if (!IsValidEmail(email))
        {
            ShowAlert("Invalid email format.");
            return;
        }

        if (password.Length < 6)
        {
            ShowAlert("Password must be at least 6 characters long.");
            return;
        }

        if (password != confirmPassword)
        {
            ShowAlert("Passwords do not match.");
            return;
        }

        RegisterUser(name, email, password);
    }

    void RegisterUser(string name, string email, string password)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                ShowAlert(GetFirebaseErrorMessage(task.Exception));
                return;
            }

            FirebaseUser newUser = task.Result.User;

            // Update user's display name
            UserProfile profile = new UserProfile { DisplayName = name };
            newUser.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(updateTask =>
            {
                if (updateTask.IsFaulted || updateTask.IsCanceled)
                {
                    ShowAlert("Failed to update profile.");
                }
                else
                {
                    ShowAlert("Registration successful! Redirecting...");
                    Debug.Log("User registered: " + name);
                    SceneManager.LoadScene("Currentlocation");
                }
            });
        });
    }

    bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    string GetFirebaseErrorMessage(System.AggregateException exception)
    {
        foreach (var e in exception.InnerExceptions)
        {
            if (e is FirebaseException firebaseEx)
            {
                return firebaseEx.Message;
            }
        }
        return "An unknown error occurred.";
    }

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
