using UnityEngine;

public class PuddleBehaviour : Obstacle, IObstacleBehaviour
{
    public void DoLogic()
    {
        Tile currentTile = BoardManager.Instance.GetTile(obstaclePosition);

        if (currentTile.GetPiece() != null)
        {
            if (currentTile.GetPiece().statusEffectController.IsStatusActive(StatusType.Disease))
                return;

            PieceController currentPiece = currentTile.GetPiece();
            int rand = Random.Range(0, 2);

            if (rand == 0)
            {
                Debug.Log("50%의 확률로 질병을 극복했습니다.");
                ToastManager.Instance.ShowToast("50%의 확률로 질병을 극복했습니다.", currentPiece.transform, 1f);
            }
            else
            {
                Debug.Log("확률 50%로 질병에 걸렸습니다.");
                ToastManager.Instance.ShowToast("확률 50%로 질병에 걸렸습니다.", currentPiece.transform, 1f);

                if (currentTile.GetPiece().GetTopFace().classData.className == "Baby")
                {
                    GoHand(currentTile.GetPiece());
                }

                // 질병 디버프 걸리는 함수 실행
                currentTile.GetPiece().statusEffectController.SetStatus(StatusType.Disease, 2);
            }
        }
    }
}
