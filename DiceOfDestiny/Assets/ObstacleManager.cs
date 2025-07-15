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
                DoZombieLogic(obstacleComponent);
            }
        }
    }

    private void DoZombieLogic(Obstacle zombie)
    {
        if (zombie.nextStep == NextStep.None)
        {
            zombie.nextStep = Random.Range(0, 2) == 1 ? NextStep.Left : NextStep.Right;
        }

        Vector2Int direction = GetDirection(zombie.nextStep);
        NextStep oppositeStep = GetOppositeStep(zombie.nextStep);

        Vector2Int nextPosition = zombie.obstaclePosition + direction;
        Tile nextTile = BoardManager.Instance.GetTile(nextPosition);

        if (nextTile == null)
        {
            AnimateObstacleHalfBack(zombie.nextStep, zombie);
            zombie.nextStep = oppositeStep;
            return;
        }

        if (nextTile.GetPiece() == null)
        {
            if (nextTile.Obstacle == ObstacleType.None)
            {
                BoardManager.Instance.MoveObstacle(zombie, nextPosition);
                AnimateObstacleMove(zombie.nextStep, zombie);
            }
            else
            {
                AnimateObstacleHalfBack(zombie.nextStep, zombie);
                zombie.nextStep = oppositeStep;
            }
        }
        else 
        {
            if (nextTile.GetPiece().GetTopFace().classData.IsCombatClass || nextTile.GetPiece().GetPiece().debuff.IsStun)
            {
                AnimateObstacleHalfBack(zombie.nextStep, zombie);
                zombie.nextStep = oppositeStep;
            }
            else
            {
                AnimateZombieNyamNyam(zombie.nextStep, zombie);
                zombie.nextStep = oppositeStep;

                Debug.Log("Piece SStun!");
                nextTile.GetPiece().GetPiece().debuff.SetStun(true, 2);
            }
        }
    }

    private Vector2Int GetDirection(NextStep step)
    {
        return step switch
        {
            NextStep.Right => Vector2Int.right,
            NextStep.Left => Vector2Int.left,
            NextStep.Up => Vector2Int.up,
            NextStep.Down => Vector2Int.down,
            _ => Vector2Int.zero
        };
    }

    private NextStep GetOppositeStep(NextStep step)
    {
        return step switch
        {
            NextStep.Right => NextStep.Left,
            NextStep.Left => NextStep.Right,
            NextStep.Up => NextStep.Down,
            NextStep.Down => NextStep.Up,
            _ => NextStep.None
        };
    }

    public void AnimateObstacleMove(NextStep nextStep, Obstacle obstacle)
    {
        Vector2Int direction = GetDirection(nextStep);

        Vector3 startPos = obstacle.transform.position;
        Vector3 targetPos = startPos + new Vector3(direction.x, direction.y, 0);

        float duration = 0.4f;
        float jumpHeight = 0.2f;

        if (direction.x != 0) // 좌우 이동은 점프 효과
        {
            Sequence seq = DOTween.Sequence();

            // 1) X축 이동 (duration 전체)
            seq.Append(obstacle.transform.DOMoveX(targetPos.x, duration).SetEase(Ease.InOutSine));

            // 2) Y축 점프 (올라갔다 내려오기) - duration 전체, Y만 움직임
            seq.Join(obstacle.transform.DOMoveY(startPos.y + jumpHeight, duration / 2).SetEase(Ease.OutSine));
            seq.Append(obstacle.transform.DOMoveY(startPos.y, duration / 2).SetEase(Ease.InSine));

        }
        else // 상하 이동은 자연스러운 이동
        {
            obstacle.transform.DOMove(targetPos, duration).SetEase(Ease.InOutSine);
        }
    }

    public void AnimateObstacleHalfBack(NextStep nextStep, Obstacle obstacle)
    {
        Vector2Int direction = GetDirection(nextStep);

        Vector3 startPos = obstacle.transform.position;
        Vector3 targetPos = startPos + new Vector3(direction.x, direction.y, 0);

        float duration = 0.6f;
        float jumpHeight = 0.2f;

        float ratio = 0.7f;
        Vector3 hitPos = Vector3.Lerp(startPos, targetPos, ratio);

        if (direction.x != 0) // 좌우 이동은 점프 효과
        {
            Sequence seq = DOTween.Sequence();

            // 1) X축 이동 (duration 전체)
            seq.Append(obstacle.transform.DOMoveX(hitPos.x, duration / 3).SetEase(Ease.InSine));
            // 2) Y축 점프 (올라갔다 내려오기) - duration 전체, Y만 움직임
            seq.Append(obstacle.transform.DOMoveY(startPos.y + jumpHeight, duration / 3).SetEase(Ease.OutSine));
            seq.Join(obstacle.transform.DOMoveX(startPos.x, duration / 3 * 2).SetEase(Ease.OutSine));
            seq.Append(obstacle.transform.DOMoveY(startPos.y, duration / 3).SetEase(Ease.InSine));
        }
        else
        {
            Sequence seq = DOTween.Sequence();

            seq.Append(obstacle.transform.DOMoveY(hitPos.y, duration / 2).SetEase(Ease.OutSine));
            seq.Append(obstacle.transform.DOMoveY(startPos.y, duration / 2).SetEase(Ease.InSine));
        }
    }

    public void AnimateZombieNyamNyam(NextStep nextStep, Obstacle obstacle)
    {
        Vector2Int direction = GetDirection(nextStep);

        Vector3 startPos = obstacle.transform.position;
        Quaternion startRot = obstacle.transform.rotation;

        Vector3 offset;
        float angle;

        if (nextStep == NextStep.Left || nextStep == NextStep.Up)
        {
            angle = 45f;
            offset = (nextStep == NextStep.Left) ? new Vector3(-0.5f, 0.5f, 0) : new Vector3(0.5f, 0.5f, 0);
        }
        else
        {
            angle = -45f;
            offset = (nextStep == NextStep.Right) ? new Vector3(0.5f, 0.5f, 0) : new Vector3(-0.5f, 0.5f, 0);
        }

        Vector3 targetPos = startPos + offset;
        float shakeAmount = 0.05f;
        float shakeDuration = 0.1f;

        Sequence seq = DOTween.Sequence();

        // 회전 및 위치 이동
        seq.Append(obstacle.transform.DORotate(new Vector3(0, 0, angle), 0.2f).SetEase(Ease.InOutSine));
        seq.Join(obstacle.transform.DOMove(targetPos, 0.2f).SetEase(Ease.InOutSine));

        // 위아래 흔들기 3번
        for (int i = 0; i < 3; i++)
        {
            seq.Append(obstacle.transform.DOMoveY(targetPos.y + shakeAmount, shakeDuration).SetEase(Ease.InOutSine));
            seq.Append(obstacle.transform.DOMoveY(targetPos.y - shakeAmount, shakeDuration).SetEase(Ease.InOutSine));
        }

        // 원래 회전, 위치 복귀
        seq.Append(obstacle.transform.DORotateQuaternion(startRot, 0.2f).SetEase(Ease.InOutSine));
        seq.Join(obstacle.transform.DOMove(startPos, 0.2f).SetEase(Ease.InOutSine));
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

            
        }
    }

    public bool IsInsideBoard(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < BoardManager.Instance.Board.GetLength(0)
            && pos.y >= 0 && pos.y < BoardManager.Instance.Board.GetLength(1);
    }
}
