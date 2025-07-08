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
            
            if(obstacleComponent.obstacleType == ObstacleType.Zombie)
            {
                if(obstacleComponent.nextStep == NextStep.None)
                {        
                    obstacleComponent.nextStep = Random.Range(0, 2) == 1 ? NextStep.Left : NextStep.Right;
                }
                
                if(obstacleComponent.nextStep == NextStep.Right)
                {
                    Vector2Int nextPosition = obstacleComponent.obstaclePosition + Vector2Int.right;
                    if (BoardManager.Instance.IsEmptyTile(nextPosition))
                    {
                        BoardManager.Instance.MoveObstacle(obstacleComponent, nextPosition);
                    }
                    else
                    {
                        obstacleComponent.nextStep = NextStep.Left; // 다음에는 왼쪽으로 이동
                    }
                }
                else // if(obstacleComponent.nextStep == NextStep.Left)
                {
                    Vector2Int nextPosition = obstacleComponent.obstaclePosition + Vector2Int.left;
                    if (BoardManager.Instance.IsEmptyTile(nextPosition))
                    {
                        BoardManager.Instance.MoveObstacle(obstacleComponent, nextPosition);
                    }
                    else
                    {
                        obstacleComponent.nextStep = NextStep.Right; // 다음에는 왼쪽으로 이동
                    }
                }
            }
        }
    }
}
