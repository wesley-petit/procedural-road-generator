using UnityEngine;

public class EuclidianDistanceHeuristic : Heuristic
{
    public override float CalculateHeuristic(in Vertex a, in Vertex b)
    {
        return _weight * Vector3.Distance(a.Position, b.Position);
    }
}