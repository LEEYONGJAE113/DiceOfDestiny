using System.Collections.Generic;
using UnityEngine;

public enum TileColor
{
    Red,
    Green,
    Blue,
    Yellow,
    Purple,
    Gray,
    None
}

public class BoardManager : Singletone<BoardManager>
{
    // public static BoardManager Instance { get; private set; }

    // private void Awake()
    // {
    //     if (Instance == null)
    //     {
    //         Instance = this;
    //         DontDestroyOnLoad(gameObject);
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //     }
    // }

    [Header("Board Size Settings")]
    [SerializeField] private int boardSize = 11;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform boardTransform;
    public Tile[,] Board { get; set; }

    [Header("Tile Colors Settings")]
    [SerializeField]
    private Color[] tileColors = new Color[] {
        new Color(1f, 0f, 0f), // ����
        new Color(0f, 1f, 0f), // �ʷ�
        new Color(0f, 0f, 1f), // �Ķ�
        new Color(1f, 1f, 0f), // ���
        new Color(1f, 0f, 1f), // ����
        new Color(0.9f, 0.9f, 0.9f) // ȸ��
    };

    List<int> colorIndices = new List<int>();
    List<ObstacleType> obstacleIndices = new List<ObstacleType>();


    void Start()
    {
        GenerateBoard();
    }

    private void Update()
    {

    }

    private void GenerateBoard()
    {
        Board = new Tile[boardSize, boardSize];

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                GameObject tileObject = Instantiate(tilePrefab, new Vector3(boardTransform.position.x + x, boardTransform.position.y + y, 0), Quaternion.identity, boardTransform);
                tileObject.name = $"Tile_{x}_{y}";
                Tile tile = tileObject.GetComponent<Tile>();
                tile.TileColor = TileColor.None;
                tile.Obstacle = ObstacleType.None;
                Board[x, y] = tile;
            }
        }
    }

    public void SetBoard(StageDifficultyProfile profile)
    {
        // ����ġ�� �°� ���� �����ϴ� �κ�
        colorIndices = new List<int>();
        for (int color = 0; color < tileColors.Length; color++)
        {
            for (int i = 0; i < profile.minimumColorEnsure; i++)
                colorIndices.Add(color);
        }

        int[] colorWeights = new int[]
        {
        profile.redWeight * profile.weightPower,
        profile.greenWeight * profile.weightPower,
        profile.blueWeight * profile.weightPower,
        profile.yellowWeight * profile.weightPower,
        profile.purpleWeight * profile.weightPower,
        profile.orangeWeight * profile.weightPower
        };

        int[] cumulativeWeights = new int[colorWeights.Length];
        cumulativeWeights[0] = colorWeights[0];

        for (int i = 1; i < colorWeights.Length; i++)
            cumulativeWeights[i] = cumulativeWeights[i - 1] + colorWeights[i];

        int total = cumulativeWeights[cumulativeWeights.Length - 1];


        for (int i = 0; i < (boardSize * boardSize) - (tileColors.Length * profile.minimumColorEnsure); i++)
        {
            int rand = Random.Range(0, total);
            for (int j = 0; j < cumulativeWeights.Length; j++)
            {
                if (rand < cumulativeWeights[j])
                {
                    colorIndices.Add(j);
                    break;
                }
            }
        }

        for (int i = colorIndices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (colorIndices[i], colorIndices[j]) = (colorIndices[j], colorIndices[i]);
        }

        // ��ĥ�ϴ� �κ�
        int idx = 0;
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                Board[x, y].SetTileColor(tileColors[colorIndices[idx]]);
                switch (colorIndices[idx])
                {
                    case 0: // ����
                        Board[x, y].TileColor = TileColor.Red;
                        break;
                    case 1: // �ʷ�
                        Board[x, y].TileColor = TileColor.Green;
                        break;
                    case 2: // �Ķ�
                        Board[x, y].TileColor = TileColor.Blue;
                        break;
                    case 3: // ���
                        Board[x, y].TileColor = TileColor.Yellow;
                        break;
                    case 4: // ����
                        Board[x, y].TileColor = TileColor.Purple;
                        break;
                    case 5: // ȸ��
                        Board[x, y].TileColor = TileColor.Gray;
                        break;
                }
                idx++;
            }
        }


        // ��ֹ� ��ġ �κ�
        obstacleIndices = new List<ObstacleType>();
        int tileCount = boardSize * boardSize;
        int obstacleCount = Mathf.RoundToInt(tileCount * profile.obstacleDensity);
        for (int i = 0; i < tileCount - obstacleCount; i++) // ��ֹ��� ���� Ÿ�� �߰�
        {
            obstacleIndices.Add(ObstacleType.None);
        }

        List<ObstacleType> availableObstacleWeight = new List<ObstacleType>();
        for (int i = 0; i < profile.availableObstacle.Count; i++) // ��ֹ� Ÿ���� �ε����� �߰�
        {
            for (int j = 0; j < profile.availableObstacle[i].weight * 10; j++)
            {
                availableObstacleWeight.Add(profile.availableObstacle[i].type);
            }
        }

        for (int i = 0; i < obstacleCount; i++) // ��ֹ��� �ִ� Ÿ��
        {
            int randIndex = Random.Range(0, availableObstacleWeight.Count);
            obstacleIndices.Add(availableObstacleWeight[randIndex]);
        }

        for (int i = obstacleIndices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (obstacleIndices[i], obstacleIndices[j]) = (obstacleIndices[j], obstacleIndices[i]);
        }

        idx = 0;
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                Board[x, y].Obstacle = obstacleIndices[idx];
                // ��ֹ� ����
                if (obstacleIndices[idx] != ObstacleType.None)
                {
                    GameObject obstacle = Instantiate(ObstacleManager.Instance.obstaclePrefabs[obstacleIndices[idx]],
                        new Vector3(boardTransform.position.x + x, boardTransform.position.y + y, 0), Quaternion.identity, boardTransform);
                    obstacle.GetComponent<Obstacle>().obstaclePosition = new Vector2Int(x, y);
                    ObstacleManager.Instance.SetObstacle(obstacle);
                }
                idx++;
            }
        }
    }



    public Obstacle ReturnObstacleByPosition(Vector2Int position)
    {
        if (position.x < 0 || position.x >= boardSize || position.y < 0 || position.y >= boardSize)
        {
            Debug.LogError("Position out of bounds");
            return null;
        }
        Tile tile = Board[position.x, position.y];
        if (tile.Obstacle == ObstacleType.None)
        {
            return null;
        }
        foreach (GameObject obstacle in ObstacleManager.Instance.currentObstacles)
        {
            if (obstacle.GetComponent<Obstacle>().obstaclePosition == position)
            {
                return obstacle.GetComponent<Obstacle>();
            }
        }
        return null;
    }

    public bool IsEmptyTile(Vector2Int position)
    {
        if (position.x < 0 || position.x >= boardSize || position.y < 0 || position.y >= boardSize)
        {
            // Debug.LogError("Position out of bounds");
            return false;
        }
        return Board[position.x, position.y].Obstacle == ObstacleType.None;
    }

    public void MoveObstacle(Obstacle obstacle, Vector2Int nextPos)
    {
        Vector2Int currentPos = obstacle.obstaclePosition;
        Board[currentPos.x, currentPos.y].Obstacle = ObstacleType.None; // ���� Ÿ���� ��ֹ� ����
        Board[nextPos.x, nextPos.y].Obstacle = obstacle.obstacleType; // ���� Ÿ�Ͽ� ��ֹ� ����
        obstacle.obstaclePosition = nextPos; // ��ֹ��� ��ġ ������Ʈ
        obstacle.transform.position = new Vector3(boardTransform.position.x + nextPos.x, boardTransform.position.y + nextPos.y, 0); // ��ֹ� ��ġ �̵�
    }
    
    public int CountMatchingColors(Vector2Int position, TileColor targetColor)
    {
        // if (targetColor == null)
        // {
        //     Debug.LogError("Target ColorData is null!");
        //     return 0;
        // }

        int matchCount = 0;
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(-1, -1), // 좌상
            new Vector2Int(-1, 0),  // 좌
            new Vector2Int(-1, 1),  // 좌하
            new Vector2Int(0, -1),  // 상
            new Vector2Int(0, 1),   // 하
            new Vector2Int(1, -1),  // 우상
            new Vector2Int(1, 0),   // 우
            new Vector2Int(1, 1)    // 우하
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int checkPos = position + dir;
            if (checkPos.x >= 0 && checkPos.x < boardSize &&
                checkPos.y >= 0 && checkPos.y < boardSize)
            {
                if (Board[checkPos.x, checkPos.y] != null &&
                    Board[checkPos.x, checkPos.y].TileColor == targetColor)
                {
                    // 체크된게 어느 타일인지 보여주면 굿.
                    matchCount++;
                }
            }
        }

        return matchCount;
    }
}
