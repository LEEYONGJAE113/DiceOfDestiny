using UnityEngine;

public class PoisonousherbBehaviour : MonoBehaviour
{
    public void DoPoisionousherbLogic(Obstacle herb)
    {
        Tile currentTile = BoardManager.Instance.GetTile(herb.obstaclePosition);
        if (currentTile.GetPiece() != null)
        {
            if (currentTile.GetPiece().GetTopFace().classData.className == "Priest")
            {
                Debug.Log("저주를 무시합니다.");
                BoardManager.Instance.RemoveObstacle(herb);
                return;
            }
            if (currentTile.GetPiece().GetTopFace().classData.className == "Demon")
            {
                Debug.Log("악마가 독초를 밟아 행동력 +1");
                GameManager.Instance.actionPointManager.AddAP(1);
                return;
            }

            Debug.Log("독초를 밟아 행동력 -1");
            GameManager.Instance.actionPointManager.RemoveAP(1);

            if (currentTile.GetPiece().GetTopFace().classData.className == "Baby")
            {
                Debug.Log("아기는 독초를 못밟습니다.");
                return;
            }

            BoardManager.Instance.RemoveObstacle(herb);
        }
    }
}
