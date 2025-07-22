using UnityEngine;

public class PuddleBehaviour : MonoBehaviour
{
    public void DoPuddleLogic(Obstacle puddle)
    {
        Tile currentTile = BoardManager.Instance.GetTile(puddle.obstaclePosition);

        if (currentTile.GetPiece() != null)
        {
            if (currentTile.GetPiece().statusEffectController.IsStatusActive(StatusType.Disease))
                return;

            PieceController currentPiece = currentTile.GetPiece();
            int rand = Random.Range(0, 2);

            if (rand == 0)
            {
                Debug.Log("확률 50%로 질병을 극복했습니다.");
                ToastManager.Instance.ShowToast("확률 50%로 질병을 극복했습니다.", currentPiece.transform, 1f);
            }
            else
            {
                Debug.Log("확률 50%로 질병에 걸렸습니다.");
                ToastManager.Instance.ShowToast("확률 50%로 질병에 걸렸습니다.", currentPiece.transform, 1f);
                // 질병 디버프 걸리는 함수 실행
                currentTile.GetPiece().statusEffectController.SetStatus(StatusType.Disease, 2);
            }
        }
    }

    public void DoGrassLogic(Obstacle grass)
    {
        Tile currentTile = BoardManager.Instance.GetTile(grass.obstaclePosition);

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
                // 질병 디버프 걸리는 함수 실행
                currentTile.GetPiece().statusEffectController.SetStatus(StatusType.Disease, 2);
            }

            if (currentTile.GetPiece().GetTopFace().classData.className == "Baby")
            {
                Debug.Log("아기는 풀을 못밟습니다.");
                return;
            }

            BoardManager.Instance.RemoveObstacle(grass);
        }
    }
}
