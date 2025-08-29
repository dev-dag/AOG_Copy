using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class SkillView : SerializedMonoBehaviour
{
    [SerializeField] private Archer target;
    [SerializeField] private Dictionary<int, SkillSlotView> slots;

    public void Initialize(Archer newTarget)
    {
        target = newTarget;

        if (target.IsInitialized == false)
        {
            return;
        }

        foreach (int index in target.Skills.Keys)
        {
            if (target.Skills[index] != null)
            {
                int tmpIndex = index;
                slots[index].SetSlot(index, target.Skills[index].Value, target.SkillCoolTimes[index]);
                target.Skills[index].onValueChengeEvent += (arg) => slots[tmpIndex].SetSlot(tmpIndex, arg.newValue, target.SkillCoolTimes[tmpIndex]);
            }
        }
    }
}