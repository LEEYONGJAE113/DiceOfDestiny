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
        new Color(1f, 0f, 0f), // »¡°­
        new Color(0f, 1f, 0f), // ÃÊ·Ï
        new Color(0f, 0f, 1f), // ÆÄ¶û
        new Color(1f, 1f, 0f), // ³ë¶û
        new Color(1f, 0f, 1f), // º¸¶ó
        new Color(0.9f, 0.9f, 0.9f) // È¸»ö
    };
    List<int> colorIndices = new List<int>();
    [SerializeField] private int minimumColorEnsure = 14;


    void Start()
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

        SetBoard();
    }

    private void Update()
    {

    }

    private void GenerateRandomColorIndex()
    {
        colorIndices = new List<int>();

        for (int color = 0; color < tileColrs.Length; color++)
        {
            for (int i = 0; i < minimumColorEnsure; i++)
                colorIndices.Add(color);
        }

        for (int i = 0; i < (boardSize * boardSize) - (tileColrs.Length * minimumColorEnsure); i++)
        {
            int randomColor = Random.Range(0, 6); // 0 ~ 5
            colorIndices.Add(randomColor);
        }

        for (int i = colorIndices.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (colorIndices[i], colorIndices[j]) = (colorIndices[j], colorIndices[i]);
        }
    }

    public void SetBoard()
    {
        GenerateRandomColorIndex();
        int idx = 0;
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                Board[x,y].SetTileColor(tileColrs[colorIndices[idx]]);
                switch (colorIndices[idx])
                {
                    case 0: // »¡°­
                        Board[x, y].TileColor = TileColor.Red;
                        break;
                    case 1: // ÃÊ·Ï
                        Board[x, y].TileColor = TileColor.Green;
                        break;
                    case 2: // ÆÄ¶û
                        Board[x, y].TileColor = TileColor.Blue;
                        break;
                    case 3: // ³ë¶û
                        Board[x, y].TileColor = TileColor.Yellow;
                        break;
                    case 4: // º¸¶ó
                        Board[x, y].TileColor = TileColor.Purple;
                        break;
                    case 5: // È¸»ö
                        Board[x, y].TileColor = TileColor.Gray;
                        break;
                }
                idx++;
            }
        }
    }
}
