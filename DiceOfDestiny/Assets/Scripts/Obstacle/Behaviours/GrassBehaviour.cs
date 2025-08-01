using UnityEngine;

public class GrassBehaviour : Obstacle, IObstacleBehaviour
{
    public void DoLogic()
    {
        Tile currentTile = BoardManager.Instance.GetTile(this.obstaclePosition);

        if (currentTile.GetPiece() != null)
        {
            if (currentTile.GetPiece().GetTopFace().classData.className == "Baby")
            {
                Debug.Log("아기는 풀을 밟지 못 합니다.");
                ToastManager.Instance.ShowToast("아기는 풀을 밟지 못 합니다.", currentTile.GetPiece().transform, 1f);
                return;
            }

            if (currentTile.GetPiece().statusEffectController.IsStatusActive(StatusType.Disease))
                return;

            int rand = Random.Range(0, 10);

            if (rand > 0)
            {
                Debug.Log("90%의 확률로 질병을 극복했습니다.");
                ToastManager.Instance.ShowToast("90%의 확률로 질병을 극복했습니다.", currentTile.GetPiece().transform, 1f);
            }
            else
            {
                Debug.Log("10%의 확률로 질병에 걸렸습니다.");
                ToastManager.Instance.ShowToast("10%의 확률로 질병에 걸렸습니다.", currentTile.GetPiece().transform, 1f);
                if (currentTile.GetPiece().GetTopFace().classData.className == "Baby")
                {
                    GoHand(currentTile.GetPiece());
                }

                // 질병 디버프 걸리는 함수 실행
                currentTile.GetPiece().statusEffectController.SetStatus(StatusType.Disease, 2);
            }


            BoardManager.Instance.RemoveObstacle(this);
        }
    }

}
