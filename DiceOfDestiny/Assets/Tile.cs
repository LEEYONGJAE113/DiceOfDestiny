using UnityEngine;

public class Tile : MonoBehaviour
{
    private TileColor tileColor;
    private ObstacleType obstacle;

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
}
