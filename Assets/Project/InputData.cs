using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputData
{
    public ObserverProperty<int> ActivatedSkillIndex { get; private set; }
    public int xDirection;

    public InputData()
    {
        ActivatedSkillIndex = new ObserverProperty<int>(-1);
    }
}
