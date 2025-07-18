using UnityEngine;
using DG.Tweening;

public class ZombieBehaviour : MonoBehaviour
{
    public void DoZombieLogic(Obstacle zombie)
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
            AnimateObstacleHalfBack(zombie.nextStep, zombie);
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
                AnimateObstacleHalfBack(zombie.nextStep, zombie);
                zombie.nextStep = oppositeStep;
            }
        }
        else
        {
            if (nextTile.GetPiece().GetTopFace().classData.IsCombatClass || nextTile.GetPiece().GetPiece().debuff.IsStun)
            {                
                AnimateObstacleHalfBack(zombie.nextStep, zombie);
                zombie.nextStep = oppositeStep;
                ToastManager.Instance.ShowToast("어림도 없지", nextTile.GetPiece().transform.position + Vector3.up * 1.2f);
            }
            else
            {
                AnimateZombieNyamNyam(zombie.nextStep, zombie);
                zombie.nextStep = oppositeStep;

                Debug.Log("Piece SStun!");                
                nextTile.GetPiece().GetPiece().debuff.SetStun(true, 2);
                ToastManager.Instance.ShowToast("좀비한테 물려버렸습니다! 1턴간 기절합니다.", nextTile.GetPiece().transform.position + Vector3.up * 1.2f);
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

    public void AnimateObstacleHalfBack(NextStep nextStep, Obstacle obstacle)
    {
        Vector2Int direction = GetDirection(nextStep);

        Vector3 startPos = obstacle.transform.position;
        Vector3 targetPos = startPos + new Vector3(direction.x, direction.y, 0);

        float duration = 0.6f;
        float jumpHeight = 0.2f;

        float ratio = 0.7f;
        Vector3 hitPos = Vector3.Lerp(startPos, targetPos, ratio);

        if (direction.x != 0) // 좌우 이동은 점프 효과
        {
            Sequence seq = DOTween.Sequence();

            // 1) X축 이동 (duration 전체)
            seq.Append(obstacle.transform.DOMoveX(hitPos.x, duration / 3).SetEase(Ease.InSine));
            // 2) Y축 점프 (올라갔다 내려오기) - duration 전체, Y만 움직임
            seq.Append(obstacle.transform.DOMoveY(startPos.y + jumpHeight, duration / 3).SetEase(Ease.OutSine));
            seq.Join(obstacle.transform.DOMoveX(startPos.x, duration / 3 * 2).SetEase(Ease.OutSine));
            seq.Append(obstacle.transform.DOMoveY(startPos.y, duration / 3).SetEase(Ease.InSine));
        }
        else
        {
            Sequence seq = DOTween.Sequence();

            seq.Append(obstacle.transform.DOMoveY(hitPos.y, duration / 2).SetEase(Ease.OutSine));
            seq.Append(obstacle.transform.DOMoveY(startPos.y, duration / 2).SetEase(Ease.InSine));
        }
    }

    public void AnimateZombieNyamNyam(NextStep nextStep, Obstacle obstacle)
    {
        Vector2Int direction = GetDirection(nextStep);

        Vector3 startPos = obstacle.transform.position;
        Quaternion startRot = obstacle.transform.rotation;

        Vector3 offset;
        float angle;

        if (nextStep == NextStep.Left || nextStep == NextStep.Up)
        {
            angle = 45f;
            offset = (nextStep == NextStep.Left) ? new Vector3(-0.5f, 0.5f, 0) : new Vector3(0.5f, 0.5f, 0);
        }
        else
        {
            angle = -45f;
            offset = (nextStep == NextStep.Right) ? new Vector3(0.5f, 0.5f, 0) : new Vector3(-0.5f, 0.5f, 0);
        }

        Vector3 targetPos = startPos + offset;
        float shakeAmount = 0.05f;
        float shakeDuration = 0.1f;

        Sequence seq = DOTween.Sequence();

        // 회전 및 위치 이동
        seq.Append(obstacle.transform.DORotate(new Vector3(0, 0, angle), 0.2f).SetEase(Ease.InOutSine));
        seq.Join(obstacle.transform.DOMove(targetPos, 0.2f).SetEase(Ease.InOutSine));

        // 위아래 흔들기 3번
        for (int i = 0; i < 3; i++)
        {
            seq.Append(obstacle.transform.DOMoveY(targetPos.y + shakeAmount, shakeDuration).SetEase(Ease.InOutSine));
            seq.Append(obstacle.transform.DOMoveY(targetPos.y - shakeAmount, shakeDuration).SetEase(Ease.InOutSine));
        }

        // 원래 회전, 위치 복귀
        seq.Append(obstacle.transform.DORotateQuaternion(startRot, 0.2f).SetEase(Ease.InOutSine));
        seq.Join(obstacle.transform.DOMove(startPos, 0.2f).SetEase(Ease.InOutSine));
    }
}
