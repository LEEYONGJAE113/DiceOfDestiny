using UnityEngine;

public class PoisonousherbBehaviour : Obstacle, IObstacleBehaviour
{
    public void DoLogic()
    {
        Tile currentTile = BoardManager.Instance.GetTile(obstaclePosition);
        if (currentTile.GetPiece() != null)
        {
            if (currentTile.GetPiece().GetTopFace().classData.className == "Priest")
            {
                Debug.Log("저주를 무시합니다.");
                BoardManager.Instance.RemoveObstacle(this);
                return;
            }

            var point = 1;
            PieceController currentPiece = currentTile.GetPiece();
            if (currentPiece.GetTopFace().classData.className == "Demon")
            {
                GameManager.Instance.actionPointManager.AddAP(point);
                Debug.Log($"악마가 독초를 밟아 행동력 +{point}");
                ToastManager.Instance.ShowToast($"독초를 밟아 {point} 행동력을 얻었습니다.", currentPiece.transform, 1f);
                return;
            }

            GameManager.Instance.actionPointManager.RemoveAP(point);
            Debug.Log($"독초를 밟아 행동력 -{point}");
            ToastManager.Instance.ShowToast($"독초를 밟아 {point} 행동력을 잃었습니다.", currentPiece.transform, 1f);

            if (currentTile.GetPiece().GetTopFace().classData.className == "Baby")
            {
                Debug.Log("아기는 독초를 못밟습니다.");
                return;
            }

            BoardManager.Instance.RemoveObstacle(this);
        }
    }
}
