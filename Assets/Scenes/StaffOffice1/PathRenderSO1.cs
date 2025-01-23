using UnityEngine;
using UnityEngine.AI;

public class PathRenderSO1: MonoBehaviour
{
    public Transform player; 
    public Material lineMaterial;
    private LineRenderer lineRenderer;
    private NavMeshAgent navMeshAgent;
    private NavMeshPath path;
    private float turnAudioTimer = 0f;
    public float turnAudioInterval = 2f;

    private float deviationTimer = 0f;
    private float deviationThreshold = 5f; 
    public float moveSpeed = 3f;
    private bool isPathValid = false;
    public int smoothness = 10;
    public AudioClip beepClip;
    private float vibrationInterval = 1f; 
    private float vibrationTimer = 0f;

    public float pathWidth = 0.3f; 
    public float delayBeforeTracking = 2f;
    private float lastPathLength = 0f;
    private float timeSincePathStart = 0f;
    private bool isTrackingEnabled = false;
    public AudioClip leftTurnClip;
    public AudioClip rightTurnClip;
    private AudioSource audioSource;

    // Destination cubes
    public GameObject Entrance;
    public GameObject LecRoom101;
    public GameObject LecRoom103;
    public GameObject StudyArea;
    public GameObject StaffRoom2;

    private Transform currentDestination;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = pathWidth;
        lineRenderer.endWidth = pathWidth;
        lineRenderer.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.red;
        lineRenderer.useWorldSpace = true;

        navMeshAgent = player.GetComponent<NavMeshAgent>();
        if (navMeshAgent == null) Debug.LogError("NavMeshAgent is missing on the player object!");

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        path = new NavMeshPath();
    }

    void Update()
    {
        // Update the turn audio timer
        turnAudioTimer += Time.deltaTime;

        // Rest of the Update code
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

            CheckPlayerOnPath();
        }
    }
    Transform GetActiveDestination()
    {
        if (Entrance.activeSelf) return Entrance.transform;
        if (LecRoom101.activeSelf) return LecRoom101.transform;
        if (LecRoom103.activeSelf) return LecRoom103.transform;
        if (StudyArea.activeSelf) return StudyArea.transform;
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

    private void CheckPlayerOnPath()
    {
        if (lineRenderer.positionCount < 2) 
        {
            Debug.Log("No path to check.");
            return;
        }

        float currentPathLength = 0f;
        for (int i = 0; i < lineRenderer.positionCount - 1; i++)
        {
            currentPathLength += Vector3.Distance(lineRenderer.GetPosition(i), lineRenderer.GetPosition(i + 1));
        }

        if (currentPathLength < lastPathLength)
        {
            vibrationTimer += Time.deltaTime;
            deviationTimer = 0f; // Reset deviation timer since the user is on the path

            if (vibrationTimer >= vibrationInterval)
            {
                Handheld.Vibrate();
                Debug.Log("On Path — Vibrating!");
                vibrationTimer = 0f;
            }
        }
        else
        {
            vibrationTimer = 0f;

            // Track how long the user has not been vibrating (potential deviation)
            deviationTimer += Time.deltaTime;
            if (deviationTimer >= deviationThreshold)
            {
                PlayAudioClip(beepClip, "User Deviated - Playing Beep Sound!");
                deviationTimer = 0f; // Reset timer after playing the beep sound
            }
        }

        lastPathLength = currentPathLength;
    }

    private void CheckForTurns()
    {
        if (path.corners.Length < 3) return; // We need at least 3 points to detect a turn

        Vector3 lastDirection = (path.corners[1] - path.corners[0]).normalized;

        for (int i = 1; i < path.corners.Length - 1; i++)
        {
            Vector3 currentDirection = (path.corners[i + 1] - path.corners[i]).normalized;

            float angle = Vector3.SignedAngle(lastDirection, currentDirection, Vector3.up);

            if (Mathf.Abs(angle) > 55f) // Threshold for turn detection (can be adjusted)
            {
                if (angle > 0) // Right Turn
                {
                    PlayAudioClip(rightTurnClip, "Right Turn Detected!");
                }
                else // Left Turn
                {
                    PlayAudioClip(leftTurnClip, "Left Turn Detected!");
                }
            }

            // Update last direction
            lastDirection = currentDirection;
        }
    }

    private void PlayAudioClip(AudioClip clip, string logMessage)
    {
        if (clip != null && !audioSource.isPlaying)
        {
            // Play audio only if it's not currently playing and 2 seconds have passed since the last play
            if (turnAudioTimer >= turnAudioInterval)
            {
                audioSource.PlayOneShot(clip);
                Debug.Log(logMessage);
                turnAudioTimer = 0f;  // Reset the timer after playing the audio
            }
        }
    }
}