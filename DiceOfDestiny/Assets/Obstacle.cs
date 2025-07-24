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
}
