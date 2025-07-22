using UnityEngine;

public class PoisonousherbBehaviour : MonoBehaviour
{
    public void DoPoisionousherbLogic(Obstacle herb)
    {
        Tile currentTile = BoardManager.Instance.GetTile(herb.obstaclePosition);
        if (currentTile.GetPiece() != null)
        {
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
        }
    }
}
