using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Store all vertex and search their neighbors
/// </summary>
public class Graph
{
	public Graph()
	{
		Vertex = new();
		NeighborsByVertex = new();
	}

	public Graph(Dictionary<Vector3, Vertex> allVertex, Dictionary<Vector3, List<Vector3>> allNeighbors)
	{
		Vertex = allVertex;
		NeighborsByVertex = allNeighbors;
	}

	public Dictionary<Vector3, Vertex> Vertex { get; }
	public Dictionary<Vector3, List<Vector3>> NeighborsByVertex { get; }

	/// <summary>
	/// Find all neighbors around a given position
	/// </summary>
	/// <param name="current">Vertex use as a reference to determine his neighbors</param>
	public List<Vertex> GetNeighbors(Vertex current)
	{
		List<Vertex> neighbors = new List<Vertex>();
		neighbors.Capacity = NeighborsByVertex[current.Position].Count;

		foreach (var neighborPosition in NeighborsByVertex[current.Position])
			neighbors.Add(Vertex[neighborPosition]);

		return neighbors;
	}

	/// <summary>
	///	Return a list of all vertex in the graph
	/// </summary>
	public List<Vertex> GetVertex() => new(Vertex.Values);
}
