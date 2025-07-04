using UnityEngine;

public class TestPieceController : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private Vector2Int gridPosition;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private int boardSize = 10;
    [SerializeField] private float moveCooldown = 0.2f;
    [SerializeField] private float offsetX = -4.5f;
    [SerializeField] private float offsetY = -4.5f;
    private float lastMoveTime;

    void Start()
    {
        gridPosition = new Vector2Int(0, 0);
        UpdateWorldPosition();
        if (PieceManager.Instance != null)
        {
            PieceManager.Instance.UpdateAllFacesVisual(); // 초기 시각적 업데이트
        }
        else
        {
            Debug.LogError("PieceManager.Instance is null in Start!");
        }
    }

    void Update()
    {
        if (Time.time - lastMoveTime < moveCooldown)
            return;
        if (SkillManager.Instance.IsSkillActive())
            return;

        Vector2Int moveDirection = Vector2Int.zero;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            moveDirection = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            moveDirection = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            moveDirection = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            moveDirection = Vector2Int.right;

        if (moveDirection != Vector2Int.zero)
        {
            Vector2Int newPosition = gridPosition + moveDirection;
            if (newPosition.x >= 0 && newPosition.x < boardSize &&
                newPosition.y >= 0 && newPosition.y < boardSize)
            {
                if (PieceManager.Instance == null)
                {
                    Debug.LogError("PieceManager.Instance is null!");
                    return;
                }

                Piece piece = PieceManager.Instance.GetPiece();
                if (piece == null)
                {
                    Debug.LogError("Piece is null!");
                    return;
                }

                gridPosition = newPosition;
                piece.UpdateTopFace(moveDirection); // 윗면 업데이트
                UpdateWorldPosition();
                PieceManager.Instance.UpdateAllFacesVisual(); // 시각적 업데이트

                // 스킬 발동 확인
                if (SkillManager.Instance != null)
                {
                    Face topFace = piece.GetFace(piece.GetTopFaceIndex());
                    SkillManager.Instance.TryActivateSkill(gridPosition, topFace);
                }
                else
                {
                    Debug.LogError("SkillManager.Instance is null!");
                }

                lastMoveTime = Time.time;
            }
            else
            {
                Debug.LogWarning($"Invalid move to position: {newPosition}");
            }
        }
    }

    private void UpdateWorldPosition()
    {
        transform.position = new Vector3(
            gridPosition.x * gridSize + offsetX,
            gridPosition.y * gridSize + offsetY,
            transform.position.z
        );
    }
}