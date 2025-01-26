using UnityEngine;
using TMPro;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using Firebase.Extensions;
using UnityEngine.UI;

public class RegisterController : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public TMP_Text feedbackText;
    public Button registerButton;

    private FirebaseAuth auth;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        registerButton.onClick.AddListener(() => RegisterUser());
    }

    void RegisterUser()
    {
        string name = nameInput.text;
        string email = emailInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            feedbackText.text = "Please fill all fields.";
            return;
        }

        if (password != confirmPassword)
        {
            feedbackText.text = "Passwords do not match.";
            return;
        }

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                feedbackText.text = "Registration Failed: " + task.Exception?.GetBaseException().Message;
                Debug.LogError("Registration failed: " + task.Exception?.ToString());
                return;
            }

            FirebaseUser newUser = task.Result.User;

            // Update the user's display name
            UserProfile profile = new UserProfile { DisplayName = name };
            newUser.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(updateTask =>
            {
                if (updateTask.IsFaulted || updateTask.IsCanceled)
                {
                    feedbackText.text = "Failed to update profile.";
                    Debug.LogError("Profile update failed: " + updateTask.Exception?.ToString());
                }
                else
                {
                    feedbackText.text = "Registration Successful! Logging in...";
                    Debug.Log("User registered with name: " + name);
                    SceneManager.LoadScene("Currentlocation");
                }
            });
        });
    }
}
