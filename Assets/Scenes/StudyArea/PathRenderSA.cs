using UnityEngine;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;  // Required for TextMeshPro

public class PathRenderSA : MonoBehaviour
{
    public Transform player;
    public Material dottedLineMaterial;
    private LineRenderer lineRenderer;
    private NavMeshAgent navMeshAgent;
    private NavMeshPath path;
    private float turnAudioTimer = 0f;
    public float turnAudioInterval = 2f;

    public Toggle soundToggle; 
    private bool isSoundOn = true; 
    public float visiblePathDistance = 2f;

    public float moveSpeed = 3f;
    private bool isPathValid = false;
    public int smoothness = 10;

    public float pathWidth = 0.3f;
    public float delayBeforeTracking = 2f;
    private float timeSincePathStart = 0f;
    private bool isTrackingEnabled = false;

    // Audio clips for left and right turns
    public AudioClip leftTurnClip;
    public AudioClip rightTurnClip;
    private AudioSource audioSource;

    // Destination cubes
    public GameObject Entrance;
    public GameObject LecRoom101;
    public GameObject StaffRoom2;
    public GameObject LecRoom103;
    public GameObject StaffOffice1;

    private Transform currentDestination;

    // UI Elements
    public GameObject alertPanel;  // The pop-up panel
    public TextMeshProUGUI alertText;  // The text inside the panel

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 1.5f;
        lineRenderer.endWidth = 1.5f;
        lineRenderer.material = dottedLineMaterial != null ? dottedLineMaterial : new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.red;
        lineRenderer.useWorldSpace = true;
        lineRenderer.textureMode = LineTextureMode.RepeatPerSegment;

        navMeshAgent = player.GetComponent<NavMeshAgent>();
        if (navMeshAgent == null) Debug.LogError("NavMeshAgent is missing on the player object!");

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        path = new NavMeshPath();

        // Make sure the alert panel is hidden at start
        alertPanel.SetActive(false);

        if (soundToggle != null)
        {
            soundToggle.onValueChanged.AddListener(ToggleSound);
        }
    }

    void Update()
    {
        turnAudioTimer += Time.deltaTime;

        currentDestination = GetActiveDestination();
        if (player == null || currentDestination == null)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        navMeshAgent.CalculatePath(currentDestination.position, path);

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            UpdateLineRendererWithPath(path);
            isPathValid = true;
            CheckForTurns();
        }
        else
        {
            lineRenderer.positionCount = 0;
            isPathValid = false;
        }

        if (isPathValid)
        {
            if (!isTrackingEnabled)
            {
                timeSincePathStart += Time.deltaTime;
                if (timeSincePathStart >= delayBeforeTracking)
                    isTrackingEnabled = true;
            }

            if (isTrackingEnabled)
                MoveAlongPath(path);

            // Check if the player is halfway through the path
            CheckHalfwayThroughPath(path);
        }
    }

    public void ClearPath()
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0; // Clear all path points
        }
        isTrackingEnabled = false; // Reset tracking state
        timeSincePathStart = 0f; // Reset path timing
        isPathValid = false; // Invalidate the path
    }

    Transform GetActiveDestination()
    {
        if (Entrance.activeSelf) return Entrance.transform;
        if (LecRoom101.activeSelf) return LecRoom101.transform;
        if (StaffRoom2.activeSelf) return StaffRoom2.transform;
        if (LecRoom103.activeSelf) return LecRoom103.transform;
        if (StaffOffice1.activeSelf) return StaffOffice1.transform;
        return null;
    }

    private void UpdateLineRendererWithPath(NavMeshPath path)
    {
        if (path.corners.Length == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        System.Collections.Generic.List<Vector3> visiblePath = new System.Collections.Generic.List<Vector3>();
        float distanceCovered = 0f;
        Vector3 lastPosition = player.position;

        // Iterate through path corners to show only what's ahead
        for (int i = 0; i < path.corners.Length; i++)
        {
            Vector3 corner = path.corners[i];
            distanceCovered += Vector3.Distance(lastPosition, corner);

            if (distanceCovered <= visiblePathDistance)
            {
                visiblePath.Add(corner);
            }
            else
            {
                // Calculate the point along the next segment where the "visible distance" ends
                float remainingDistance = visiblePathDistance - (distanceCovered - Vector3.Distance(lastPosition, corner));
                Vector3 direction = (corner - lastPosition).normalized;
                Vector3 visiblePoint = lastPosition + direction * remainingDistance;
                visiblePath.Add(visiblePoint);
                break;
            }

            lastPosition = corner;
        }

        lineRenderer.positionCount = visiblePath.Count;
        for (int i = 0; i < visiblePath.Count; i++)
        {
            lineRenderer.SetPosition(i, visiblePath[i]);
        }
    }
    private float CalculatePathLength(System.Collections.Generic.List<Vector3> pathPoints)
    {
        float length = 0f;
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            length += Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
        }
        return length;
    }

    private void MoveAlongPath(NavMeshPath path)
    {
        if (path.corners.Length < 2) return;

        Vector3 targetPosition = path.corners[1];
        navMeshAgent.SetDestination(targetPosition);

        if (Vector3.Distance(player.position, targetPosition) > 0.1f)
            navMeshAgent.Move((targetPosition - player.position).normalized * moveSpeed * Time.deltaTime);
        else if (path.corners.Length > 2)
            navMeshAgent.SetDestination(path.corners[2]);
    }

    private void CheckForTurns()
    {
        if (path.corners.Length < 3) return;

        Vector3 lastDirection = (path.corners[1] - path.corners[0]).normalized;

        for (int i = 1; i < path.corners.Length - 1; i++)
        {
            Vector3 currentDirection = (path.corners[i + 1] - path.corners[i]).normalized;

            float angle = Vector3.SignedAngle(lastDirection, currentDirection, Vector3.up);

            if (Mathf.Abs(angle) > 45f)
            {
                if (angle > 0)
                {
                    PlayAudioClip(rightTurnClip, "Right Turn Detected!");
                    ShowTurnAlert("Turn Right");
                }
                else
                {
                    PlayAudioClip(leftTurnClip, "Left Turn Detected!");
                    ShowTurnAlert("Turn Left");
                }
            }

            lastDirection = currentDirection;
        }
    }

    private void ToggleSound(bool isOn)
    {
        isSoundOn = isOn;
        Debug.Log("Sound is now " + (isSoundOn ? "ON" : "OFF"));
    }

    private void PlayAudioClip(AudioClip clip, string logMessage)
    {
        if (isSoundOn && clip != null && !audioSource.isPlaying)
        {
            if (turnAudioTimer >= turnAudioInterval)
            {
                audioSource.PlayOneShot(clip);
                Debug.Log(logMessage);
                turnAudioTimer = 0f; // Reset timer after playing
            }
        }
    }


    private void ShowTurnAlert(string message)
    {
        alertPanel.SetActive(true);
        alertText.text = message;

        CancelInvoke(nameof(HideTurnAlert));
        Invoke(nameof(HideTurnAlert), 2f); 
    }

    private void HideTurnAlert()
    {
        alertPanel.SetActive(false); // Hide the pop-up
    }

    private void CheckHalfwayThroughPath(NavMeshPath path)
    {
        if (path.corners.Length < 2) return;

        float pathLength = CalculatePathLength(new System.Collections.Generic.List<Vector3>(path.corners));
        float halfwayPoint = pathLength / 2f;
        float currentDistance = Vector3.Distance(player.position, path.corners[0]);

        // Check if the player is halfway through
        if (currentDistance >= halfwayPoint)
        {
            ShowAlert("You're halfway through the " + GetActiveDestinationName());
        }
    }

    private string GetActiveDestinationName()
    {
        if (Entrance.activeSelf) return "Entrance";
        if (LecRoom101.activeSelf) return "LecRoom101";
        if (StaffRoom2.activeSelf) return "StaffRoom2";
        if (LecRoom103.activeSelf) return "LecRoom103";
        if (StaffOffice1.activeSelf) return "StaffOffice1";
        return "Unknown";
    }

    private void ShowAlert(string message)
    {
        alertPanel.SetActive(true);
        alertText.text = message;
    }
}