using UnityEngine;

public class TempManager : Singletone<TempManager>
{
    [SerializeField] private GameObject NextStageUI;
    [SerializeField] private GameObject mainCanvasGroup;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            StageClear();
        }
    }

    public void StageClear()
    {
        // 인게임 보드판에 있는 피스들 제거
        foreach (var piece in PieceManager.Instance.Pieces)
        {
            Destroy(piece.gameObject);
        }

        // 피스 리스트에 제거
        PieceManager.Instance.Pieces.Clear();

        // 현재 선택 피스 null
        PieceManager.Instance.SetCurrentPiece(null);

        // 피스 선택 테두리 제거
        BoardSelectManager.Instance.DestroyPieceHighlightTile();

        Time.timeScale = 0f;

        mainCanvasGroup.SetActive(false);
        NextStageUI.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;

        mainCanvasGroup.SetActive(true);
        NextStageUI.SetActive(false);

        // 행동력, 턴 상태 초기화
        GameManager.Instance.actionPointManager.Reset();

        // 기물 인벤토리 초기화
        PieceManager.Instance.pieceInventory.ResetSlot();

        // 기물 인벤토리 UI 새로고침
        EventManager.Instance.TriggerEvent("Refresh");
    }
}
