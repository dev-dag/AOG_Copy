using TMPro;
using UnityEngine;

public class ViewControl : MonoBehaviour
{
    public TimerView TimerView { get => timerView; }
    public MoveControllerView MoveControllerView { get => moveControllerView; }

    [SerializeField] private TimerView timerView;
    [SerializeField] private MoveControllerView moveControllerView;
    [SerializeField] private FillBarView playerHpBar;
    [SerializeField] private FillBarView AI_hpBar;

    public void Initialize(GameSceneControl newGameSceneControl, ObserverProperty<int> playerHP, ObserverProperty<int> AI_hp)
    {
        timerView.Initialize(newGameSceneControl);
        moveControllerView.Initialize(newGameSceneControl);

        playerHpBar.Initialize(playerHP.Value, playerHP.Value);
        AI_hpBar.Initialize(AI_hp.Value, AI_hp.Value);

        playerHP.onValueChengeEvent += (arg) => playerHpBar.Current = arg.newValue;
        AI_hp.onValueChengeEvent += (arg) => AI_hpBar.Current = arg.newValue;
    }
}
