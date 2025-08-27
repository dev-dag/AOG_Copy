using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class GameSceneControl : SerializedMonoBehaviour
{
    public float TimeLimit { get; private set; }
    public InputActionAsset InputActionAsset { get => inputActionAsset; } // 인풋 액션 에셋
    public ViewControl ViewControl { get => viewControl; }
    public InputData InputData { get => inputData; }

    [SerializeField] private ViewControl viewControl;
    [SerializeField] private InputActionAsset inputActionAsset;
    [SerializeField] private InputData inputData;

    private void Awake()
    {
        // **** for test

        Initialize(Time.time + 80f);
        inputActionAsset.Enable();

        // **** for test
    }

    public void Initialize(float timeLimit)
    {
        TimeLimit = timeLimit;
        inputData = new InputData();

        viewControl.Initialize(this);
    }
}
