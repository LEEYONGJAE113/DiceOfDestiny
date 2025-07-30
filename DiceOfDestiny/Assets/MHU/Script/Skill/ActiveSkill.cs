using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActiveSkill : MonoBehaviour
{
    [SerializeField] private GameObject knightSkillEffect;
    [SerializeField] private GameObject demonSkillEffect;
    [SerializeField] private GameObject painterSkillEffect;
    [SerializeField] private GameObject fanaticSkillEffect;
    [SerializeField] private GameObject priestSkillEffect;

    [SerializeField] private PainterActiveSkillUI painterActiveSkillUI;

    // 기사 스킬: 앞으로 이동
    public IEnumerator MoveForward(PieceController pieceController, Vector2Int moveDirection)
    {
        yield return new WaitForSeconds(SkillManager.Instance.blinkTime + 0.1f);

        if (moveDirection != Vector2Int.up && moveDirection != Vector2Int.down &&
            moveDirection != Vector2Int.right && moveDirection != Vector2Int.left)
        {
            Debug.LogWarning($"Invalid move direction: {moveDirection}");
            yield break;
        }

        Vector3 moveVec = new Vector3(moveDirection.x, moveDirection.y, 0);
        float moveDuration = 0.4f;
        float time = 0f;

        Vector3 startPos = pieceController.transform.position;
        Vector3 endPos = startPos + moveVec;

        GameObject skillEffect = null;
        if (knightSkillEffect != null)
        {
            skillEffect = Instantiate(knightSkillEffect, startPos, Quaternion.identity);
            skillEffect.transform.SetParent(pieceController.transform);

            if (moveDirection == Vector2Int.left)
            {
                skillEffect.transform.localScale = new Vector3(1f, 1f, 1f);
                skillEffect.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else if (moveDirection == Vector2Int.right)
            {
                skillEffect.transform.localScale = new Vector3(-1f, 1f, 1f);
                skillEffect.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else if (moveDirection == Vector2Int.up)
            {
                skillEffect.transform.localScale = new Vector3(1f, 1f, 1f);
                skillEffect.transform.localRotation = Quaternion.Euler(0f, 0f, -120f);
            }
            else if (moveDirection == Vector2Int.down)
            {
                skillEffect.transform.localScale = new Vector3(1f, 1f, 1f);
                skillEffect.transform.localRotation = Quaternion.Euler(0f, 0f, 60f);
            }
        }
        else
        {
            Debug.LogWarning("Skill effect prefab is not assigned!");
        }

        while (time < moveDuration)
        {
            float t = time / moveDuration;
            float ease = Mathf.SmoothStep(0f, 1f, t);
            pieceController.transform.position = Vector3.Lerp(startPos, endPos, ease);
            time += Time.deltaTime;
            yield return null;
        }

        pieceController.transform.position = endPos;
        Vector2Int gridPos = pieceController.gridPosition;
        gridPos += moveDirection;
        pieceController.gridPosition = gridPos;

        bool hasObstacle = BoardManager.Instance.IsEmptyTile(gridPos);

        if (!hasObstacle)
        {
            BoardManager.Instance.RemoveObstacleAtPosition(gridPos);
        }

        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.TrySkill(gridPos, pieceController);
        }
        else
        {
            Debug.LogError("SkillManager.Instance is null!");
        }

        if (skillEffect != null)
        {
            Destroy(skillEffect, 0.5f);
        }
    }

    // 악마 스킬: 독초 심기
    public IEnumerator Plant(PieceController pieceController)
    {
        yield return new WaitForSeconds(SkillManager.Instance.blinkTime + 0.1f);

        BoardSelectManager.Instance.HighlightTiles();
        yield return BoardSelectManager.Instance.WaitForTileClick();

        SkillManager.Instance.IsSelectingProgress = true;
        Vector2Int gridPos = BoardSelectManager.Instance.lastClickedPosition;

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

        yield return new WaitForSeconds(0.5f);
        BoardManager.Instance.CreateObstacle(gridPos, ObstacleType.PoisonousHerb);
        SkillManager.Instance.IsSelectingProgress = false;
    }

    // 화가 스킬: 색칠하기
    public IEnumerator Paint(PieceController pieceController)
    {
        yield return new WaitForSeconds(SkillManager.Instance.blinkTime + 0.1f);

        BoardSelectManager.Instance.AllHighlightTiles();
        yield return BoardSelectManager.Instance.WaitForTileClick();

        SkillManager.Instance.IsSelectingProgress = true;
        Vector2Int gridPos = BoardSelectManager.Instance.lastClickedPosition;

        if (painterActiveSkillUI != null)
        {
            painterActiveSkillUI.OnDisable();
            painterActiveSkillUI.ShowPalette();
            while (painterActiveSkillUI.SelectedColor == TileColor.None)
            {
                yield return null;
            }

            TileColor selectedColor = painterActiveSkillUI.SelectedColor;

            if (painterSkillEffect != null)
            {
                Vector2Int selectPos = BoardSelectManager.Instance.lastClickedPosition;
                Vector3 effectPosition = new Vector3(
                    selectPos.x - 5.5f,
                    selectPos.y - 5.8f,
                    0f
                );

                GameObject effect = Instantiate(
                    painterSkillEffect,
                    effectPosition,
                    Quaternion.identity
                );
                Destroy(effect, 1f);
            }
            else
            {
                Debug.LogWarning("PainterSkillEffect is not assigned!");
            }

            yield return new WaitForSeconds(0.5f);
            BoardManager.Instance.SetTileColor(gridPos, selectedColor);
            SkillManager.Instance.IsSelectingProgress = false;
        }
        else
        {
            Debug.LogWarning("PainterActiveSkillUI is not assigned!");
        }
    }

    public IEnumerator ConvertToFanatic(PieceController piece)
    {
        yield return new WaitForSeconds(SkillManager.Instance.blinkTime + 0.1f);

        List<Vector2Int> surroundList = BoardManager.Instance.GetTilePositions(DirectionType.Eight, piece.gridPosition);

        bool converted = false;
        foreach (PieceController targetPiece in PieceManager.Instance.Pieces)
        {
            if (targetPiece == null || targetPiece == piece) continue;

            if (surroundList.Contains(targetPiece.gridPosition))
            {
                for (int i = 0; i < 6; i++)
                {
                    Face face = targetPiece.GetFace(i);
                    if (face.classData.className == "Priest")
                    {
                       
                        targetPiece.ChangeClass(i, "Fanatic");
                        Debug.Log($"Converted Priest to Fanatic on face {i} at position {targetPiece.gridPosition}");
                        converted = true;

                        if (fanaticSkillEffect != null)
                        {
                            GameObject effect = Instantiate(
                                fanaticSkillEffect,
                                new Vector3(
                                    BoardManager.Instance.boardTransform.position.x + targetPiece.gridPosition.x,
                                    BoardManager.Instance.boardTransform.position.y + targetPiece.gridPosition.y,
                                    -1),
                                Quaternion.identity,
                                BoardManager.Instance.boardTransform
                            );
                            Destroy(effect, 0.5f);
                        }
                    }
                }
            }
        }

        if (!converted)
        {
            ToastManager.Instance.ShowToast("주변에 사제가 없어 아무 일도 일어나지 않았습니다.", piece.transform);
        }
        else
        {
            
            //ToastManager.Instance.ShowToast("성공", piece.transform);
        }
    }

    // 사제 스킬
    public IEnumerator HealAP()
    {
        yield return new WaitForSeconds(SkillManager.Instance.blinkTime + 0.1f);

        if (priestSkillEffect != null)
        {
            GameObject effect = Instantiate(
                priestSkillEffect,
                new Vector3(
                    BoardManager.Instance.boardTransform.position.x + PieceManager.Instance.currentPiece.gridPosition.x,
                    BoardManager.Instance.boardTransform.position.y + PieceManager.Instance.currentPiece.gridPosition.y,
                    -1),
                Quaternion.identity,
                BoardManager.Instance.boardTransform
            );
            Destroy(effect, 0.5f);
        }
        else
        {
            Debug.LogWarning("PriestSkillEffect is not assigned!");

        }
    }


    public IEnumerator MoveToBaby(PieceController piece)
    {
        yield return new WaitForSeconds(SkillManager.Instance.blinkTime + 0.1f);
    }
}