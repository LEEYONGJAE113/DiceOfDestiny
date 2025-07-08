using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]


public class SkillManager : Singletone<SkillManager>
{
    // public static SkillManager Instance { get; private set; }
    private bool isSkillActive; // 스킬 이펙트 중 움직임 금지 하기 위한 플래그

    public bool IsSkillActive()
    {
        return isSkillActive;
    }

    // void Awake()
    // {
    //     if (Instance == null)
    //     {
    //         Instance = this;
    //         DontDestroyOnLoad(gameObject);
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //     }
    // }

    public void TryActivateSkill(Vector2Int position, Face topFace)
    {
        if (/*topFace.tileColor == null || */topFace.classData == null)
        {
            Debug.LogError("Top face has invalid data!");
            return;
        }

        // 주변 8칸 중 상단 컬러와 일치하는 칸 수 확인
        int matchCount = BoardManager.Instance.CountMatchingColors(position, topFace.tileColor);
        if (matchCount >= 3)
        {
            ActivateSkill(topFace.classData);
            StartCoroutine(SkillEffectCoroutine());
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
                

                break;
            case "Fanatic":
                Debug.Log("광신도 스킬 발동!");
                

                break;
            case "Knight":
                Debug.Log("기사 스킬 발동!");
                // 실제 구현: 적에게 피해를 주는 로직
                
                break;
            case "Priest":
                Debug.Log("사제 스킬 발동!");
                // 실제 구현: 아군 체력 회복 로직
                break;
            case "Thief":
                Debug.Log("도둑 스킬 발동!");
                

                break;
            case "Wizard":
                Debug.Log("마법사 스킬 발동!");
                

                break;
            default:
                Debug.LogError($"알 수 없는 클래스 : {classData.className}");
                break;
        }
    }

    private IEnumerator SkillEffectCoroutine()
    {
        isSkillActive = true;
        if (PieceManager.Instance == null || PieceManager.Instance.GetPiece() == null)
        {
            Debug.LogError("PieceManager or Piece is null!");
            yield break;
        }

        SpriteRenderer colorRenderer = PieceManager.Instance.GetComponentInChildren<SpriteRenderer>();
        if (colorRenderer == null)
        {
            Debug.LogError("topColorRenderer is null!");
            yield break;
        }

        Color originalColor = colorRenderer.color;
        float duration = 1f;
        float blinkInterval = 0.25f; // 깜빡임 간격 (1초에 4번 깜빡임)
        int blinkCount = Mathf.FloorToInt(duration / blinkInterval);
        float elapsed = 0f;

        for (int i = 0; i < blinkCount; i++)
        {
            // 알파 값을 0.2로 낮춤
            colorRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.2f);
            yield return new WaitForSeconds(blinkInterval / 2);

            // 알파 값을 원래 값(1.0)으로 복원
            colorRenderer.color = originalColor;
            yield return new WaitForSeconds(blinkInterval / 2);

            elapsed += blinkInterval;
        }

        // 정확히 1초가 되도록 남은 시간 대기
        if (elapsed < duration)
        {
            yield return new WaitForSeconds(duration - elapsed);
        }

        // 최종적으로 원래 색상 복원
        colorRenderer.color = originalColor;

        isSkillActive = false;
    }
}