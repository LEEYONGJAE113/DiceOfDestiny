using UnityEngine;

public enum ObstacleType
{
    Zombie,
    Tree,
    Rock,
    Lion,
    Pond,
    Box,
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
