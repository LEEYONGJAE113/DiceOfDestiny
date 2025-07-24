using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private TileColor tileColor;
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

    // 타일 눌렀을 때 호출, 장애물 없는 타일이면 BoardSelectManager에 저장해버림
    private void OnMouseDown()
    {
        Vector2Int position = new Vector2Int(
        Mathf.RoundToInt(transform.position.x - BoardManager.Instance.boardTransform.position.x),
        Mathf.RoundToInt(transform.position.y - BoardManager.Instance.boardTransform.position.y));

        if (!BoardManager.Instance.IsEmptyTile(position))
            return; // 장애물 타일이거나 타일 선택 상태가 아니면 클릭 저장 하지마
        BoardSelectManager.Instance.SetClickedTilePosition(position);
        BoardSelectManager.Instance.ClearAllEffects();
    }
}
