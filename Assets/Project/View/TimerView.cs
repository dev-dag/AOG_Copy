using TMPro;
using UnityEngine;

public class TimerView : MonoBehaviour
{
    [SerializeField] private TMP_Text remainTimeTmp;

    private GameSceneControl gameSceneControl;
    private Awaitable timerAwaiter;

    public void Initialize(GameSceneControl newGameSceneControl)
    {
        gameSceneControl = newGameSceneControl;

        remainTimeTmp.text = (gameSceneControl.TimeLimit - Time.time).ToString("N2");

        if (timerAwaiter != null)
        {
            timerAwaiter.Cancel();
        }

        timerAwaiter = Timer();
    }

    private async Awaitable Timer()
    {
        while (Time.time < gameSceneControl.TimeLimit)
        {
            remainTimeTmp.text = (gameSceneControl.TimeLimit - Time.time).ToString("N2");

            await Awaitable.NextFrameAsync();
        }

        remainTimeTmp.text = "0";
    }
}