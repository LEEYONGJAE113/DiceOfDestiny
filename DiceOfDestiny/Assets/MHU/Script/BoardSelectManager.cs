using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSelectManager : Singletone<BoardSelectManager>
{
    [Header("이펙트 설정")]
    [SerializeField] private GameObject highlight; // 빛나는 이펙트 프리팹 (빈 타일용)
    [SerializeField] private GameObject notHighlight; // 빛나지 않는 이펙트 프리팹 (장애물 타일용)
    [Header("클릭된 타일")]
    [SerializeField] public Vector2Int lastClickedPosition; // 마지막 클릭된 타일 위치

    private bool isWaitingForClick = false; // 클릭 대기 상태

    private Dictionary<Vector2Int, GameObject> activeEffects; // 활성화된 이펙트 저장
    private BoardManager boardManager;

    private void Awake()
    {
        activeEffects = new Dictionary<Vector2Int, GameObject>();
        boardManager = BoardManager.Instance;
    }

    // 타일마다 장애물 검사 후 적절한 이펙트 적용
    public void HighlightTiles()
    {
        ClearAllEffects(); // 기존 이펙트 제거

        for (int x = 0; x < boardManager.boardSize; x++)
        {
            for (int y = 0; y < boardManager.boardSize; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                Tile tile = boardManager.GetTile(position);
                if (tile != null)
                {
                    // 빈 타일에는 highlightEffectPrefab, 장애물 타일에는 effect2Prefab 적용
                    GameObject effectPrefab = boardManager.IsEmptyTile(position) ? highlight : notHighlight;
                    // 이펙트 프리팹 인스턴스화
                    GameObject effect = Instantiate(effectPrefab,
                        new Vector3(boardManager.boardTransform.position.x + x,
                                  boardManager.boardTransform.position.y + y,
                                  -1), // z=-1로 타일 위에 렌더링
                        Quaternion.identity,
                        boardManager.boardTransform);
                    activeEffects.Add(position, effect);
                }

                
            }
        }
    }

    // 모든 이펙트 제거
    public void ClearAllEffects()
    {
        foreach (var effect in activeEffects.Values)
        {
            if (effect != null)
            {
                Destroy(effect);
            }
        }
        activeEffects.Clear();
    }

    // 클릭된 타일의 위치를 비동기적으로 반환
    public IEnumerator WaitForTileClick()
    {
        isWaitingForClick = true;
        lastClickedPosition = Vector2Int.zero;

        while (isWaitingForClick)
        {
            yield return null;
        }

        yield return lastClickedPosition;
    }

    public void SetClickedTilePosition(Vector2Int position)
    {
        lastClickedPosition = position;
        isWaitingForClick = false;
        Debug.Log($"클릭된 타일 위치 저장: {lastClickedPosition}");
         ClearAllEffects();
    }

}