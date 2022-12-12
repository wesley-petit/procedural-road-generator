using System;

public class SlopeHeuristic : Heuristic
{
    public override float CalculateHeuristic(in Vertex a, in Vertex b)
    {
        return _weight * Math.Abs(a.Position.y - b.Position.y);
        // Vector3 c = b.Position - a.Position;
        // float slope = Vector3.Dot(c, Vector3.up);
    }
}
