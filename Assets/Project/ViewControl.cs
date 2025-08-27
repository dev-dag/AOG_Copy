using TMPro;
using UnityEngine;

public class ViewControl : MonoBehaviour
{
    public TimerView TimerView { get => timerView; }

    [SerializeField] private TimerView timerView;

    public void Initialize(GameSceneControl newGameSceneControl)
    {
        timerView.Initialize(newGameSceneControl);
    }
}
