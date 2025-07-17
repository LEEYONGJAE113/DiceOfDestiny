using UnityEngine;

public class Tile : MonoBehaviour
{
    private TileColor tileColor;
    private ObstacleType obstacle;
    private PieceController piece;
    public bool isWalkable { get; set; }

    SpriteRenderer sr;

    public TileColor TileColor
    {
        get => tileColor;
        set => tileColor = value;
    }

    public ObstacleType Obstacle
    {
        get => obstacle;
        set => obstacle = value;
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetTileColor(Color color)
    {
        sr.color = color;
    }

    public PieceController GetPiece()
    {
        return piece;
    }

    public void SetPiece(PieceController newPiece)
    {
        piece = newPiece;
    }
}
