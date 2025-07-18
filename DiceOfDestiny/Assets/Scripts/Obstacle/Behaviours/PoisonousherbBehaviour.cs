using UnityEngine;

public class PoisonousherbBehaviour : MonoBehaviour
{
    public void DoPoisionousherbLogic(Obstacle herb)
    {
        Tile currentTile = BoardManager.Instance.GetTile(herb.obstaclePosition);
        if (currentTile.GetPiece() != null)
        {
            PieceController currentPiece = currentTile.GetPiece();
            if (currentPiece.GetTopFace().classData.className == "Demon")
            {
                Debug.Log("악마가 독초를 밟아 행동력 +1");                
                ToastManager.Instance.ShowToast("독초를 밟아 행동력 +1", currentPiece.transform.position + Vector3.up * 1.2f);
                GameManager.Instance.actionPointManager.AddAP(1);
                return;
            }
            Debug.Log("독초를 밟아 행동력 -1");
            GameManager.Instance.actionPointManager.RemoveAP(1);
        }
    }
}
