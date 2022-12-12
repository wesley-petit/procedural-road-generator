using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadConstructor : MonoBehaviour
{
    [SerializeField] private AStar _pathfinding;
    [SerializeField] private GraphUtils _graphUtils;
    
    private Graph _graph;

    public void Inject(in Graph graph)
    {
        _graph = graph;
    }

    public List<Path> ConstructRoad(in List<City> cities)
    {
        List<City> towns = cities.Where(c => c.Type == City.CityType.VILLE).ToList();
        List<City> villages = cities.Where(c => c.Type != City.CityType.VILLE).ToList();
        
        var roadNetwork = ConstructMainNetwork(towns);
        AddMinorCity(roadNetwork, villages);

        return roadNetwork;
    }

    /// <summary>
    /// Connect all towns with highway
    /// Ref : https://scholarworks.rit.edu/cgi/viewcontent.cgi?article=6536&context=theses
    /// </summary>
    /// <param name="town"></param>
    /// <returns></returns>
    public List<Path> ConstructMainNetwork(in List<City> town)
    {
        List<Path> roadNetwork = new();
        List<City> connectedCities = new List<City>();
        List<City> unconnectedCities = town;

        var visitTown = town[0];
        connectedCities.Add(visitTown);
        unconnectedCities.Remove(visitTown);
        
        while (unconnectedCities.Count != 0)
        {
            Path shortestPath = null;

            // Connect a town with the network
            // We connect by the smallest cost
            foreach (var networkCity in connectedCities)
            {
                foreach (var awaitingCity in unconnectedCities)
                {
                    Vertex start = _graphUtils.ConvertWorldPositionToVertex(networkCity.Position);
                    Vertex end = _graphUtils.ConvertWorldPositionToVertex(awaitingCity.Position);

                    Path path = _pathfinding.GetShortestPath(start, end, _graph);
                    if (shortestPath == null || path.Cost < shortestPath.Cost)
                    {
                        path.From = networkCity;
                        path.To = awaitingCity;
                        shortestPath = path;
                    }
                }
            }

            if (shortestPath != null)
            {
                unconnectedCities.Remove(shortestPath.To);
                connectedCities.Add(shortestPath.To);
                roadNetwork.Add(shortestPath);
            }
        }

        return roadNetwork;
    }

    public void AddMinorCity(in List<Path> roadNetwork, in List<City> villages)
    {
        List<City> unconnectedCities = villages;

        while (unconnectedCities.Count != 0)
        {
            var city = unconnectedCities[0];

            float smallestDistance = float.PositiveInfinity;
            Vector3 nearestVertexPosition = default;
            Path nearestRoad = null;
            foreach (var path in roadNetwork)
            {
                foreach (var vertex in path.Vertex)
                {
                    var d = Vector3.Distance(city.Position, vertex.Position);
                    if (d < smallestDistance)
                    {
                        smallestDistance = d;
                        nearestVertexPosition = vertex.Position;
                        nearestRoad = path;
                    }
                }
            }
   
            Vertex start = _graphUtils.ConvertWorldPositionToVertex(city.Position);
            Vertex end = _graphUtils.ConvertWorldPositionToVertex(nearestVertexPosition);

            var minorCityPath = _pathfinding.GetShortestPath(start, end, _graph);
            minorCityPath.From = city;
            
            // Set road direction to the nearest town
            if (Vector3.Distance(city.Position, nearestRoad.From.Position) <
                Vector3.Distance(city.Position, nearestRoad.To.Position))
            {
                minorCityPath.To = nearestRoad.From; 
            }
            else
            {
                minorCityPath.To = nearestRoad.To;
            }
            
            roadNetwork.Add(minorCityPath); 
            unconnectedCities.Remove(city);
        }
    }
}
