using System.Linq;
using UnityEngine;

/// <summary>
/// Toolbox to use or debug a graph
/// </summary>
public class GraphUtils : MonoBehaviour
{
    [Header("Debugs Only")]
    [SerializeField] private bool _bDebug;
    [SerializeField] private GameObject _debugPlaceholderModel;
    [SerializeField] Color _debugPathColor;
    [SerializeField] Color _debugGraphColor;
    [SerializeField] private float _debugDuration = 60.0f;

    private Graph _graph;

    public void Inject(in Graph graph)
    {
        _graph = graph;
    }
    
    /// <summary>
    /// Get nearest vertex near a given position
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public Vertex ConvertWorldPositionToVertex(in Vector3 worldPosition)
    {
        var vertex = _graph.GetVertex();
        
        Vertex result = default;
        float smallestDistance = Mathf.Infinity;
        foreach (var v in vertex)
        {
            if (Vector3.Distance(worldPosition, v.Position) < smallestDistance)
            {
                smallestDistance = Vector3.Distance(worldPosition, v.Position);
                result = v;
            }
        }

        return result;
    }
    
    /// <summary>
    /// Instantiate a placeholder in each given position
    /// </summary>
    /// <param name="positions"></param>
    public void PlacePlaceholderAt(in Vector3[] positions)
    {
        if (!_bDebug)
            return;
        
        var thisTransform = transform;
        for (int i = 0; i < positions.Length; i++)
        {
            var currentClone = Instantiate(_debugPlaceholderModel, positions[i], Quaternion.identity, thisTransform);
            currentClone.name = $"Clone_{i}";
        }
    }
    
    /// <summary>
    /// Draw the current graph to help in vizualisation
    /// </summary>
    /// <param name="graph"></param>
    public void DrawGraph()
    {
        if (!_bDebug)
            return;
        
        // Draw vertex
        PlacePlaceholderAt(_graph.Vertex.Keys.ToArray());
        
        // Draw edges
        foreach (var (k, neighbors) in _graph.NeighborsByVertex)
            foreach (var n in neighbors)
                Debug.DrawLine(k, n, _debugGraphColor, _debugDuration);
    }
    
    /// <summary>
    /// Draw a given path from start to end
    /// </summary>
    /// <param name="path"></param>
    public void DrawPath(in Path path)
    {
        if (!_bDebug)
            return;
        
        for (int i = 0; i < path.Vertex.Length - 1; i++)
            Debug.DrawLine(path.Vertex[i].Position, path.Vertex[i + 1].Position, _debugPathColor, _debugDuration);
    }
}
