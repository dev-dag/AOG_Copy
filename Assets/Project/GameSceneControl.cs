using UnityEngine;

public class GameSceneControl : MonoBehaviour
{
    public float TimeLimit { get; private set; }
    public ViewControl ViewControl { get => viewControl; }

    [SerializeField] private ViewControl viewControl;

    private void Awake()
    {
        // **** for test

        Initialize(Time.time + 80f);

        // **** for test
    }

    public void Initialize(float timeLimit)
    {
        TimeLimit = timeLimit;

        viewControl.Initialize(this);
    }
}
