using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PieceActiveSkill : MonoBehaviour
{
    [SerializeField] private GameObject knightSkillEffect;
    [SerializeField] private GameObject demonSkillEffect;
    [SerializeField] private GameObject painterSkillEffect;
    [SerializeField] private PainterActiveSkillUI painterActiveSkillUI;

    public void MoveForward(PieceController pieceController, Vector2Int moveDirection)
    {
        StartCoroutine(MoveForwardCoroutine(pieceController, moveDirection));
    }

    private IEnumerator MoveForwardCoroutine(PieceController pieceController, Vector2Int moveDirection)
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

        Vector3 startPos = pieceController.transform.position;
        Vector3 endPos = startPos + moveVec;

        // 스킬 이펙트 생성
        GameObject skillEffect = null;
        if (knightSkillEffect != null)
        {
            // 이펙트를 시작 위치에 생성
            skillEffect = Instantiate(knightSkillEffect, startPos, Quaternion.identity);
            // 이펙트가 캐릭터를 따라가도록 부모로 설정
            skillEffect.transform.SetParent(pieceController.transform);

            // 방향에 따라 이펙트 조정 (기본: 왼쪽)
            if (moveDirection == Vector2Int.left)
            {
                // 왼쪽: 기본 방향이므로 변경 없음
                skillEffect.transform.localScale = new Vector3(1f, 1f, 1f);
                skillEffect.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else if (moveDirection == Vector2Int.right)
            {
                // 오른쪽: x축 스케일 반전
                skillEffect.transform.localScale = new Vector3(-1f, 1f, 1f);
                skillEffect.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else if (moveDirection == Vector2Int.up)
            {
                // 위쪽: -90도 회전 (왼쪽에서 위로)
                skillEffect.transform.localScale = new Vector3(1f, 1f, 1f);
                skillEffect.transform.localRotation = Quaternion.Euler(0f, 0f, -120f);
            }
            else if (moveDirection == Vector2Int.down)
            {
                // 아래쪽: 90도 회전 (왼쪽에서 아래로)
                skillEffect.transform.localScale = new Vector3(1f, 1f, 1f);
                skillEffect.transform.localRotation = Quaternion.Euler(0f, 0f, 60f);
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
            pieceController.transform.position = Vector3.Lerp(startPos, endPos, ease);
            time += Time.deltaTime;
            yield return null;
        }

        // 실제 그리드 위치 설정
        pieceController.transform.position = endPos;
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
            Destroy(skillEffect, 0.5f); // 0.5초 후 제거
        }
    }

    // 악마 스킬: 독초 심기
    public void Plant(PieceController pieceController)
    {
        StartCoroutine(PlantCoroutine(pieceController));
    }

    IEnumerator PlantCoroutine(PieceController pieceController)
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
        if (demonSkillEffect != null)
        {
            GameObject effect = Instantiate(
                demonSkillEffect,
                new Vector3(
                    BoardManager.Instance.boardTransform.position.x + gridPos.x,
                    BoardManager.Instance.boardTransform.position.y + gridPos.y,
                    -1),
                Quaternion.identity,
                BoardManager.Instance.boardTransform
            );
            Destroy(effect, 0.5f);
        }
        else
        {
            Debug.LogWarning("DemonSkillEffect is not assigned!");
        }

        // 0.5초 대기
        yield return new WaitForSeconds(0.5f);

        // 장애물 설정
        BoardManager.Instance.CreateObstacle(gridPos, ObstacleType.PoisonousHerb);
    }

    // 화가 스킬: 색칠하기
    public void Paint(PieceController pieceController)
    {
        StartCoroutine(PaintCoroutine(pieceController));
    }

    IEnumerator PaintCoroutine(PieceController pieceController)
    {
        // 깜빡임 대기
        yield return new WaitForSeconds(SkillManager.Instance.blinkTime + 0.1f);

        // 타일 선택 이미지 띄우기
        BoardSelectManager.Instance.AllHighlightTiles();

        // 클릭 기다림
        yield return BoardSelectManager.Instance.WaitForTileClick();

        // 위치 불러오기
        Vector2Int gridPos = BoardSelectManager.Instance.lastClickedPosition;

        // 화가 스킬 UI 표시
        if (painterActiveSkillUI != null)
        {

            painterActiveSkillUI.OnDisable(); // ui 초기화
            painterActiveSkillUI.ShowPalette();
            // UI에서 색상 선택을 기다림
            while (painterActiveSkillUI.SelectedColor == TileColor.None)
            {
                yield return null; // 색상이 선택될 때까지 대기
            }
            
            // 선택된 색상 가져오기
            TileColor selectedColor = painterActiveSkillUI.SelectedColor;

            // 이펙트 생성
            if (painterSkillEffect != null)
            {
                GameObject effect = Instantiate(
                    painterSkillEffect,
                    pieceController.transform.position, // pieceController의 위치 사용
                    Quaternion.identity
                );
                Destroy(effect, 0.5f);
            }
            else
            {
                Debug.LogWarning("PainterSkillEffect is not assigned!");
            }


            // 0.5초 대기
            yield return new WaitForSeconds(0.5f);

            BoardManager.Instance.SetTileColor(gridPos, selectedColor);

        }
        else
        {
            Debug.LogWarning("PainterActiveSkillUI is not assigned!");
        }
    }
}