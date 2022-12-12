using System.Collections.Generic;
using UnityEngine;

public abstract class RoadDisplay : MonoBehaviour
{
    public abstract void Draw(in List<Path> path);
}
