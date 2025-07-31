using UnityEngine;

public class GrassBehaviour : Obstacle, IObstacleBehaviour
{
    public void DoLogic()
    {
        Tile currentTile = BoardManager.Instance.GetTile(this.obstaclePosition);

        if (currentTile.GetPiece() != null)
        {
            if (currentTile.GetPiece().statusEffectController.IsStatusActive(StatusType.Disease))
                return;

            int rand = Random.Range(0, 10);

            if (rand > 0)
            {
                Debug.Log("90퍼 확률로 질병을 극복했습니다.");
            }
            else
            {
                Debug.Log("10퍼 확률로 질병에 걸렸습니다.");
                if (currentTile.GetPiece().GetTopFace().classData.className == "Baby")
                {
                    GoHand(currentTile.GetPiece());
                }

                // 질병 디버프 걸리는 함수 실행
                currentTile.GetPiece().statusEffectController.SetStatus(StatusType.Disease, 2);
            }

            if (currentTile.GetPiece().GetTopFace().classData.className == "Baby")
            {
                Debug.Log("아기는 풀을 못밟습니다.");
                return;
            }

            BoardManager.Instance.RemoveObstacle(this);
        }
    }

}
