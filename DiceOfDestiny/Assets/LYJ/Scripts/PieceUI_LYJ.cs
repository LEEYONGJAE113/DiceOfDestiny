using System.Collections.Generic;
using UnityEngine;

public enum UIState { Selectable, Current, CantSelect }

public class PieceUI_LYJ : MonoBehaviour
{
    [Header("상태별 UI")]
    [SerializeField, Tooltip("선택 가능")] private GameObject UISelectable;
    [SerializeField, Tooltip("현재 선택 중")] private GameObject UICurrent;
    [SerializeField, Tooltip("선택 불가")] private GameObject UICantSelect;
    private bool CanSelect;


    // private Dictionary<UIState, GameObject> uiDic;
    // private UIState currentState;

    // private void Awake()
    // {
    //     GroupUIs();
    //     if (uiDic.TryGetValue(UIState.Selectable, out GameObject selectableUI))
    //     {
    //         selectableUI.SetActive(true);
    //     }
    // }

    // private void GroupUIs()
    // {
    //     uiDic = new Dictionary<UIState, GameObject>
    //     {
    //         {UIState.Selectable, UISelectable},
    //         {UIState.Current, UICurrent},
    //         {UIState.CantSelect, UICantSelect}
    //     };
    // }

    // public void TryChangeUIState(UIState targetState)
    // {
    //     if (currentState == targetState) { return; }
    //     if (alreadySelected && targetState == UIState.Current)
    //     {
    //         Debug.Log("이미 선택했던 말임 ㅅㄱ");
    //         return;
    //     }

    //     if (uiDic.TryGetValue(targetState, out GameObject targetUI))
    //     {
    //         targetUI.SetActive(true);
    //     }

    //     currentState = targetState;
    // }

}