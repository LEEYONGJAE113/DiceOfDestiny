using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PieceController : MonoBehaviour
{
    [SerializeField] private Piece piece; // 현재 기물
    [SerializeField] private Vector2Int lastMoveDirection = Vector2Int.zero; // 마지막 방향
    private Vector2Int gridPosition;

    // 전개도 데이터 (십자형: 0:바닥, 1:앞, 2:위, 3:뒤, 4:왼쪽, 5:오른쪽)
    private readonly int[] upTransition = new int[] { 1, 2, 3, 0, 4, 5 }; // 위로 이동
    private readonly int[] downTransition = new int[] { 3, 0, 1, 2, 4, 5 }; // 아래로 이동
    private readonly int[] leftTransition = new int[] { 4, 1, 5, 3, 2, 0 }; // 왼쪽으로 이동
    private readonly int[] rightTransition = new int[] { 5, 1, 4, 3, 0, 2 }; // 오른쪽으로 이동

    [SerializeField] private SpriteRenderer classRenderer;
    [SerializeField] public SpriteRenderer colorRenderer;
    

    bool isMoving = false; // 이동 중인지 여부

    public StatusEffectController statusEffectController;

    void Start()
    {
        gridPosition = new Vector2Int(0, 0);

        statusEffectController = GetComponent<StatusEffectController>();
    }

    void Update()
    {
        TestInput();
    }


    public void TestInput() // 이벤트로 넘기거나 할 필요가 있을듯................................하바ㅏㅏㅏㅏㅏㅏ니ㅏㄷ..............밑에관련메소드잇음................................

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

            // 이동 확정 시
            // 행동력이 0이면 행동 불가
            if (!GameManager.Instance.actionPointManager.TryUseAP())
                return;

            // 이동하는 곳이 보드 밖이면 return
            if (!ObstacleManager.Instance.IsInsideBoard(newPosition))
            {
                return;
            }

            if (statusEffectController.IsStatusActive(StatusType.Stun)) // if (piece.debuff.IsStun)
            {
                Debug.Log("Piece is stunned!");
                return;
            }

            // 이동하는 곳에 장애물이 있으면
            Debug.Log("Obstacle Name : " + BoardManager.Instance.Board[newPosition.x, newPosition.y].Obstacle);
            if (BoardManager.Instance.Board[newPosition.x, newPosition.y].Obstacle != ObstacleType.None)
            {
                // 밟을 수 없다면
                if (!BoardManager.Instance.Board[newPosition.x, newPosition.y].isWalkable)
                {
                    RotateHalfBack(moveDirection); // 튕김 애니메이션
                    return;
                }
                else
                {
                    // 밟을 수 있는 장애물을 밟아서 효과 발동!
                    //PieceManager.Instance.AddDebuffPiece(BoardManager.Instance.Board[newPosition.x, newPosition.y].Obstacle, this);
                }
            }

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

                

                // 이전 타일에 Piece 값을 null로 바꾸고, 다음 타일에 Piece 값을 적용 
                BoardManager.Instance.Board[gridPosition.x, gridPosition.y].SetPiece(null);
                BoardManager.Instance.Board[newPosition.x, newPosition.y].SetPiece(this);

                GameManager.Instance.actionPointManager.PieceAction();


                // 마지막 이동 방향 저장
                lastMoveDirection = moveDirection;

                RotateToTopFace(moveDirection);
                UpdateTopFace(moveDirection); // 윗면 업데이트

                //RotateHalfBack(moveDirection);

                ObstacleManager.Instance.UpdateObstacleStep();
            }
            else
            {
                Debug.LogWarning($"Invalid move to position: {newPosition}");
            }
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
        isMoving = true; // 이동 중 입력받지 아니함. 

        int nextfaceIndex = 3; // 다음 윗면 인덱스

        if (moveDirection == Vector2Int.up)
            nextfaceIndex = 3;
        else if (moveDirection == Vector2Int.down)
            nextfaceIndex = 1;
        else if (moveDirection == Vector2Int.right)
            nextfaceIndex = 4;
        else if (moveDirection == Vector2Int.left)
            nextfaceIndex = 5;
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

        newClassRenderer.sprite = piece.faces[nextfaceIndex].classData.sprite;
        newColorRenderer.color = BoardManager.Instance.tileColors[(int)piece.faces[nextfaceIndex].color];

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

        float inflateAmount = 0.4f; // 부풀어 오르는 양

        while (time < duration)
        {
            float t = time / duration;
            float ease = Mathf.SmoothStep(0f, 1f, t);

            float inflate = Mathf.Sin(ease * Mathf.PI) * inflateAmount;
            float totalScale = 1f + inflate;

            float scaleOld = (1f - ease) * totalScale;
            float scaleNew = ease * totalScale;

            // 렌더러 위치 (접점 기준 위치)
            Vector3 offsetOld = moveVec * (scaleOld * 0.5f);
            Vector3 offsetNew = -moveVec * (scaleNew * 0.5f);

            classTransform.localPosition = offsetOld;
            colorTransform.localPosition = offsetOld;
            newClassObj.transform.localPosition = offsetNew;
            newColorObj.transform.localPosition = offsetNew;

            Vector3 scaleVecOld;
            Vector3 scaleVecNew;

            // 렌더러 스케일 
            if (vertical)
            {
                scaleVecOld = new Vector3(1f, scaleOld, 1f);
                scaleVecNew = new Vector3(1f, scaleNew, 1f);
            }
            else
            {
                scaleVecOld = new Vector3(scaleOld, 1f, 1f);
                scaleVecNew = new Vector3(scaleNew, 1f, 1f);
            }

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

        // 마무리.
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

        // 스킬 발동
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.TryActivateSkill(gridPosition, this);
        }
        else
        {
            Debug.LogError("SkillManager.Instance is null!");
        }

    }

    public void RotateHalfBack(Vector2Int moveDirection)
    {
        StartCoroutine(RotateHalfBackCoroutine(moveDirection));
    }

    IEnumerator RotateHalfBackCoroutine(Vector2Int moveDirection)
    {
        isMoving = true;

        float overshoot = 0.3f; // 진행 정도

        int nextfaceIndex = 3;
        if (moveDirection == Vector2Int.up) nextfaceIndex = 3; // 위로 이동
        if (moveDirection == Vector2Int.down) nextfaceIndex = 1;
        if (moveDirection == Vector2Int.right) nextfaceIndex = 4;
        if (moveDirection == Vector2Int.left) nextfaceIndex = 5;

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

        newClassRenderer.sprite = piece.faces[nextfaceIndex].classData.sprite;
        newColorRenderer.color = BoardManager.Instance.tileColors[(int)piece.faces[nextfaceIndex].color];

        // 초기 위치
        classTransform.localPosition = moveVec * 0.5f;
        colorTransform.localPosition = moveVec * 0.5f;
        newClassObj.transform.localPosition = -moveVec * 0.5f;
        newColorObj.transform.localPosition = -moveVec * 0.5f;

        classTransform.localScale = Vector3.one;
        colorTransform.localScale = Vector3.one;
        newClassObj.transform.localScale = Vector3.zero;
        newColorObj.transform.localScale = Vector3.zero;

        float duration = 0.4f; // 튕겨나감 + 복귀 전체 시간
        float time = 0f;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + moveVec * overshoot;

        float inflateAmount = 0.3f;

        while (time < duration)
        {
            float t = time / duration;

            // 빠르게 튕기고, 천천히 복귀하는 커브
            float ease = t < 0.3f
                ? Mathf.SmoothStep(0f, 1f, t / 0.3f)                    // 빠르게 전진 (0~0.3초)
                : Mathf.SmoothStep(1f, 0f, (t - 0.3f) / (0.7f));         // 천천히 복귀 (0.3~1)

            float inflate = Mathf.Sin(ease * Mathf.PI) * inflateAmount;
            float totalScale = 1f + inflate;

            float scaleOld = (1f - ease) * totalScale;
            float scaleNew = ease * totalScale;

            Vector3 offsetOld = moveVec * (scaleOld * 0.5f);
            Vector3 offsetNew = -moveVec * (scaleNew * 0.5f);

            classTransform.localPosition = offsetOld;
            colorTransform.localPosition = offsetOld;
            newClassObj.transform.localPosition = offsetNew;
            newColorObj.transform.localPosition = offsetNew;

            Vector3 scaleVecOld = vertical ? new Vector3(1f, scaleOld, 1f) : new Vector3(scaleOld, 1f, 1f);
            Vector3 scaleVecNew = vertical ? new Vector3(1f, scaleNew, 1f) : new Vector3(scaleNew, 1f, 1f);

            classTransform.localScale = scaleVecOld;
            colorTransform.localScale = scaleVecOld;
            newClassObj.transform.localScale = scaleVecNew;
            newColorObj.transform.localScale = scaleVecNew;

            float arc = Mathf.Sin(ease * Mathf.PI) * 0.15f;
            float contactOffset = (scaleOld - scaleNew) * 0.5f;

            transform.position = Vector3.Lerp(startPos, endPos, ease)
                                 + Vector3.up * arc
                                 - moveVec * contactOffset;

            time += Time.deltaTime;
            yield return null;
        }

        // 정리
        classTransform.localPosition = Vector3.zero;
        colorTransform.localPosition = Vector3.zero;
        classTransform.localScale = Vector3.one;
        colorTransform.localScale = Vector3.one;
        transform.position = startPos;

        Destroy(newClassObj);
        Destroy(newColorObj);

        isMoving = false;
    }

    public Face GetFace(int index)
    {
        if (index >= 0 && index < 6)
            return piece.faces[index];
        Debug.LogError($"Invalid face index: {index}");
        return default;
    }

    public Piece GetPiece()
    {
        return piece;
    }

    public void SetPiece(Piece newPiece)
    {
        piece = newPiece;
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }


    public Vector2Int GetLastMoveDirection()
    {
        return lastMoveDirection;
    }

    private bool isInGame;
    public bool IsinGame => IsinGame;


    public void Init(Piece piece)
    {
        gridPosition = new Vector2Int(0, 0);
        this.piece = piece;
    }

    public void SetInGame(bool value)
    {
        isInGame = value;
    }

    public Vector2Int MovePiece(Directions dir)
    {
        switch (dir)
        {
            case Directions.Up:
                return Vector2Int.up;
            case Directions.Down:
                return Vector2Int.down;
            case Directions.Left:
                return Vector2Int.left;
            case Directions.Right:
                return Vector2Int.right;
            default:
                return Vector2Int.zero;
        }

    }
}