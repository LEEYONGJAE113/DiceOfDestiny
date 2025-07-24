using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSkill : MonoBehaviour
{
    [SerializeField] private GameObject knightPassiveEffect;
    [SerializeField] private GameObject demonPassiveEffect;
    [SerializeField] private float knightPassiveEffectDuration = 0.5f;
    [SerializeField] private float demonPassiveEffectDuration = 0.5f;

    public IEnumerator KnightPassiveSkill(PieceController pieceController)
    {
        if (pieceController == null || knightPassiveEffect == null)
        {
            Debug.LogWarning("PieceController or knightPassiveEffect is null.");
            yield break;
        }

        List<Vector2Int> cardinalList = BoardManager.Instance.GetTilePositions(DirectionType.Four, PieceManager.Instance.GetCurrentPiece().gridPosition);

        bool hasTarget = false;
        for (int i = 0; i < cardinalList.Count; i++)
        {
            var obstacle = BoardManager.Instance.ReturnObstacleByPosition(cardinalList[i]);
            if (obstacle != null &&
                (obstacle.obstacleType == ObstacleType.Slime || obstacle.obstacleType == ObstacleType.Zombie))
            {
                hasTarget = true;
                Debug.Log($"기사가 공격 대상 찾았어: ({cardinalList[i].x}, {cardinalList[i].y})");
                break;
            }
        }

        if (hasTarget)
        {
            List<GameObject> skillEffects = new List<GameObject>();
            (Vector2Int direction, float rotationZ)[] directions = new[]
            {
                (new Vector2Int(0, 1), 0f),   // 상 
                (new Vector2Int(0, -1), 180f), // 하
                (new Vector2Int(-1, 0), 90f),  // 좌
                (new Vector2Int(1, 0), -90f)   // 우
            };

            foreach (var (dir, rotationZ) in directions)
            {
                Vector2Int targetPos = pieceController.gridPosition + dir;
                Vector3 effectPos = pieceController.transform.position;
                Quaternion rotation = Quaternion.Euler(0f, 0f, rotationZ);
                GameObject skillEffect = Instantiate(
                    knightPassiveEffect,
                    effectPos,
                    rotation
                );
                skillEffects.Add(skillEffect);
                var targetObstacle = BoardManager.Instance.ReturnObstacleByPosition(targetPos);
                if (targetObstacle != null &&
                    (targetObstacle.obstacleType == ObstacleType.Slime || targetObstacle.obstacleType == ObstacleType.Zombie))
                {
                    BoardManager.Instance.RemoveObstacleAtPosition(targetPos);
                   
                }
            }

            yield return new WaitForSeconds(knightPassiveEffectDuration);
            foreach (var skillEffect in skillEffects)
            {
                if (skillEffect != null)
                {
                    Destroy(skillEffect);
                }
            }
        }
        else
        {
            yield return null;
        }
    }

    public IEnumerator DemonPassiveSkill(PieceController pieceController)
    {
        if (pieceController == null || demonPassiveEffect == null)
        {
            Debug.LogWarning("PieceController or devilPassiveEffect is null.");
            yield break;
        }

        // 전방 3칸 타일 위치 가져오기 (전방 1칸 + 좌우 대각선 1칸)
        List<Vector2Int> forwardList = BoardManager.Instance.GetTilePositions(DirectionType.ForwardThree, pieceController.gridPosition);

        bool hasTarget = false;
        for (int i = 0; i < forwardList.Count; i++)
        {
            var obstacle = BoardManager.Instance.ReturnObstacleByPosition(forwardList[i]);
            if (obstacle != null &&
                (obstacle.obstacleType == ObstacleType.Slime || obstacle.obstacleType == ObstacleType.Zombie))
            {
                hasTarget = true;
                
                break;
            }
        }

        // 공격 대상이 있는 경우
        if (hasTarget)
        {
            List<GameObject> skillEffects = new List<GameObject>();
            Vector2Int lastMoveDirection = PieceManager.Instance.currentPiece.GetLastMoveDirection();

            // 전방 3칸에 대해 이펙트 3개 생성, 위치는 pieceController.transform.position
            for (int i = 0; i < forwardList.Count; i++)
            {
                Vector3 effectPos = pieceController.transform.position;

                // 회전 각도 설정
                float rotationZ;
                if (i == 0) // 전방 1칸
                    rotationZ = 0f; // 0도
                else if (i == 1) // 좌 대각선
                    rotationZ = -60f; // -60도
                else // 우 대각선
                    rotationZ = 60f; // 60도

                // 이동 방향에 따라 회전 각도 조정
                if (lastMoveDirection == new Vector2Int(0, -1)) // 하
                    rotationZ += 180f;
                else if (lastMoveDirection == new Vector2Int(-1, 0)) // 좌
                    rotationZ += 90f;
                else if (lastMoveDirection == new Vector2Int(1, 0)) // 우
                    rotationZ += -90f;

                Quaternion rotation = Quaternion.Euler(0f, 0f, rotationZ);
                GameObject skillEffect = Instantiate(
                    demonPassiveEffect,
                    effectPos,
                    rotation
                );
                skillEffects.Add(skillEffect);


                var targetObstacle = BoardManager.Instance.ReturnObstacleByPosition(forwardList[i]);
                if (targetObstacle != null &&
                    (targetObstacle.obstacleType == ObstacleType.Slime || targetObstacle.obstacleType == ObstacleType.Zombie))
                {
                    BoardManager.Instance.RemoveObstacleAtPosition(forwardList[i]);
                    Debug.Log($"장애물 제거됨: ({forwardList[i].x}, {forwardList[i].y})");
                }

            }
            // 이펙트 지속 시간 대기
            yield return new WaitForSeconds(demonPassiveEffectDuration);

            // 모든 이펙트 제거
            foreach (var skillEffect in skillEffects)
            {
                if (skillEffect != null)
                {
                    Destroy(skillEffect);
                }
            }

            if (!hasTarget)
            {
                yield return null;
            }
        }
    }
}