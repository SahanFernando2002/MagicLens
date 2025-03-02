using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class BackBtn: MonoBehaviour
{
    public string locationSceneName = "CurrentLocation"; // Adjust to match your location scene name

    public void OnBackButtonClick()
    {
        // Reset the app's state
        ResetAppState();

        // Load the location screen
        SceneManager.LoadScene(locationSceneName);
    }

    private void ResetAppState()
    {
        // Reset NavMesh agent path
        NavMeshAgent navMeshAgent = FindAnyObjectByType<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.ResetPath();
        }

        // Clear the path in PathRenderL101
        PathRender pathRender = FindAnyObjectByType<PathRender>();
        if (pathRender != null)
        {
            pathRender.ClearPath(); // Uses a new public method to clear the path safely
        }

        PathRenderL101 pathRender1 = FindAnyObjectByType<PathRenderL101>();
        if (pathRender != null)
        {
            pathRender.ClearPath(); // Uses a new public method to clear the path safely
        }

        PathRenderL103 pathRender2 = FindAnyObjectByType<PathRenderL103>();
        if (pathRender != null)
        {
            pathRender.ClearPath(); // Uses a new public method to clear the path safely
        }

        PathRenderSA pathRender3 = FindAnyObjectByType<PathRenderSA>();
        if (pathRender != null)
        {
            pathRender.ClearPath(); // Uses a new public method to clear the path safely
        }

        PathRenderSO1 pathRender4 = FindAnyObjectByType<PathRenderSO1>();
        if (pathRender != null)
        {
            pathRender.ClearPath(); // Uses a new public method to clear the path safely
        }

        PathRenderSR2 pathRender5 = FindAnyObjectByType<PathRenderSR2>();
        if (pathRender != null)
        {
            pathRender.ClearPath(); // Uses a new public method to clear the path safely
        }

        // Stop any playing audio
        AudioSource audioSource = FindAnyObjectByType<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}
