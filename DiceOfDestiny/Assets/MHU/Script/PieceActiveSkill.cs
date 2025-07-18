using UnityEngine;
using System.Collections;

public class PieceActiveSkill : MonoBehaviour
{
    [SerializeField] private PieceController pieceController;
    [SerializeField] private GameObject knightSkillEffect;
    [SerializeField] private GameObject DemonSkillEffect;

    public void MoveForward(Vector2Int moveDirection)
    {
        StartCoroutine(MoveForwardCoroutine(moveDirection));
    }

    private IEnumerator MoveForwardCoroutine(Vector2Int moveDirection)
    {
        // 깜빡임 대기
        yield return new WaitForSeconds(SkillManager.Instance.blinkTime + 0.1f);

        // 유효한 이동 방향인지 확인
        if (moveDirection != Vector2Int.up && moveDirection != Vector2Int.down &&
            moveDirection != Vector2Int.right && moveDirection != Vector2Int.left)
        {
            Debug.LogWarning($"Invalid move direction: {moveDirection}");
            yield break;
        }

        // 마지막 방향으로 이동 애니메이션
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
                // 왼쪽: 기본 방향
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
                // 위쪽: -90도 회전
                skillEffect.transform.localScale = new Vector3(1f, 1f, 1f);
                skillEffect.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
            }
            else if (moveDirection == Vector2Int.down)
            {
                // 아래쪽: 90도 회전
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

        // 실제 그리드 위치 설정
        transform.position = endPos;
        Vector2Int gridPos = pieceController.gridPosition;
        gridPos += moveDirection;
        pieceController.gridPosition = gridPos;

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

    public void Plant()
    {
        StartCoroutine(PlantCoroutine());
    }

    IEnumerator PlantCoroutine()
    {
        // 깜빡임 대기
        yield return new WaitForSeconds(SkillManager.Instance.blinkTime + 0.1f);

        // 타일 선택 이미지 띄우기
        BoardSelectManager.Instance.HighlightTiles();

        // 클릭 기다림
        yield return BoardSelectManager.Instance.WaitForTileClick();

        // 위치 불러오기
        Vector2Int gridPos = BoardSelectManager.Instance.lastClickedPosition;

        // 이펙트 생성
        if (DemonSkillEffect != null)
        {
            GameObject effect = Instantiate(
                DemonSkillEffect,
                new Vector3(
                    BoardManager.Instance.boardTransform.position.x + gridPos.x,
                    BoardManager.Instance.boardTransform.position.y + gridPos.y,
                    -1), // z=-1로 타일 위에 렌더링
                Quaternion.identity,
                BoardManager.Instance.boardTransform
            );
            // 0.5초 후 이펙트 삭제
            Destroy(effect, 0.5f);
        }
        else
        {
            Debug.LogWarning("Effect prefab is not assigned!");
        }

        // 1초 대기
        yield return new WaitForSeconds(0.5f);

        // 장애물 설정
        BoardManager.Instance.CreateObstacle(gridPos, ObstacleType.PoisonousHerb);
    }
}