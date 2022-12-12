using System;
using UnityEngine;

public class HeuristicSumContainer : Heuristic
{
    [SerializeField] private Heuristic[] _heuristics;

    public override float CalculateHeuristic(in Vertex a, in Vertex b)
    {
        float result = 0.0f;
        foreach (var heuristic in _heuristics)
        {
            if (heuristic == this)
            {
                Debug.LogError($"Stack overflow avoid with {heuristic.name}");
                continue;
            }
            
            result += heuristic.CalculateHeuristic(a, b);
        }

        return _weight * result;
    }
}
