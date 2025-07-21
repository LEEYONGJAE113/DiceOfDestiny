using UnityEngine;
using DG.Tweening;

public class SlimeBehaviour : MonoBehaviour
{
    public void DoSlimeLogic(Obstacle zombie)
    {
        if (zombie.nextStep == NextStep.None)
        {
            zombie.nextStep = Random.Range(0, 2) == 1 ? NextStep.Left : NextStep.Right;
        }

        Vector2Int direction = GetDirection(zombie.nextStep);
        NextStep oppositeStep = GetOppositeStep(zombie.nextStep);

        Vector2Int nextPosition = zombie.obstaclePosition + direction;
        Tile nextTile = BoardManager.Instance.GetTile(nextPosition);

        if (nextTile == null)
        {
            //AnimateObstacleHalfBack(zombie.nextStep, zombie);
            zombie.nextStep = oppositeStep;
            return;
        }

        if (nextTile.GetPiece() == null)
        {
            if (nextTile.Obstacle == ObstacleType.None)
            {
                BoardManager.Instance.MoveObstacle(zombie, nextPosition);
                AnimateObstacleMove(zombie.nextStep, zombie);
            }
            else
            {
                //AnimateObstacleHalfBack(zombie.nextStep, zombie);
                zombie.nextStep = oppositeStep;
            }
        }
        else
        {
            if (nextTile.GetPiece().GetTopFace().classData.IsCombatClass || nextTile.GetPiece().statusEffectController.IsStatusActive(StatusType.Stun))
            {
                //AnimateObstacleHalfBack(zombie.nextStep, zombie);
                zombie.nextStep = oppositeStep;
            }
            else
            {
                //AnimateZombieNyamNyam(zombie.nextStep, zombie);
                zombie.nextStep = oppositeStep;

                if (nextTile.GetPiece().GetTopFace().classData.className == "Priest")
                {
                    Debug.Log("사제는 기절을 무시합니다.");
                    return;
                }
                Debug.Log("Piece SStun!");
                nextTile.GetPiece().statusEffectController.SetStatus(StatusType.Stun, 2);
            }
        }
    }

    private Vector2Int GetDirection(NextStep step)
    {
        return step switch
        {
            NextStep.Right => Vector2Int.right,
            NextStep.Left => Vector2Int.left,
            NextStep.Up => Vector2Int.up,
            NextStep.Down => Vector2Int.down,
            _ => Vector2Int.zero
        };
    }

    private NextStep GetOppositeStep(NextStep step)
    {
        return step switch
        {
            NextStep.Right => NextStep.Left,
            NextStep.Left => NextStep.Right,
            NextStep.Up => NextStep.Down,
            NextStep.Down => NextStep.Up,
            _ => NextStep.None
        };
    }

    public void AnimateObstacleMove(NextStep nextStep, Obstacle obstacle)
    {
        Vector2Int direction = GetDirection(nextStep);

        Vector3 startPos = obstacle.transform.position;
        Vector3 targetPos = startPos + new Vector3(direction.x, direction.y, 0);

        float duration = 0.4f;
        float jumpHeight = 0.2f;

        if (direction.x != 0) // 좌우 이동은 점프 효과
        {
            Sequence seq = DOTween.Sequence();

            // 1) X축 이동 (duration 전체)
            seq.Append(obstacle.transform.DOMoveX(targetPos.x, duration).SetEase(Ease.InOutSine));

            // 2) Y축 점프 (올라갔다 내려오기) - duration 전체, Y만 움직임
            seq.Join(obstacle.transform.DOMoveY(startPos.y + jumpHeight, duration / 2).SetEase(Ease.OutSine));
            seq.Append(obstacle.transform.DOMoveY(startPos.y, duration / 2).SetEase(Ease.InSine));

        }
        else // 상하 이동은 자연스러운 이동
        {
            obstacle.transform.DOMove(targetPos, duration).SetEase(Ease.InOutSine);
        }
    }


}
