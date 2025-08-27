using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

/// <summary>
/// 좌우 이동 뷰에 대한 입력을 데이터에 반영하는 클래스
/// </summary>
public class MoveControllerView : MonoBehaviour
{
    [SerializeField] private GameObject leftObject;
    [SerializeField] private GameObject rightObject;

    private GameSceneControl gameSceneControl;
    private InputData inputData;
    private InputAction clickAction;

    private void Update()
    {
        inputData.xDirection = 0;

        var value = clickAction.ReadValue<float>();

        if (value > 0f)
        {
            var pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Mouse.current.position.value;

            List<RaycastResult> hits = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointerData, hits);

            foreach (var hit in hits)
            {
                if (hit.gameObject == leftObject)
                {
                    inputData.xDirection = -1;
                }
                else if (hit.gameObject == rightObject)
                {
                    inputData.xDirection = 1;
                }
            }
        }
    }

    public void Initialize(GameSceneControl newGameSceneControl)
    {
        gameSceneControl = newGameSceneControl;
        inputData = gameSceneControl.InputData;
        clickAction = gameSceneControl.InputActionAsset.FindActionMap("UI").FindAction("Click");
    }
}
