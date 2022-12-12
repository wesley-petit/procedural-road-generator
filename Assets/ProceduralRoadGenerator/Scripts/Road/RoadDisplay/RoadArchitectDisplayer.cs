using GSD.Roads;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class made to display the roads. It uses the Road Architect plugin.
/// </summary>
public class RoadArchitectDisplayer : RoadDisplay
{
    [SerializeField] private GSDRoadSystem _roadSystem;
    [SerializeField] private float _yOffset = 1f;
    [SerializeField] private int _gapSkipVertex = 3;

    [Header("Roads Rules Display")]
    [SerializeField] private RoadSettings _highway = new(2.75f, LaneEnum.Four, 1f);
    [SerializeField] private RoadSettings _nationalRoad = new(2.25f, LaneEnum.Two, 0.5f);
    [SerializeField] private RoadSettings _countryRoad = new(1.5f, LaneEnum.Two, 1f, GSDRoad.RoadMaterialDropdownEnum.Dirt);

    [Header("Debug")]
    [SerializeField] private bool _bShowDebug;
    
    private Dictionary<RoadSettings, List<GSDRoad>> _roadBySettings = new();
    private List<GSDRoad> _roads = new ();

    // Update road on play
    private readonly FieldChangesTracker _fieldChangesTracker = new();
    private readonly HashSet<RoadSettings> _roadSettingsChanged = new();
    private float _timerInSeconds;
    private const float REFRESH_TIME_SECONDS = 1.5f;
    
    private void Awake()
    {
        _roadSystem.opt_bMultithreading = false;
        _roadSystem.opt_bAllowRoadUpdates = false;
    }

    private void OnValidate()
    {
        if (_fieldChangesTracker.TrackFieldChanges(this, x => x._highway))
            _roadSettingsChanged.Add(_highway);
        
        if (_fieldChangesTracker.TrackFieldChanges(this, x => x._nationalRoad))
            _roadSettingsChanged.Add(_nationalRoad);
        
        if (_fieldChangesTracker.TrackFieldChanges(this, x => x._countryRoad))
            _roadSettingsChanged.Add(_countryRoad);
    }

    private void Update()
    {
        if (_roadSettingsChanged.Count != 0)
        {
            _timerInSeconds += Time.deltaTime;
        }

        if (REFRESH_TIME_SECONDS < _timerInSeconds)
        {
            _timerInSeconds = 0f;
            
            foreach (var settingsChanged in _roadSettingsChanged)
            {
                if (_roadBySettings.ContainsKey(settingsChanged))
                {
                    foreach (var road in _roadBySettings[settingsChanged])
                        ConfigureRoadAppearance(road, settingsChanged);
                }
            }
            
            _roadSettingsChanged.Clear();
            
            UpdateAllRoads();
        }
    }

    private void DestroyRoads()
    {
        bool previousState = _roadSystem.opt_bAllowRoadUpdates;
        _roadSystem.opt_bAllowRoadUpdates = false;

        // Reset terrain modifications
        foreach (var r in _roads)
            GSDTerraforming.TerrainsReset(r);
        
        // Destroy all roads
        while (_roads.Count != 0)
        {
            var r = _roads[0];
            Destroy(r.gameObject);
            _roads.RemoveAt(0);
        }
        
        _roadBySettings.Clear();

        _roadSystem.opt_bAllowRoadUpdates = previousState;
    }
    
    public override void Draw(in List<Path> path)
    {
        DestroyRoads();
        _roadSystem.opt_bAllowRoadUpdates = false;
        
        foreach (var current in path)
        {
            var road = BuildRoad(in current.Vertex, _bShowDebug);
            var roadType = GetRoadType(current.From, current.To);
            ConfigureRoadAppearance(road, roadType);

            if (!_roadBySettings.ContainsKey(roadType))
                _roadBySettings.Add(roadType, new());
            
            _roadBySettings[roadType].Add(road);
            _roads.Add(road);
        }
        
        UpdateAllRoads();
    }

    private void UpdateAllRoads()
    {
        // Param�trage du syst�me, pour prendre en comtpe les changements effectu�s
        _roadSystem.opt_bAllowRoadUpdates = true;

        foreach (var road in _roads)
            road.UpdateRoad();
        
        _roadSystem.opt_bAllowRoadUpdates = false;
    }

    private List<Vector3> CleanVertexPosition(in Vertex[] vertices)
    {
        List<Vector3> vertexPos = new ();

        for (int i = 0; i < vertices.Length; i++)
        {
            if (i % _gapSkipVertex == 0 || i == vertices.Length - 1)
            {
                vertexPos.Add(vertices[i].Position + new Vector3(0f, _yOffset, 0f));
            }
        }
        
        return vertexPos;
    }
    
    private GSDRoad BuildRoad(in Vertex[] rawVertex, bool showGizmo = false)
    {
        var nodePositions = CleanVertexPosition(in rawVertex);
        
        // Affichage de la route � l'aide de la classe GSDRoadAutomation
        var r = GSDRoadAutomation.CreateRoad_Programmatically(_roadSystem, ref nodePositions);
        
        // Affichage/dissimulation du gizmo
        r.opt_GizmosEnabled = showGizmo;

        // Remove terrain modification
        r.opt_HeightModEnabled = false;
        r.opt_TreeModEnabled = false;
        r.opt_DetailModEnabled = false;
        
        return r;
    }

    private RoadSettings GetRoadType(in City from, in City to)
    {
        // Modification de l'apparence, selon le type de route
        // Une route ville/village est une route nationale
        if (from.Type != to.Type)
        {
            return _nationalRoad;
        }
        
        // Une route village/village est faite de boue
        if (from.Type == City.CityType.VILLAGE)
        {
            return _countryRoad;
        }

        // Une route ville/ville est une auto-route
        return _highway;
    }

    private void ConfigureRoadAppearance(in GSDRoad road, in RoadSettings roadSettings)
    {
        road.opt_tRoadMaterialDropdown = roadSettings.Type;
        road.opt_Lanes = (int) roadSettings.NbLane; // nombre de voies, pouvant valoir 2, 4 ou 6
        road.opt_LaneWidth = roadSettings.Width; // largeur des voies
        road.opt_ShoulderWidth = roadSettings.ShoulderWidth; // largeur des bandes d'arr�t d'urgence
    }
}
