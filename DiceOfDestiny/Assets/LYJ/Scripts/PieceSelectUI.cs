using UnityEngine;

public class PieceSelectUI : MonoBehaviour
{
    private enum SelectType { Selectable, CantSelect }
    [SerializeField] private SelectType selectType;
    public void OnClick()
    {
        switch (selectType)
        {
            case SelectType.Selectable:
                Debug.Log("피스 선택됨");
                // 매니저에서 상태 변경
                // ui지우기요청
                // 컨트롤 ui 그리기 요청
                break;
            case SelectType.CantSelect:
                Debug.Log("선택할 수 없는 피스");
                // ui 따로 띄워주기?
                break;
            default:
                Debug.Log("Piece UI Enum값 지정 안됨");
                break;
        }
        
    }
}
