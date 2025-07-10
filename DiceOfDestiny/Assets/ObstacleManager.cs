using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    static public ObstacleManager Instance;

    [Header("Obstacle Prefab")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private GameObject treePrefab;
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private GameObject lionPrefab;
    [SerializeField] private GameObject puddlePrefab;
    [SerializeField] private GameObject chestPrefab;
    [SerializeField] private GameObject poisonousHerbPrefab;
    [SerializeField] private GameObject unicornPrefab;

    public Dictionary<ObstacleType, GameObject> obstaclePrefabs;

    public List<GameObject> currentObstacles;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        obstaclePrefabs = new Dictionary<ObstacleType, GameObject>
        {
            { ObstacleType.Zombie, zombiePrefab },
            { ObstacleType.Tree, treePrefab },
            { ObstacleType.Rock, rockPrefab },
            { ObstacleType.Lion, lionPrefab },
            { ObstacleType.Puddle, puddlePrefab },
            { ObstacleType.Chest, chestPrefab },
            { ObstacleType.PoisonousHerb, poisonousHerbPrefab },
            { ObstacleType.Unicorn, unicornPrefab },
        };

        currentObstacles = new List<GameObject>();
    }

    public void SetObstacle(GameObject obstacle)
    {
        currentObstacles.Add(obstacle);
    }

    public void RemoveAllObstacle()
    {
        foreach (GameObject obstacle in currentObstacles)
        {
            Destroy(obstacle);

        }
        currentObstacles.Clear();
    }


    public void UpdateObstacleStep()
    {
        foreach (GameObject obstacle in currentObstacles)
        {
            Obstacle obstacleComponent = obstacle.GetComponent<Obstacle>();

            if (obstacleComponent.obstacleType == ObstacleType.Zombie) // 좀비
            {
                DoZmobieLogic(obstacleComponent);
            }
        }
    }

    private void DoZmobieLogic(Obstacle zombie)
    {
        Debug.Log("DoZmobieLogic called for zombie at position: " + zombie.obstaclePosition);
        if (zombie.nextStep == NextStep.None)
        {
            zombie.nextStep = Random.Range(0, 2) == 1 ? NextStep.Left : NextStep.Right; // 최초 방향 랜덤 지정.
        }

        if (zombie.nextStep == NextStep.Right)
        {
            Debug.Log("Zombie is moving right from position: " + zombie.obstaclePosition);
            Vector2Int nextPosition = zombie.obstaclePosition + Vector2Int.right;
            Tile next = BoardManager.Instance.Board[nextPosition.x, nextPosition.y];
            if (next == null)
            {
                Debug.LogError("Next tile is null at position: " + nextPosition);
                return;
            }
            else
            {
                if (next.GetPiece() == null)
                {
                    Debug.Log("Next tile is empty at position: " + nextPosition);
                    if (next.Obstacle == ObstacleType.None)
                    {
                        // 움직이는 애니메이션
                        BoardManager.Instance.MoveObstacle(zombie, nextPosition);
                        Debug.Log("Move Zombie to " + nextPosition);
                        AnimateObstacleMove(zombie.nextStep, zombie);

                    }
                    else
                    {
                        Debug.Log("Next tile has an obstacle at position: " + nextPosition);
                        // 팅기는 애니메이션 후 기절
                    }
                }
                else
                {
                    if (true /*기절중인가? */)
                    {

                    }
                    else
                    {
                        // 공격! 기절시켜
                    }
                }
            }
        }
        else // if(obstacleComponent.nextStep == NextStep.Left)
        {
            Vector2Int nextPosition = zombie.obstaclePosition + Vector2Int.left;
            Tile next = BoardManager.Instance.Board[nextPosition.x, nextPosition.y];
            if (next == null)
            {
                Debug.LogError("Next tile is null at position: " + nextPosition);
                return;
            }
            else
            {
                if (next.GetPiece() == null)
                {
                    if (next.Obstacle == ObstacleType.None)
                    {
                        BoardManager.Instance.MoveObstacle(zombie, nextPosition);
                    }
                }
                else
                {
                    if (true /*기절중인가? */)
                    {

                    }
                    else
                    {
                        // 공격! 기절시켜
                    }
                }
            }
        }
    }

    public void AnimateObstacleMove(NextStep nextStep, Obstacle obstacle)
    {
        Vector2Int direction = nextStep switch
        {
            NextStep.Right => Vector2Int.right,
            NextStep.Left => Vector2Int.left,
            NextStep.Up => Vector2Int.up,
            NextStep.Down => Vector2Int.down,
            _ => Vector2Int.zero
        };

        if (direction == Vector2Int.zero)
        {
            Debug.LogWarning("잘못된 이동 방향입니다.");
            return;
        }

        Vector2Int currentPos = obstacle.obstaclePosition;
        Vector2Int nextPos = currentPos + direction;

        // 월드 좌표로 변환 (타일 크기 또는 보정이 필요하면 수정)
        Vector3 startPos = obstacle.transform.position;
        Vector3 targetPos = transform.position + (Vector3)(Vector2)(direction);

        float duration = 0.4f;
        float jumpHeight = 0.3f;

        if (direction.x != 0) // 좌우 이동은 점프 효과
        {
            Sequence seq = DOTween.Sequence();
            seq.Append(obstacle.transform.DOMoveY(startPos.y + jumpHeight, duration / 2).SetEase(Ease.OutQuad));
            seq.Join(obstacle.transform.DOMoveX(targetPos.x, duration).SetEase(Ease.Linear));
            seq.Append(obstacle.transform.DOMoveY(targetPos.y, duration / 2).SetEase(Ease.InQuad));
        }
        else // 상하 이동은 자연스러운 이동
        {
            obstacle.transform.DOMove(targetPos, duration).SetEase(Ease.InOutSine);
        }

        // 위치 데이터 갱신 (논리상 좌표도)
        obstacle.obstaclePosition = nextPos;
    }

    private void MoveObstacle(Obstacle _obstacleComponent, Vector2Int _vector2Int, NextStep _nextStep)
{
    Vector2Int nextPosition = _obstacleComponent.obstaclePosition + _vector2Int;

    // 이동하려는 위치가 보드 밖이면 return
    // if (!IsInsideBoard(nextPosition))
    // {
    //     return;
    // }

    if (BoardManager.Instance.IsEmptyTile(nextPosition))
    {
        // 이동하려는 위치에 장애물 있으면 return
        if (BoardManager.Instance.Board[nextPosition.x, nextPosition.y].Obstacle != ObstacleType.None)
        {
            return;
        }

        // 이동한 위치 기준으로 8칸 내에 플레이어가 있는가?
        DetectPlayer(nextPosition);

        // 이동하려는 위치에 플레이어가 있으면 return
        if (BoardManager.Instance.Board[nextPosition.x, nextPosition.y].GetPiece() != null)
        {
            Debug.Log("Meet Player to Obstacle");
            return;
        }

        BoardManager.Instance.MoveObstacle(_obstacleComponent, nextPosition);
    }
    else
    {
        _obstacleComponent.nextStep = _nextStep;
    }
}

private void DetectPlayer(Vector2Int _nextPosition)
{
    Vector2Int[] directions = new Vector2Int[]
    {
            new Vector2Int(-1, -1), // 좌상
            new Vector2Int(-1, 0),  // 좌
            new Vector2Int(-1, 1),  // 좌하
            new Vector2Int(0, -1),  // 상
            new Vector2Int(0, 1),   // 하
            new Vector2Int(1, -1),  // 우상
            new Vector2Int(1, 0),   // 우
            new Vector2Int(1, 1),    // 우하
            new Vector2Int(0, 0)    // 중앙
    };

    for (int i = 0; i < directions.Length; i++)
    {
        Vector2Int detection = _nextPosition + directions[i];

        // 감지하려는 위치가 밖이면 continue
        if (!IsInsideBoard(detection))
        {
            continue;
        }

        // 해당 좌표에 피스가 있으면 해당 피스 행동력 1 감소
        if (BoardManager.Instance.Board[detection.x, detection.y].GetPiece() != null)
        {
            Debug.Log("RemoveAP");
            GameManager.Instance.actionPointManager.RemoveAP(1);
            return;
        }
    }
}

public bool IsInsideBoard(Vector2Int pos)
{
    return pos.x >= 0 && pos.x < BoardManager.Instance.Board.GetLength(0)
        && pos.y >= 0 && pos.y < BoardManager.Instance.Board.GetLength(1);
}
}
