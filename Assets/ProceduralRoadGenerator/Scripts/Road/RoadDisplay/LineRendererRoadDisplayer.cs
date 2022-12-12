using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class LineRendererRoadDisplayer : RoadDisplay
{
    [SerializeField] private Material _highwayMaterial;
    [SerializeField] private float _highwayWidth = 30.0f;
    [SerializeField] private Material _countryMaterial;
    [SerializeField] private float _countryWidth = 10.0f;

    private ObjectPool<LineRenderer> _lineRendererPooling = new(
        Create,
        actionOnGet: obj => obj.enabled = true,
        actionOnRelease: obj => obj.enabled = false);

    private List<LineRenderer> _activeLineRenderer = new();
    
    private static LineRenderer Create()
    {
        GameObject clone = new GameObject();
        var lineRenderer = clone.AddComponent<LineRenderer>();
        return lineRenderer;
    }

    public void ResetRoads()
    {
        foreach (var activeLine in _activeLineRenderer)
            _lineRendererPooling.Release(activeLine);

        _activeLineRenderer = new List<LineRenderer>();
    }
    
    public override void Draw(in List<Path> roads)
    {
        ResetRoads();
        foreach (var path in roads)
        {
            if (path.From.Type == City.CityType.VILLE)
            {
                Draw(path.Vertex, _highwayMaterial, _highwayWidth);
            }
            else
            {
                Draw(path.Vertex, _countryMaterial, _countryWidth);
            }
        }
    }
    
    private void Draw(in Vertex[] vertexPath, in Material roadMaterial, float roadWidth)
    {
        var lineRenderer = _lineRendererPooling.Get();
        lineRenderer.material = roadMaterial;
        lineRenderer.startWidth = roadWidth;
        lineRenderer.ApplyPositions(vertexPath);
        _activeLineRenderer.Add(lineRenderer);
    }
}
