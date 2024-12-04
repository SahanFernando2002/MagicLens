using UnityEngine;
using TMPro; 
using Firebase.Auth;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Firebase.Extensions;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    public TMP_InputField emailInput;  
    public TMP_InputField passwordInput;
    public TMP_Text feedbackText; 
    public Button loginButton;

    private FirebaseAuth auth;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        loginButton.onClick.AddListener(() => Login(emailInput.text, passwordInput.text));
    }

    void Login(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                // Log the detailed exception message for more information
                feedbackText.text = "Login Failed: " + task.Exception?.ToString();
                Debug.LogError("Login failed: " + task.Exception?.ToString());
                return;
            }

            FirebaseUser user = task.Result.User;
            Debug.Log("Login Successful: " + user.Email);
            SceneManager.LoadScene("Currentlocation");
        });
    }
}
