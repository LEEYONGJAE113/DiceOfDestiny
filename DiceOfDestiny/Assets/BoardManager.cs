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

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }

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

    [Header("Board Size Settings")]
    [SerializeField] private int boardSize = 11;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform boardTransform;
    public Tile[,] Board { get; set; }

    [Header("Tile Colors Settings")]
    [SerializeField]
    private Color[] tileColrs = new Color[] {
        new Color(1f, 0f, 0f), // ����
        new Color(0f, 1f, 0f), // �ʷ�
        new Color(0f, 0f, 1f), // �Ķ�
        new Color(1f, 1f, 0f), // ���
        new Color(1f, 0f, 1f), // ����
        new Color(0.9f, 0.9f, 0.9f) // ȸ��
    };

    List<int> colorIndices = new List<int>();


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
        for (int color = 0; color < tileColrs.Length; color++)
        {
            for (int i = 0; i < profile.minimumColorEnsure; i++)
                colorIndices.Add(color);
        }

        int[] weights = new int[]
        {
        profile.redWeight * profile.weightPower,
        profile.greenWeight * profile.weightPower,
        profile.blueWeight * profile.weightPower,
        profile.yellowWeight * profile.weightPower,
        profile.purpleWeight * profile.weightPower,
        profile.orangeWeight * profile.weightPower
        };

        int[] cumulativeWeights = new int[weights.Length];
        cumulativeWeights[0] = weights[0];

        for (int i = 1; i < weights.Length; i++)
            cumulativeWeights[i] = cumulativeWeights[i - 1] + weights[i];

        int total = cumulativeWeights[cumulativeWeights.Length - 1];


        for (int i = 0; i < (boardSize * boardSize) - (tileColrs.Length * profile.minimumColorEnsure); i++)
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
                Board[x, y].SetTileColor(tileColrs[colorIndices[idx]]);
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
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (Random.Range(0f, 1f) < profile.obstacleWeight)
                {
                    int obstacleIndex = Random.Range(0, profile.availableObstacle.Count);
                    Board[x, y].Obstacle = profile.availableObstacle[obstacleIndex];
                    // ��ֹ� ����
                    GameObject Obstacle = Instantiate(ObstacleManager.Instance.obstaclePrefabs[profile.availableObstacle[obstacleIndex]],
                        new Vector3(boardTransform.position.x + x, boardTransform.position.y + y, 0), Quaternion.identity, boardTransform);
                    ObstacleManager.Instance.SetObstacle(Obstacle);
                }
                else
                {
                    Board[x, y].Obstacle = ObstacleType.None;
                }
            }
        }

    }
}
