using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

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

    bool isMoving = false; // 이동 중인지 여부

    void Start()
    {
        gridPosition = new Vector2Int(0, 0);
    }

    void Update()
    {

        Vector2Int moveDirection = Vector2Int.zero;

        if (!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                moveDirection = Vector2Int.up;
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                moveDirection = Vector2Int.down;
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                moveDirection = Vector2Int.left;
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                moveDirection = Vector2Int.right;
        }

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

                UpdateTopFace(moveDirection); // 윗면 업데이트
                RotateToTopFace(moveDirection);

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

    public void RotateToTopFace(Vector2Int moveDirection)
    {
        StartCoroutine(RotateToTopFaceCoroutine(moveDirection));
    }

    IEnumerator RotateToTopFaceCoroutine(Vector2Int moveDirection)
    {
        isMoving = true;

        int faceIndex = -1;
        if (moveDirection == Vector2Int.up)
            faceIndex = 3;
        else if (moveDirection == Vector2Int.down)
            faceIndex = 1;
        else if (moveDirection == Vector2Int.right)
            faceIndex = 4;
        else if (moveDirection == Vector2Int.left)
            faceIndex = 5;
        else
        {
            Debug.LogWarning($"Invalid move direction for rotation: {moveDirection}");
            isMoving = false;
            yield break;
        }

        bool vertical = moveDirection == Vector2Int.up || moveDirection == Vector2Int.down;
        Vector3 moveVec = new Vector3(moveDirection.x, moveDirection.y, 0);

        Transform classTransform = classRenderer.transform;
        Transform colorTransform = colorRenderer.transform;

        GameObject newClassObj = Instantiate(classRenderer.gameObject, transform);
        GameObject newColorObj = Instantiate(colorRenderer.gameObject, transform);

        newClassObj.name = "NewClassRenderer";
        newColorObj.name = "NewColorRenderer";

        SpriteRenderer newClassRenderer = newClassObj.GetComponent<SpriteRenderer>();
        SpriteRenderer newColorRenderer = newColorObj.GetComponent<SpriteRenderer>();

        newClassRenderer.sprite = piece.faces[faceIndex].classData.sprite;
        newColorRenderer.color = BoardManager.Instance.tileColors[(int)piece.faces[faceIndex].color];

        // 위치 초기화
        classTransform.localPosition = moveVec * 0.5f;
        colorTransform.localPosition = moveVec * 0.5f;
        newClassObj.transform.localPosition = -moveVec * 0.5f;
        newColorObj.transform.localPosition = -moveVec * 0.5f;

        // 스케일 초기화
        classTransform.localScale = Vector3.one;
        colorTransform.localScale = Vector3.one;
        newClassObj.transform.localScale = Vector3.zero;
        newColorObj.transform.localScale = Vector3.zero;

        float duration = 0.8f;
        float time = 0f;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + moveVec;

        while (time < duration)
        {
            float t = time / duration;
            float ease = Mathf.SmoothStep(0f, 1f, t);

            float scaleOld = Mathf.Lerp(1f, 0f, ease);
            float scaleNew = Mathf.Lerp(0f, 1f, ease);

            // 렌더러 위치 (접점 기준 위치)
            Vector3 offsetOld = moveVec * (scaleOld * 0.5f);
            Vector3 offsetNew = -moveVec * (scaleNew * 0.5f);

            classTransform.localPosition = offsetOld;
            colorTransform.localPosition = offsetOld;
            newClassObj.transform.localPosition = offsetNew;
            newColorObj.transform.localPosition = offsetNew;

            // 렌더러 스케일
            Vector3 scaleVecOld = vertical
                ? new Vector3(1f, scaleOld, 1f)
                : new Vector3(scaleOld, 1f, 1f);

            Vector3 scaleVecNew = vertical
                ? new Vector3(1f, scaleNew, 1f)
                : new Vector3(scaleNew, 1f, 1f);

            classTransform.localScale = scaleVecOld;
            colorTransform.localScale = scaleVecOld;
            newClassObj.transform.localScale = scaleVecNew;
            newColorObj.transform.localScale = scaleVecNew;

            // 정확히 접점 기준으로 1만큼 이동하도록 보정
            float arc = Mathf.Sin(ease * Mathf.PI) * 0.15f;
            float contactOffset = (scaleOld - scaleNew) * 0.5f;

            transform.position = Vector3.Lerp(startPos, endPos, ease) + Vector3.up * arc - moveVec * contactOffset;

            time += Time.deltaTime;
            yield return null;
        }

        // 마무리
        gridPosition += moveDirection;
        transform.position = endPos;

        Destroy(classRenderer.gameObject);
        Destroy(colorRenderer.gameObject);

        classRenderer = newClassRenderer;
        colorRenderer = newColorRenderer;

        classRenderer.transform.localPosition = Vector3.zero;
        colorRenderer.transform.localPosition = Vector3.zero;
        classRenderer.transform.localScale = Vector3.one;
        colorRenderer.transform.localScale = Vector3.one;

        isMoving = false;
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