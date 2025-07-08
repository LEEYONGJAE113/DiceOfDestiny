using UnityEngine;

public enum States { Selectable, Selecting, CantSelect }
public class PieceState : MonoBehaviour
{
    private bool canSelect;
    public bool CanSelect => canSelect;
    private States currentState;
    public States CurrentState => currentState;

    public void ChangeState(States targetState)
    {
        if (currentState == targetState) { return; }
        currentState = targetState;
    }

    public void ChangeSelectable(bool value)
    {
        canSelect = value;
    }


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