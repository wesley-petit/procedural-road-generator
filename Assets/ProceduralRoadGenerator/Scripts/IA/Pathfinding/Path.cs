public class Path
{
    public Path(Vertex[] vertex)
    {
        From = null;
        To = null;
        Vertex = vertex;
        Cost = 0f;

        // Compute maximal travel cost
        foreach (var v in Vertex)
        {
            if (Cost < v.RealCost)
            {
                Cost = v.RealCost;
            }
        }
    }
    
    public Vertex[] Vertex;
    public float Cost;
    public City From;
    public City To;
}