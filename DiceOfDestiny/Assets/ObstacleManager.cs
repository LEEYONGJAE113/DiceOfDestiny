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

    ZombieBehaviour zombieBehaviour;
    PoisonousherbBehaviour poisonousherbBehaviour;

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
                zombieBehaviour.DoZombieLogic(obstacleComponent);
            }
            else if (obstacleComponent.obstacleType == ObstacleType.PoisonousHerb) // 독초
            {
                poisonousherbBehaviour.DoPoisionousherbLogic(obstacleComponent);
            }
        }
    }

    














    // 미사용 ===============================================================================================================














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
