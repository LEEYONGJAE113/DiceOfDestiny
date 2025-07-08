using UnityEngine;

public class PieceController : MonoBehaviour
{
    Piece piece; // 현재 기물

    Vector2Int gridPosition;

    // 전개도 데이터 (십자형: 0:바닥, 1:앞, 2:위, 3:뒤, 4:왼쪽, 5:오른쪽)
    private readonly int[] upTransition = new int[] { 1, 2, 3, 0, 4, 5 }; // 위로 이동
    private readonly int[] downTransition = new int[] { 3, 0, 1, 2, 4, 5 }; // 아래로 이동
    private readonly int[] leftTransition = new int[] { 4, 1, 5, 3, 2, 0 }; // 왼쪽으로 이동
    private readonly int[] rightTransition = new int[] { 5, 1, 4, 3, 0, 2 }; // 오른쪽으로 이동

    void Start()
    {
        gridPosition = new Vector2Int(0, 0);
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



    void Awake()
    {
        if (faces == null || faces.Length != 6)
        {
            faces = new Face[6];
            Debug.LogWarning("Faces array initialized.");
        }

        for (int i = 0; i < faces.Length; i++)
        {
            if (faces[i].classData == null /*|| faces[i].tileColor == null*/)
            {
                faces[i].classData = new ClassData();
                faces[i].tileColor = new TileColor();
                Debug.LogWarning($"Face {i} initialized with default values.");
            }
        }
    }

    public Face GetFace(int index)
    {
        if (index >= 0 && index < 6)
            return faces[index];
        Debug.LogError($"Invalid face index: {index}");
        return default;
    }

    public void SetFace(int index, ClassData classData, TileColor colorData)
    {
        if (index >= 0 && index < 6)
        {
            faces[index].classData = classData;
            faces[index].tileColor = colorData;
        }
        else
        {
            Debug.LogError($"Invalid face index for SetFace: {index}");
        }
    }

    public int GetTopFaceIndex()
    {
        return topFaceIndex;
    }

    public void UpdateTopFace(Vector2Int direction)
    {
        Face[] newFaces = new Face[6];

        // 이동 방향에 따라 faces 배열 재배치
        if (direction == Vector2Int.up)
        {
            for (int i = 0; i < 6; i++)
                newFaces[i] = faces[upTransition[i]];

        }
        else if (direction == Vector2Int.down)
        {
            for (int i = 0; i < 6; i++)
                newFaces[i] = faces[downTransition[i]];

        }
        else if (direction == Vector2Int.left)
        {
            for (int i = 0; i < 6; i++)
                newFaces[i] = faces[leftTransition[i]];

        }
        else if (direction == Vector2Int.right)
        {
            for (int i = 0; i < 6; i++)
                newFaces[i] = faces[rightTransition[i]];

        }
        else
        {
            Debug.LogWarning($"Invalid move direction: {direction}");
            return;
        }

        // faces 배열 업데이트
        faces = newFaces;

        // 디버깅: 회전 후 각 면의 상태 출력
        //for (int i = 0; i < 6; i++)
        //{
        //    Debug.Log($"Face {i}: ClassData={faces[i].classData}, ColorData={faces[i].colorData}");
        //}
    }

    void RoateToTopFace()
    {

    }

    public Piece GetPiece()
    {
        return piece;
    }

    public void SetPiece(Piece newPiece)
    {
        piece = newPiece;
        UpdateAllFacesVisual();
    }

    public void UpdateAllFacesVisual()
    {
        if (piece == null) return;
        int topFaceIndex = piece.GetTopFaceIndex();
        Face topFace = piece.GetFace(topFaceIndex);
        if (topClassRenderer != null /*&& topFace.tileColor != null*/ && topFace.classData != null)
        {
            topClassRenderer.sprite = topFace.classData.sprite;
            // topColorRenderer.color = topFace.tileColor.color;
            switch (topFace.tileColor) // temp
            {
                case TileColor.Red:
                    topColorRenderer.color = Color.red;
                    break;
                case TileColor.Blue:
                    topColorRenderer.color = Color.blue;
                    break;
                case TileColor.Yellow:
                    topColorRenderer.color = Color.yellow;
                    break;
                case TileColor.Green:
                    topColorRenderer.color = Color.green;
                    break;
                case TileColor.Gray:
                    topColorRenderer.color = Color.gray;
                    break;
                case TileColor.Purple:
                    topColorRenderer.color = Color.magenta;
                    break;
            }
        }
    }

    public void ChangeFaceColor(int faceIndex, TileColor newColorData)
    {
        if (piece != null)
        {
            Face face = piece.GetFace(faceIndex);
            piece.SetFace(faceIndex, face.classData, newColorData);
            UpdateAllFacesVisual();
        }
    }
}