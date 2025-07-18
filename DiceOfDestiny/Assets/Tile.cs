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
    
    // 타일 눌렀을 때 호출됨
    private void OnMouseDown()
    {
        Vector2Int position = new Vector2Int(
        Mathf.RoundToInt(transform.position.x - BoardManager.Instance.boardTransform.position.x),
        Mathf.RoundToInt(transform.position.y - BoardManager.Instance.boardTransform.position.y));

        Debug.Log($"Clicked tile at position: {position}");
        BoardSelectManager.Instance.SetClickedTilePosition(position);
        BoardSelectManager.Instance.ClearAllEffects();
    }
}
