using UnityEngine;
using System.Collections;

public class PieceActiveSkill : MonoBehaviour
{
    [SerializeField] private PieceController pieceController;
    [SerializeField] private GameObject knightSkillEffect; // 이펙트 프리팹 참조

    public void MoveForward(Vector2Int moveDirection)
    {
        StartCoroutine(MoveForwardCoroutine(moveDirection));
    }

    private IEnumerator MoveForwardCoroutine(Vector2Int moveDirection)
    {
        yield return new WaitForSeconds(SkillManager.Instance.blinkTime + 0.1f); // 잠시 대기

        // 유효한 이동 방향인지 확인
        if (moveDirection != Vector2Int.up && moveDirection != Vector2Int.down &&
            moveDirection != Vector2Int.right && moveDirection != Vector2Int.left)
        {
            Debug.LogWarning($"Invalid move direction: {moveDirection}");
            yield break;
        }

        // 1단계: 주어진 방향으로 이동
        Vector3 moveVec = new Vector3(moveDirection.x, moveDirection.y, 0);
        float moveDuration = 0.4f; // 이동 시간
        float time = 0f;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + moveVec;

        // 스킬 이펙트 생성
        GameObject skillEffect = null;
        if (knightSkillEffect != null)
        {
            skillEffect = Instantiate(knightSkillEffect, startPos, Quaternion.identity);
            // 이펙트가 캐릭터를 따라가도록 설정
            skillEffect.transform.SetParent(transform);

            // 방향에 따라 이펙트 조정
            if (moveDirection == Vector2Int.left)
            {
                // 기본 방향 (왼쪽): 아무 작업도 하지 않음
                skillEffect.transform.localScale = new Vector3(1f, 1f, 1f);
                skillEffect.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else if (moveDirection == Vector2Int.right)
            {
                // 오른쪽: X축 플립
                skillEffect.transform.localScale = new Vector3(-1f, 1f, 1f);
                skillEffect.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else if (moveDirection == Vector2Int.up)
            {
                // 위쪽: 90도 회전
                skillEffect.transform.localScale = new Vector3(1f, 1f, 1f);
                skillEffect.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
            }
            else if (moveDirection == Vector2Int.down)
            {
                // 아래쪽: -90도 회전
                skillEffect.transform.localScale = new Vector3(1f, 1f, 1f);
                skillEffect.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            }
        }
        else
        {
            Debug.LogWarning("Skill effect prefab is not assigned!");
        }

        // 부드러운 이동 애니메이션
        while (time < moveDuration)
        {
            float t = time / moveDuration;
            float ease = Mathf.SmoothStep(0f, 1f, t);
            transform.position = Vector3.Lerp(startPos, endPos, ease);
            time += Time.deltaTime;
            yield return null;
        }

        // 최종 위치 설정
        transform.position = endPos;
        Vector2Int gridPos = pieceController.GetGridPosition();
        gridPos += moveDirection;

        // 이동한 위치에 장애물 확인
        bool hasObstacle = BoardManager.Instance.IsEmptyTile(gridPos);

        if (!hasObstacle)
        {
            // 이동한 위치에서 장애물 제거
            BoardManager.Instance.RemoveObstacleAtPosition(gridPos);
        }

        // 이동한 위치에서 스킬 발동
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.TryActivateSkill(gridPos, pieceController);
        }
        else
        {
            Debug.LogError("SkillManager.Instance is null!");
        }

        // 스킬 이펙트 제거
        if (skillEffect != null)
        {
            Destroy(skillEffect, 0.5f); // 0.5초 후 제거 (이펙트 지속 시간에 맞게 조정)
        }
    }
}