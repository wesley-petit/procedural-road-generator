using UnityEngine;

public enum LaneEnum{
    Two = 2,
    Four = 4,
    Six = 6
}

[System.Serializable]
public class RoadSettings
{
    [Range(1f, 3f)] public float Width;
    public LaneEnum NbLane;
    [Range(0f, 2f)] public float ShoulderWidth;
    public GSDRoad.RoadMaterialDropdownEnum Type;

    public RoadSettings(float width, LaneEnum nbLane, float shoulderWidth, GSDRoad.RoadMaterialDropdownEnum type = GSDRoad.RoadMaterialDropdownEnum.Asphalt)
    {
        Width = width;
        NbLane = nbLane;
        ShoulderWidth = shoulderWidth;
        Type = type;
    }
}
