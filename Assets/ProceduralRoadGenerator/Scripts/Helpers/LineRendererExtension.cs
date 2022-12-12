using UnityEngine;

public static class LineRendererExtension
{
    public static void ApplyPositions(this LineRenderer lineRenderer, Vector3[] positions)
    {
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }
    
    public static void ApplyPositions(this LineRenderer lineRenderer, Vertex[] vertexPath)
    {
        Vector3[] positions = new Vector3[vertexPath.Length];
        for (int i = 0; i < vertexPath.Length; i++)
            positions[i] = vertexPath[i].Position;

        lineRenderer.ApplyPositions(positions);
    }
}
