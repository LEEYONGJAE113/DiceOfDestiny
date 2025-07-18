using UnityEngine;

public class PuddleBehaviour : MonoBehaviour
{
    public void DoPuddleLogic(Obstacle puddle)
    {
        Tile currentTile = BoardManager.Instance.GetTile(puddle.obstaclePosition);
        if (currentTile.GetPiece() != null)
        {
            PieceController currentPiece = currentTile.GetPiece();
            int rand = Random.Range(0, 2);

            if (rand == 0)
            {
                Debug.Log("50% 확률로 질병을 극복했습니다.");
                ToastManager.Instance.ShowToast("50퍼 확률로 질병을 극복했습니다.", currentPiece.transform.position + Vector3.up * 1.2f);
            }
            else
            {
                Debug.Log("50% 확률로 질병에 걸렸습니다.");
                ToastManager.Instance.ShowToast("50% 확률로 질병에 걸렸습니다.", currentPiece.transform.position + Vector3.up * 1.2f);
                // 질병 디버프 걸리는 함수 실행
            }
            // 해당 장애물 오브젝트 삭제 함수 실행
        }
    }
    
}
