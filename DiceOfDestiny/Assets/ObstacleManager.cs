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

            if (obstacleComponent.obstacleType == ObstacleType.Zombie)
            {
                if (obstacleComponent.nextStep == NextStep.None)
                {
                    obstacleComponent.nextStep = Random.Range(0, 2) == 1 ? NextStep.Left : NextStep.Right;
                }

                if (obstacleComponent.nextStep == NextStep.Right)
                {
                    MoveObstacle(obstacleComponent, Vector2Int.right, NextStep.Left);
                }
                else // if(obstacleComponent.nextStep == NextStep.Left)
                {
                    MoveObstacle(obstacleComponent, Vector2Int.left, NextStep.Right);
                }

            }
        }
    }

    private void MoveObstacle(Obstacle _obstacleComponent, Vector2Int _vector2Int, NextStep _nextStep)
    {
        Vector2Int nextPosition = _obstacleComponent.obstaclePosition + _vector2Int;

        if (BoardManager.Instance.Board[nextPosition.x, nextPosition.y].Obstacle != ObstacleType.None)
        {
            nextPosition = _obstacleComponent.obstaclePosition;
            return;
        }

        if (BoardManager.Instance.IsEmptyTile(nextPosition))
        {
            BoardManager.Instance.MoveObstacle(_obstacleComponent, nextPosition);
        }
        else
        {
            _obstacleComponent.nextStep = _nextStep; // �������� �������� �̵�
        }
    }
}
