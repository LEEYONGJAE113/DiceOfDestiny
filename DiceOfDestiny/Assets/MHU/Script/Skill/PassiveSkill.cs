using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSkill : MonoBehaviour
{
    [SerializeField] private GameObject knightPassiveEffect;
    [SerializeField] private float knightPassiveEffectDuration = 0.5f;

    public IEnumerator KnightPassiveSkill(PieceController pieceController)
    {
        if (pieceController == null || knightPassiveEffect == null)
        {
            Debug.LogWarning("PieceController or knightPassiveEffect is null.");
            yield break;
        }

        // 상하좌우에 좀비 또는 고블린이 있는지 확인
        if (BoardManager.Instance.HasObstacleCardinal(pieceController.gridPosition))
        {
            // 이펙트 객체를 저장할 리스트
            List<GameObject> skillEffects = new List<GameObject>();

            // 상하좌우 방향 벡터와 각 방향에 맞는 회전 각도
            (Vector2Int direction, float rotationZ)[] directions = new[]
            {
                (new Vector2Int(0, 1), 0f),   // 상 
                (new Vector2Int(0, -1), 180f), // 하
                (new Vector2Int(-1, 0), 90f),  // 좌
                (new Vector2Int(1, 0), -90f)   // 우
            };

            ToastManager.Instance.ShowToast("기사 패시브 발동! 주변 4방향을 공격합니다.", pieceController.transform);

            foreach (var (dir, rotationZ) in directions)
            {
                Vector2Int targetPos = pieceController.gridPosition + dir;

                {                    
                    // 이펙트 위치
                    Vector3 effectPos = pieceController.transform.position;

                    // 이펙트 생성 (방향에 맞게 회전)
                    Quaternion rotation = Quaternion.Euler(0f, 0f, rotationZ);
                    GameObject skillEffect = Instantiate(
                        knightPassiveEffect,
                        effectPos,
                        rotation
                    );

                    // 이펙트 리스트에 추가
                    skillEffects.Add(skillEffect);

                    // 장애물 제거
                    BoardManager.Instance.RemoveObstacleAtPosition(targetPos);
                }
            }

            // 이펙트 지속 시간 대기
            yield return new WaitForSeconds(knightPassiveEffectDuration);

            // 모든 이펙트 제거
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
}