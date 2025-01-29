using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScreenManager : MonoBehaviour
{
    public RawImage logo; // Your logo (Raw Image)
    public float fadeDuration = 2f; // Time for fade to black
    public string loginSceneName = "Login"; // Change this to your login scene's name

    private CanvasGroup canvasGroup;
    private bool isFading = false;
    private float timer = 0f;

    void Start()
    {
        // Add or get CanvasGroup to control transparency
        canvasGroup = logo.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = logo.gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 1f; // Ensure the logo starts fully visible
        Invoke(nameof(StartFadeToBlack), 2f); // Wait for 2 seconds before fading
    }

    void Update()
    {
        if (isFading)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = 1f - (timer / fadeDuration);

            if (timer >= fadeDuration)
            {
                SceneManager.LoadScene(loginSceneName);
            }
        }
    }

    void StartFadeToBlack()
    {
        isFading = true;
        timer = 0f;
    }
}
