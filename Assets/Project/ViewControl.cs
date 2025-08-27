using TMPro;
using UnityEngine;

public class ViewControl : MonoBehaviour
{
    public TimerView TimerView { get => timerView; }
    public MoveControllerView MoveControllerView { get => moveControllerView; }

    [SerializeField] private TimerView timerView;
    [SerializeField] private MoveControllerView moveControllerView;

    public void Initialize(GameSceneControl newGameSceneControl)
    {
        timerView.Initialize(newGameSceneControl);
        moveControllerView.Initialize(newGameSceneControl);
    }
}
