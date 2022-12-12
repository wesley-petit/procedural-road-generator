using UnityEngine;

public class HeuristicUpdater : MonoBehaviour
{
    private bool _bRefresh;
    private float _timerInSeconds;
    private ProceduralRoadManager _proceduralRoadManager;
    private const float REFRESH_TIME_SECONDS = 0.5f;

    private void Awake() => _proceduralRoadManager = FindObjectOfType<ProceduralRoadManager>();

    private void OnEnable()
    {
        Heuristic[] heuristics = GetComponents<Heuristic>();
        foreach (var heuristic in heuristics)
            heuristic.OnWeightChanged += Refresh;
    }
    
    private void OnDisable()
    {
        Heuristic[] heuristics = GetComponents<Heuristic>();
        foreach (var heuristic in heuristics)
            heuristic.OnWeightChanged -= Refresh;
    }

    private void Update()
    {
        if (_bRefresh)
        {
            _timerInSeconds += Time.deltaTime;

            if (REFRESH_TIME_SECONDS < _timerInSeconds)
            {
                _timerInSeconds = 0f;
                _bRefresh = false;

                _proceduralRoadManager.Refresh();
            }
        }
    }

    private void Refresh()
    {
        _bRefresh = true;
        _timerInSeconds = 0.0f;
    }
}
