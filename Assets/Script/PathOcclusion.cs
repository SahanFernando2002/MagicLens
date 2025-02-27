using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathOcclusion : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public LayerMask occlusionLayer; // Assign WallsOcclusion layer in the inspector

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        OccludePath();
    }

    void OccludePath()
    {
        Vector3[] points = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(points);

        // Holds visible points for the LineRenderer
        var visiblePoints = new System.Collections.Generic.List<Vector3>();

        bool pathBlocked = false;

        for (int i = 0; i < points.Length - 1; i++)
        {
            if (Physics.Linecast(points[i], points[i + 1], occlusionLayer))
            {
                pathBlocked = true;
                break;
            }
            visiblePoints.Add(points[i]);
        }

        // If blocked, only show the visible points
        if (pathBlocked && visiblePoints.Count > 0)
        {
            lineRenderer.positionCount = visiblePoints.Count;
            lineRenderer.SetPositions(visiblePoints.ToArray());
        }
        else
        {
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
        }
    }
}
