using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class GameSceneControl : SingleTon<GameSceneControl>
{
    public float TimeLimit { get; private set; }
    public InputActionAsset InputActionAsset { get => inputActionAsset; } // 인풋 액션 에셋
    public ViewControl ViewControl { get => viewControl; }
    public InputData InputData { get => inputData; }
    public Archer Player { get => player; }
    public Archer AI1 { get => AI; }
    public Pool GlobalPool { get => globalPool; }

    [SerializeField, Required] private ViewControl viewControl;
    [SerializeField, Required] private InputActionAsset inputActionAsset;
    [SerializeField] private InputData inputData;
    [SerializeField, Required] private Archer player;
    [SerializeField, Required] private Archer AI;
    [SerializeField] private ObserverProperty<int> playerHP;
    [SerializeField] private ObserverProperty<int> AI_HP;
    [SerializeField, Required] Pool globalPool;

    protected override void Awake()
    {
        base.Awake();

        // **** for test

        Initialize(Time.time + 80f);
        inputActionAsset.Enable();

        // **** for test
    }

    public void Initialize(float timeLimit)
    {
        playerHP = new ObserverProperty<int>(1000);
        AI_HP = new ObserverProperty<int>(1000);

        TimeLimit = timeLimit;
        inputData = new InputData();

        viewControl.Initialize(this, playerHP, AI_HP);

        player.Initialize(AI, playerHP, 40f);
        AI.Initialize(player, AI_HP, 40f);
    }
}
