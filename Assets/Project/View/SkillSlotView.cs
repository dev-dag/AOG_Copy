using System;
using System.Threading;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotView : SerializedMonoBehaviour
{
    [SerializeField, Required] private Canvas canvas;
    [SerializeField, Required] private Image iconImage;
    [SerializeField, Required] private Image coolTimeFillImage;
    [SerializeField, Required] private Button button;
    [SerializeField] private int slotIndex;
    [SerializeField] private ObserverProperty<float> coolTimeObserver;
    [SerializeField] private SO_Skill skillData;

    private CancellationTokenSource canceler;

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    public void SetSlot(int newSlotIndex, SO_Skill newSkillData, ObserverProperty<float> newCoolTimeObserver)
    {
        slotIndex = newSlotIndex;
        skillData = newSkillData;

        if (canceler != null)
        {
            canceler.Cancel();
            canceler = null;
        }

        if (skillData == null)
        {
            canvas.enabled = false;
            coolTimeObserver = null;
            coolTimeFillImage.fillAmount = 0f;
        }
        else
        {
            canvas.enabled = true;
            iconImage.sprite = skillData.Icon;

            if (coolTimeObserver != null)
            {
                coolTimeObserver.onValueChengeEvent -= OnCoolTimeObserverChanceEventHandler;
            }

            coolTimeObserver = newCoolTimeObserver;
            coolTimeObserver.onValueChengeEvent += OnCoolTimeObserverChanceEventHandler;

            SetTimer();
        }
    }

    private void OnCoolTimeObserverChanceEventHandler(ObserverEventArg<float> arg)
    {
        SetTimer();
    }

    private void SetTimer()
    {
        if (canceler != null)
        {
            canceler.Cancel();
            canceler = null;
        }

        canceler = new CancellationTokenSource();
        CoolTimer(canceler.Token);
    }

    private async Awaitable CoolTimer(CancellationToken ct)
    {
        float endTime = coolTimeObserver.Value;
        float startTime = Time.time;
        float coolTimeSpan = endTime - startTime;

        while (Time.time < endTime)
        {
            ct.ThrowIfCancellationRequested();

            var fillAmount = 1f - (Time.time - startTime) / coolTimeSpan;

            coolTimeFillImage.fillAmount = fillAmount;

            await Awaitable.NextFrameAsync();
        }

        coolTimeFillImage.fillAmount = 0f;
    }

    private void OnClick()
    {
        if (Time.time >= coolTimeObserver.Value)
        {
            GameSceneControl.Instance.InputData.ActivatedSkillIndex.Value = slotIndex;
        }
    }
}