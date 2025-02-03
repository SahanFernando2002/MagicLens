using UnityEngine;
using UnityEngine.AI;
using TMPro;  // Required for TextMeshPro

public class PathRenderSO1 : MonoBehaviour
{
    public Transform player;
    public Material dottedLineMaterial;
    private LineRenderer lineRenderer;
    private NavMeshAgent navMeshAgent;
    private NavMeshPath path;
    private float turnAudioTimer = 0f;
    public float turnAudioInterval = 2f;

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
    public GameObject LecRoom103;
    public GameObject StaffOffice1;
    public GameObject StaffRoom2;

    private Transform currentDestination;

    // UI Elements
    public GameObject alertPanel;  // The pop-up panel
    public TextMeshProUGUI alertText;  // The text inside the panel

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.8f;
        lineRenderer.endWidth = 0.8f;
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

    Transform GetActiveDestination()
    {
        if (Entrance.activeSelf) return Entrance.transform;
        if (LecRoom101.activeSelf) return LecRoom101.transform;
        if (LecRoom103.activeSelf) return LecRoom103.transform;
        if (StaffOffice1.activeSelf) return StaffOffice1.transform;
        if (StaffRoom2.activeSelf) return StaffRoom2.transform;
        return null;
    }

    private void UpdateLineRendererWithPath(NavMeshPath path)
    {
        if (path.corners.Length == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        // Smooth the path
        System.Collections.Generic.List<Vector3> smoothedPath = new System.Collections.Generic.List<Vector3> { path.corners[0] };

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Vector3 start = path.corners[i];
            Vector3 end = path.corners[i + 1];
            for (int j = 1; j <= smoothness; j++)
            {
                float t = j / (float)(smoothness + 1);
                smoothedPath.Add(Vector3.Lerp(start, end, t));
            }
        }

        smoothedPath.Add(path.corners[path.corners.Length - 1]);
        lineRenderer.positionCount = smoothedPath.Count;

        for (int i = 0; i < smoothedPath.Count; i++)
        {
            lineRenderer.SetPosition(i, smoothedPath[i]);
        }

        float pathLength = CalculatePathLength(smoothedPath);
        float dotSpacing = pathLength / 150f;

        lineRenderer.material.mainTextureScale = new Vector2(dotSpacing, 1f);
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
                }
                else
                {
                    PlayAudioClip(leftTurnClip, "Left Turn Detected!");
                }
            }

            lastDirection = currentDirection;
        }
    }

    private void PlayAudioClip(AudioClip clip, string logMessage)
    {
        if (clip != null && !audioSource.isPlaying)
        {
            if (turnAudioTimer >= turnAudioInterval)
            {
                audioSource.PlayOneShot(clip);
                Debug.Log(logMessage);
                turnAudioTimer = 0f;
            }
        }
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
        if (LecRoom103.activeSelf) return "LecRoom103";
        if (StaffOffice1.activeSelf) return "StaffOffice1";
        if (StaffRoom2.activeSelf) return "StaffRoom2";
        return "Unknown";
    }

    private void ShowAlert(string message)
    {
        alertPanel.SetActive(true);
        alertText.text = message;
    }
}
