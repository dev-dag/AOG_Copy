using System;
using UnityEngine;
using static ConditionControl;

/// <summary>
/// 컨디션 상태에 따라 애니메이션을 제어하는 클래스
/// </summary>
public class ConditionEffector
{
    private Animator animator;
    private ConditionControl conditionControl;

    public ConditionEffector(Animator newAnimator, ConditionControl newConditionControl)
    {
        animator = newAnimator;
        conditionControl = newConditionControl;

        conditionControl.onConditionChangeEvent += OnConditionChangeEventHandler;
    }

    private void OnConditionChangeEventHandler(bool isAdded, ConditionControl.ConditionEnum condition)
    {
        if (isAdded)
        {
            DoAnimation(condition);
        }
        else
        {
            UndoAnimation(condition);
        }
    }

    private void DoAnimation(ConditionEnum condition)
    {
        switch (condition)
        {
            case ConditionControl.ConditionEnum.Burn:
                animator.Play(Archer.AnimationHash.CONDITION_BURN, 1);
                break;
            case ConditionControl.ConditionEnum.Frozen:
                animator.Play(Archer.AnimationHash.CONDITION_FROZEN, 1);
                break;
            default:
                break;
        }
    }

    private void UndoAnimation(ConditionEnum condition)
    {
        switch (condition)
        {
            case ConditionControl.ConditionEnum.Burn:
            case ConditionControl.ConditionEnum.Frozen:
                animator.Play(Archer.AnimationHash.CONDITION_NORMAL, 1);
                break;
            default:
                break;
        }
    }
}