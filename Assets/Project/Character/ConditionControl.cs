using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// 상태이상을 제어하는 클래스
/// </summary>
public class ConditionControl
{
    [Flags]
    public enum ConditionEnum
    {
        None = 0,
        Frozen = 1,
        Burn = 2,
    }

    public delegate void ConditionChangeEvent(bool isAdded, ConditionEnum condition);

    public ConditionEnum ConditionFlag { get; private set; }
    public Dictionary<ConditionEnum, float> ConditionTimes { get => conditionTimes; }

    public event ConditionChangeEvent onConditionChangeEvent;

    [SerializeField] private Dictionary<ConditionEnum, float> conditionTimes = new Dictionary<ConditionEnum, float>();
    private CancellationTokenSource canceler;

    public ConditionControl()
    {
        canceler = new CancellationTokenSource();

        CheckCondition(canceler.Token);
    }

    public void SetCondition(ConditionEnum newCondition, float time)
    {
        ConditionFlag |= newCondition;

        if (conditionTimes.ContainsKey(newCondition) == false)
        {
            conditionTimes.Add(newCondition, 0f);
        }

        conditionTimes[newCondition] = time;

        onConditionChangeEvent?.Invoke(true, newCondition);
    }

    /// <summary>
    /// 유지시간이 지난 컨디션을 해소시키는 함수
    /// </summary>
    private async Awaitable CheckCondition(CancellationToken ct)
    {
        List<ConditionEnum> removeList = new List<ConditionEnum>();

        while (true)
        {
            ct.ThrowIfCancellationRequested();

            foreach (ConditionEnum condition in conditionTimes.Keys)
            {
                if (Time.time >= conditionTimes[condition]) // 유지 시간이 지난 컨디션을 해제
                {
                    if ((ConditionFlag & condition) == condition)
                    {
                        ConditionFlag -= condition;
                        onConditionChangeEvent?.Invoke(false, condition);
                        removeList.Add(condition);
                    }
                }
            }

            if (removeList.Count > 0)
            {
                foreach (ConditionEnum condition in removeList) // 쿨타임이 지난 컨디션을 체크 목록에서 제외
                {
                    conditionTimes.Remove(condition);
                }

                removeList.Clear();
            }

            await Awaitable.NextFrameAsync();
        }
    }

    ~ConditionControl()
    {
        canceler.Cancel();
    }
}