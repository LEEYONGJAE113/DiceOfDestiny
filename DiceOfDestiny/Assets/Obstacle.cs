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
    Unicorn,
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
    [SerializeField] private ObstacleType obstacleType;

    private NextStep nextStep;


}
