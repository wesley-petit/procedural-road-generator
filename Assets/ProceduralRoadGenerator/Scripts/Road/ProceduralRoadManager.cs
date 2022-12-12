using UnityEngine;

/// <summary>
/// Manage procedural road construction and order
/// </summary>
public class ProceduralRoadManager : MonoBehaviour
{
    [SerializeField] private TerrainConverter _terrainConverter;
    [SerializeField] private CitiesGenerator _citiesGenerator;
    [SerializeField] private GraphUtils _graphUtils;
    [SerializeField] private RoadConstructor _roadConstructor;
    [SerializeField] private RoadDisplay _roadDisplayer;

    
    private void Awake()
    {
        // Graph Creation
        Graph graph = _terrainConverter.ConstructGrid();
        _graphUtils.Inject(graph);
        _roadConstructor.Inject(graph);
        
        // Debugs Only
        // _graphUtils.DrawGraph(Graph);
    }

    private void Start()
    {
        _citiesGenerator.Generate();
        Refresh();
    }

    public void Refresh()
    {
        // Road network generation
        var roadNetwork = _roadConstructor.ConstructRoad(_citiesGenerator.cities);     

        // Final road construction
        _roadDisplayer.Draw(roadNetwork);
    }
}