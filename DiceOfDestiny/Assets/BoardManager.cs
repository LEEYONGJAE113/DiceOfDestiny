using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private int BoardSize = 10;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform boardTransform;
    Tile[,] Board;

    void Start()
    {
        Board = new Tile[BoardSize, BoardSize];

        for (int x = 0; x < BoardSize; x++)
        {
            for (int y = 0; y < BoardSize; y++)
            {
                GameObject tileObject = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity, boardTransform);
                tileObject.name = $"Tile_{x}_{y}";
                Tile tile = tileObject.GetComponent<Tile>();
                tile.TileColor = "Green"; // Example color
                tile.Obstacle = "None"; // Example obstacle
                Board[x, y] = tile;
            }
        }
    }
}
