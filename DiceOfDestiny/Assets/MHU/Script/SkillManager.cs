using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillManager : Singletone<SkillManager>
{
    [SerializeField] public float blinkTime = 1.5f;
    [SerializeField] private PieceController pieceController;
    [SerializeField] private PieceActiveSkill pieceActiveSkill;



    public void TryActivateSkill(Vector2Int position, PieceController piece)
    {
        // 주변 8칸 중 상단 컬러와 일치하는 칸 수 확인
        int matchCount = BoardManager.Instance.CountMatchingColors(position, piece.GetTopFace().color);
        if (matchCount >= 3)
        {
            ActivateSkill(piece.GetTopFace().classData);
            List<Vector2Int> matchingTile = BoardManager.Instance.GetMatchingColorTiles(position, piece.GetTopFace().color);
            StartCoroutine(SkillEffectCoroutine(piece.colorRenderer, position, matchingTile));
            StartCoroutine(BoardReassign(piece, position));
        }
        else
        {
            //Debug.Log($"Not enough matching colors ({matchCount}/3) to activate skill.");
        }
    }

    private void ActivateSkill(ClassData classData)
    {
        switch (classData.className)
        {
            case "Baby":
                Debug.Log("아기 스킬 발동!");

                break;
            case "Demon":
                Debug.Log("악마 스킬 발동!");
                DemonActiveSkill();

                break;
            case "Fanatic":
                Debug.Log("광신도 스킬 발동!");

                break;
            case "Knight":
                Debug.Log("기사 스킬 발동!");

                KnightActiveSkill();

                break;
            case "Priest":
                Debug.Log("사제 스킬 발동!");

                PriestActiveSkill();

                break;
            case "Thief":
                Debug.Log("도둑 스킬 발동!");

                ThiefActiveSkill();

                break;
            case "Painter":
                Debug.Log("화가 스킬 발동!");
                PainterActiveSkill();

                break;
            default:
                Debug.LogError($"알 수 없는 클래스 : {classData.className}");
                break;
        }
    }

    private void PriestActiveSkill()
    {
        GameManager.Instance.actionPointManager.AddAP(1);
    }


    private void ThiefActiveSkill()
    {
        // 도둑 스킬 : 원하는 방향으로 1칸 움직임, 컨트롤러 한번 더 띄움
    }

    private void KnightActiveSkill()
    {
        // 기사 스킬 : 진행했던 방향으로 1칸 움직임, 다 부숨
        Vector2Int lastDirection = pieceController.GetLastMoveDirection();

        pieceActiveSkill.MoveForward(pieceController,lastDirection);

    }
    private void DemonActiveSkill()
    {
        // 악마 스킬 : 원하는 보드 한칸에 독초 장애물을 만듬
        pieceActiveSkill.Plant(pieceController);

    }

    private void PainterActiveSkill()
    {
        // 화가 스킬: 원하는 보드 한칸에 색깔을 칠함
        pieceActiveSkill.Paint(pieceController);
    }

    #region 스킬 발동 시 깜빡임, 보드 색상 재배치 코루틴

    private IEnumerator SkillEffectCoroutine(SpriteRenderer pieceRenderer, Vector2Int position, List<Vector2Int> matchingTiles)
    {
        if (PieceManager.Instance == null /*|| PieceManager.Instance.GetPiece() == null*/)
        {
            Debug.LogError("PieceManager or Piece is null!");
            yield break;
        }

        // 스킬이 발동된 타일과 매칭된 타일들의 SpriteRenderer 수집
        List<(SpriteRenderer renderer, Color originalColor)> renderers = new List<(SpriteRenderer, Color)>();

        // 피스의 SpriteRenderer 추가
        if (pieceRenderer != null)
        {
            renderers.Add((pieceRenderer, pieceRenderer.color));
        }
        else
        {
            Debug.LogError("Piece SpriteRenderer is null!");
        }

        // 스킬이 발동된 타일의 SpriteRenderer 추가
        if (position.x >= 0 && position.x < BoardManager.Instance.boardSize &&
            position.y >= 0 && position.y < BoardManager.Instance.boardSize &&
            BoardManager.Instance.Board[position.x, position.y] != null)
        {
            SpriteRenderer tileRenderer = BoardManager.Instance.Board[position.x, position.y].GetComponent<SpriteRenderer>();
            if (tileRenderer != null)
            {
                renderers.Add((tileRenderer, tileRenderer.color));
            }
            else
            {
                Debug.LogError($"SpriteRenderer is null for tile at {position}");
            }
        }
        else
        {
            Debug.LogError("Invalid tile position or tile is null!");
        }

        // 매칭된 타일들의 SpriteRenderer 추가
        foreach (Vector2Int tilePos in matchingTiles)
        {
            if (tilePos.x >= 0 && tilePos.x < BoardManager.Instance.boardSize &&
                tilePos.y >= 0 && tilePos.y < BoardManager.Instance.boardSize &&
                BoardManager.Instance.Board[tilePos.x, tilePos.y] != null)
            {
                SpriteRenderer tileRenderer = BoardManager.Instance.Board[tilePos.x, tilePos.y].GetComponent<SpriteRenderer>();
                if (tileRenderer != null)
                {
                    renderers.Add((tileRenderer, tileRenderer.color));
                }
                else
                {
                    Debug.LogError($"SpriteRenderer is null for tile at {tilePos}");
                }
            }
            else
            {
                Debug.LogError($"Invalid position or null tile at {tilePos}");
            }
        }

        // 깜빡임 효과
        float blinkInterval = 0.25f; // 1초에 4번 깜빡임
        int blinkCount = Mathf.FloorToInt(blinkTime / blinkInterval);
        float elapsed = 0f;

        for (int i = 0; i < blinkCount; i++)
        {
            // 모든 SpriteRenderer를 검정색으로 변경
            foreach (var (renderer, originalColor) in renderers)
            {
                renderer.color = Color.black;
            }
            yield return new WaitForSeconds(blinkInterval / 2);

            // 모든 SpriteRenderer를 원래 색상으로 복원
            foreach (var (renderer, originalColor) in renderers)
            {
                renderer.color = originalColor;
            }
            yield return new WaitForSeconds(blinkInterval / 2);

            elapsed += blinkInterval;
        }

        // 정확히 1초가 되도록 남은 시간 대기
        if (elapsed < blinkTime)
        {
            yield return new WaitForSeconds(blinkTime - elapsed);
        }

        // 최종적으로 모든 SpriteRenderer를 원래 색상으로 복원
        foreach (var (renderer, originalColor) in renderers)
        {
            renderer.color = originalColor;
        }
    }
    IEnumerator BoardReassign(PieceController piece, Vector2Int position)
    {
        yield return new WaitForSeconds(0.1f + blinkTime);
        BoardManager.Instance.ReassignMatchingColorTiles(position, piece.GetTopFace().color);
        // 기물 움직일 수 있게
    }

    #endregion


}