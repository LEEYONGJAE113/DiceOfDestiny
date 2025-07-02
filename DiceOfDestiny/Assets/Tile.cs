using UnityEngine;

public class Tile : MonoBehaviour
{
    private string tileColor;
    private string obstacle;

    public string TileColor
    {
        get => tileColor;
        set => tileColor = value;
    }

    public string Obstacle
    {
        get => obstacle;
        set => obstacle = value;
    }
}
