using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Implementation of A* in the search of shortest path
/// </summary>
public class AStar : MonoBehaviour
{
    [SerializeField] private Heuristic _heuristic;

    public Path GetShortestPath(in Vertex start, in Vertex end, in Graph graph)
    {
        // Reset compute cost from previous algorithm
        foreach (var (position, v) in graph.Vertex)
            v.ResetCost();

        // Hash Set collections avoid duplicate management
        HashSet<Vertex> visited = new();
        HashSet<Vertex> notVisited = new();
    
        notVisited.Add(start);

        while (0 < notVisited.Count)
        {
            // Select an vertex with the lowest cost and that has not been visited
            Vertex nearestVertex = notVisited.OrderBy(k => k.GetTotalCosts).First();

            notVisited.Remove(nearestVertex);
            visited.Add(nearestVertex);

            if (nearestVertex == end)
                return ConstructPath(in start, in end);

            // Update cost of near by nodes
            foreach (var neighbor in graph.GetNeighbors(nearestVertex))
            {
                if (visited.Contains(neighbor))
                    continue;

                // Estimate cost from start to end using neighbor. We use 3 values :
                // Cost from start to previous vertex, previous vertex to the current neighbor
                // and finally, an estimate cost or distance between neighbor and end.
            
                // We use an heuristic to determine a direction and maximise the chance of finding the shortest path.
                float estimateCost = nearestVertex.RealCost + neighbor.GraphCost + _heuristic.CalculateHeuristic(neighbor, end);
                if (!notVisited.Contains(neighbor) || estimateCost < neighbor.GetTotalCosts)
                {
                    neighbor.RealCost = nearestVertex.RealCost + neighbor.GraphCost;
                    neighbor.HeuristicCost = _heuristic.CalculateHeuristic(neighbor, end);
                    neighbor.Predecessor = nearestVertex;

                    notVisited.Add(neighbor);
                }
            }
        }

        return null;
    }
    
    /// <summary>
    /// If any, return a path of vertex from start to end
    /// </summary>
    /// <param name="start">Edge which will stop the path construction</param>
    /// <param name="end">Edge use as end in the path</param>
    protected Path ConstructPath(in Vertex start, in Vertex end)
    {
        List<Vertex> vertexPath = new();
        Vertex temp = end;
    
        while (temp != start)
        {
            if (temp == null)
            {
                Debug.LogError("No path available.");
                vertexPath.Clear();
                break;
            }
            vertexPath.Add(temp);
            temp = temp.Predecessor;
        }

        vertexPath.Add(start);
        vertexPath.Reverse();
        return new Path(vertexPath.ToArray());
    }
}
