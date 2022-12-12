using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cast several raycast on a zone to create and connect a graph
/// </summary>
public class TerrainConverter : MonoBehaviour
{
    [SerializeField, Range(0, 3000)] int _width = 1000;
    [SerializeField, Range(0, 3000)] int _length = 1000;
    [SerializeField, Range(0, 1000)] int _maxHeight = 500;
    [SerializeField, Range(0, 50)] private int _gridSize = 10;

    [Header("Debug Only")]
    [SerializeField] private bool _bDebug = false;
    [SerializeField] private float _debugRayLength = 10f;
    [SerializeField] private float _debugDurationSec = 60.0f;
    
    /// <summary>
    /// Construct a grid with raycast on a zone
    /// </summary>
    /// <returns></returns>
    public Graph ConstructGrid()
    {
        Dictionary<Vector3, Vertex> vertexByPosition = new Dictionary<Vector3, Vertex>();
        
        // I use i and j coordinate to  estimate a vertex neighbors
        Dictionary<Vector2, Vector3> indexPositionToVertex = new Dictionary<Vector2, Vector3>();

        // Start ray zone around the game object
        var start = transform.position + new Vector3(-_width / 2.0f, 0f, -_length / 2.0f);
        for (int i = 0; i < _width; i += _gridSize)
        {
            for (int j = 0; j < _length; j += _gridSize)
            {
                Ray ray = new Ray(start + new Vector3(i, _maxHeight, j), Vector3.down);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    indexPositionToVertex.Add(new Vector2(i, j), hit.point);
                    vertexByPosition.Add(hit.point, new Vertex(hit.point));

                    if (_bDebug)
                    {
                        Debug.DrawLine(hit.point + Vector3.up * _debugRayLength, hit.point, Color.green, _debugDurationSec);
                    }
                }
                else if (_bDebug)
                {
                    Debug.DrawLine(ray.origin, ray.origin + ray.direction * _debugRayLength, Color.red, _debugDurationSec);
                }
            }
        }

        // Neighbors are in 8 direction
        Vector2[] neighborsDirection = {
            new Vector2(1,1),
            new Vector2(0,1),
            new Vector2(-1,1),
            new Vector2(-1,0),
            new Vector2(-1,-1),
            new Vector2(0,-1),
            new Vector2(1,-1),
            new Vector2(1,0),
        };
        
        Dictionary<Vector3, List<Vector3>> neighborsByVertex = new();
        foreach (var (indexPosition, vertexPosition) in indexPositionToVertex)
        {
            neighborsByVertex.Add(vertexPosition, new ());
            foreach (var direction in neighborsDirection)
            {
                var neighborPosition = indexPosition + direction * _gridSize;
                if (indexPositionToVertex.ContainsKey(neighborPosition))
                {
                    var neighbor = indexPositionToVertex[neighborPosition];
                    neighborsByVertex[vertexPosition].Add(neighbor);
                }
            }
        }

        return new Graph(vertexByPosition, neighborsByVertex);
    }
}
