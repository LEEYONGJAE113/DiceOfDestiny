using UnityEngine;

public class PieceController : MonoBehaviour
{
    [SerializeField] private Piece piece; // 현재 기물
    Vector2Int gridPosition;

    // 전개도 데이터 (십자형: 0:바닥, 1:앞, 2:위, 3:뒤, 4:왼쪽, 5:오른쪽)
    private readonly int[] upTransition = new int[] { 1, 2, 3, 0, 4, 5 }; // 위로 이동
    private readonly int[] downTransition = new int[] { 3, 0, 1, 2, 4, 5 }; // 아래로 이동
    private readonly int[] leftTransition = new int[] { 4, 1, 5, 3, 2, 0 }; // 왼쪽으로 이동
    private readonly int[] rightTransition = new int[] { 5, 1, 4, 3, 0, 2 }; // 오른쪽으로 이동

    [SerializeField] private SpriteRenderer classRenderer;
    [SerializeField] public SpriteRenderer colorRenderer;

    void Start()
    {
        gridPosition = new Vector2Int(0, 0);
    }

    void Update()
    {
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
            if (newPosition.x >= 0 && newPosition.x < BoardManager.Instance.boardSize &&
                newPosition.y >= 0 && newPosition.y < BoardManager.Instance.boardSize)
            {
                if (PieceManager.Instance == null)
                {
                    Debug.LogError("PieceManager.Instance is null!");
                    return;
                }

                if (piece == null)
                {
                    Debug.LogError("Piece is null!");
                    return;
                }

                gridPosition = newPosition;
                transform.position = new Vector3(
                    BoardManager.Instance.boardTransform.position.x + gridPosition.x,
                    BoardManager.Instance.boardTransform.position.y + gridPosition.y,
                    0f
                );
                UpdateTopFace(moveDirection); // 윗면 업데이트
                RotateToTopFace();

                // 스킬 발동 확인
                if (SkillManager.Instance != null)
                {
                    SkillManager.Instance.TryActivateSkill(gridPosition, this);
                }
                else
                {
                    Debug.LogError("SkillManager.Instance is null!");
                }
            }
            else
            {
                Debug.LogWarning($"Invalid move to position: {newPosition}");
            }
        }
    }  
    public Face GetFace(int index)
    {
        if (index >= 0 && index < 6)
            return piece.faces[index];
        Debug.LogError($"Invalid face index: {index}");
        return default;
    }

    public void SetFace(int index, ClassData classData, TileColor color)
    {
        if (index >= 0 && index < 6)
        {
            piece.faces[index].classData = classData;
            piece.faces[index].color = color;
        }
        else
        {
            Debug.LogError($"Invalid face index for SetFace: {index}");
        }
    }

    public Face GetTopFace()
    {
        return piece.faces[2];
    }

    public void UpdateTopFace(Vector2Int direction)
    {
        Face[] newFaces = new Face[6];

        // 이동 방향에 따라 faces 배열 재배치
        if (direction == Vector2Int.up)
        {
            for (int i = 0; i < 6; i++)
                newFaces[i] = piece.faces[upTransition[i]];

        }
        else if (direction == Vector2Int.down)
        {
            for (int i = 0; i < 6; i++)
                newFaces[i] = piece.faces[downTransition[i]];

        }
        else if (direction == Vector2Int.left)
        {
            for (int i = 0; i < 6; i++)
                newFaces[i] = piece.faces[leftTransition[i]];

        }
        else if (direction == Vector2Int.right)
        {
            for (int i = 0; i < 6; i++)
                newFaces[i] = piece.faces[rightTransition[i]];
        }
        else
        {
            Debug.LogWarning($"Invalid move direction: {direction}");
            return;
        }

        piece.faces = newFaces;
    }

    void RotateToTopFace()
    {
        UpdateTopFace();
    }

    void UpdateTopFace()
    {
        classRenderer.sprite = GetTopFace().classData.sprite;
        colorRenderer.color = BoardManager.Instance.tileColors[(int)(GetTopFace().color)];
    }

    public Piece GetPiece()
    {
        return piece;
    }

    public void SetPiece(Piece newPiece)
    {
        piece = newPiece;
    }
}