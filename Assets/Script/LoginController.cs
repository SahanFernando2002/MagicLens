using UnityEngine;
using TMPro; 
using Firebase.Auth;
using UnityEngine.SceneManagement;
using Firebase.Extensions;
using UnityEngine.UI;
using Firebase;

public class LoginController : MonoBehaviour
{
    public TMP_InputField emailInput;  
    public TMP_InputField passwordInput;
    public Button loginButton;

    private FirebaseAuth auth;

    // Pop-up Panel
    public GameObject alertPanel;  
    public TMP_Text alertText;  

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        loginButton.onClick.AddListener(() => AttemptLogin());
        alertPanel.SetActive(false); // Hide the pop-up at start
    }

    void AttemptLogin()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowAlert("Email and password cannot be empty.");
            return;
        }

        if (!IsValidEmail(email))
        {
            ShowAlert("Invalid email format.");
            return;
        }

        Login(email, password);
    }

    void Login(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                ShowAlert(GetFirebaseErrorMessage(task.Exception));
                return;
            }

            FirebaseUser user = task.Result.User;
            Debug.Log("Login Successful: " + user.Email);
            SceneManager.LoadScene("Currentlocation");
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
        return "Unknown error occurred.";
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
