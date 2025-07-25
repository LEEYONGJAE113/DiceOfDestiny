using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIBackpack : MonoBehaviour
{
    [Header("Backpack Button")]
    [SerializeField] private Button BackpackOpenCloseButton;

    [SerializeField] private Button[] PieceAppearButton;

    [Header("Backpack UI")]
    [SerializeField] private GameObject UIPieceGroup;

    private void Start()
    {
        Debug.Log("UIBackpack Start");
        BackpackOpenCloseButton.onClick.AddListener(onClickBackpackOpenCloseButton);
    }


    public void onClickBackpackOpenCloseButton()
    {
        UIPieceGroup.SetActive(!UIPieceGroup.activeSelf);
    }

    public void onClickPieceAppearButton(int index)
    {
        Debug.Log(index + "번 피스 생성");
        StopAllCoroutines();

        if (!PieceManager.Instance.pieceInventory.slots[index].IsActivePiece())
        {
            Debug.Log("해당 슬롯에 기물이 존재하지 않습니다.");
            return;
        }

        // 선택 피스 저장
        PieceManager.Instance.pieceInventory.selectedSlot = PieceManager.Instance.pieceInventory.slots[index];

        // 피스 스폰
        StartCoroutine(SpawnPiece(index));
    }

    IEnumerator SpawnPiece(int index)
    {
        // 타일 선택 이미지 띄우기
        BoardSelectManager.Instance.HighlightTiles();

        // 클릭 기다림
        yield return BoardSelectManager.Instance.WaitForTileClick();

        // 위치 불러오기
        Vector2Int gridPos = BoardSelectManager.Instance.lastClickedPosition;

        // 피스 생성
        GameObject piece = Instantiate(PieceManager.Instance.piecePrefabs[index],
            new Vector2(BoardManager.Instance.boardTransform.position.x + gridPos.x,
                        BoardManager.Instance.boardTransform.position.y + gridPos.y),
            Quaternion.identity);

        PieceController currentPieceController = piece.GetComponent<PieceController>();

        // 보드 판 내부 좌표 초기화
        currentPieceController.gridPosition = gridPos;
        Debug.Log(currentPieceController.gridPosition);

        // 피스 리스트에 추가
        PieceManager.Instance.Pieces.Add(currentPieceController);

        // 현재 선택 피스
        PieceManager.Instance.SetCurrentPiece(currentPieceController);

        // 슬롯에 있는 피스 제거
        Debug.Log(index + "번 피스 제거");
        PieceManager.Instance.pieceInventory.slots[index].RemovePiece();
    }
}
