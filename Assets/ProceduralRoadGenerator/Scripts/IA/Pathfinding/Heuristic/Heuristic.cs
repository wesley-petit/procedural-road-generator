using System;
using UnityEngine;

public abstract class Heuristic : MonoBehaviour
{
    public Action OnWeightChanged;
    [SerializeField, Range(0f, 10f)] protected float _weight = 1;
    
    /// <summary>
    /// Returns the result of an heuristic between a and b.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public abstract float CalculateHeuristic(in Vertex a, in Vertex b);

    private void OnValidate() => OnWeightChanged?.Invoke();
}
