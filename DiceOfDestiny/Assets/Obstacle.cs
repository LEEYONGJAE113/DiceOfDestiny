using UnityEngine;

public enum ObstacleType
{
    Zombie,
    Tree,
    Rock,
    Lion,
    Puddle,
    Chest,
    ManaSpring,
    Goblin,
    PoisonousHerb,
    Grass,
    Slime,
    SlimeDdong,
    None
}

public enum NextStep
{
    Up,
    Down,
    Left,
    Right,
    None
}

public class Obstacle : MonoBehaviour
{
    public ObstacleType obstacleType;

    public NextStep nextStep = NextStep.None;
    public Vector2Int obstaclePosition;

    public bool isWalkable;

    public SpriteRenderer spriteRenderer { get; private set; }
    public Animator animator { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    protected void GoHand(PieceController pieceController)
    {
        // 가방에 새로운 기물 생성
        for (int i = 0; i < 3; i++)
        {
            if (!PieceManager.Instance.pieceInventory.slots[i].IsActivePiece())
            {
                PieceManager.Instance.pieceInventory.slots[i].AddPiece(pieceController.GetPiece());
                EventManager.Instance.TriggerEvent("Refresh");
                break;
            }
        }

        // 기존 보드의 기물 제거
        Destroy(pieceController.gameObject);

        // 기물 선택 타일 제거
        BoardSelectManager.Instance.DestroyPieceHighlightTile();
    }
}
