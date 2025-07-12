using System.Collections;
using UnityEngine;

public class FourSideRollingDice : MonoBehaviour
{
    [Header("Sprite Renderers")]
    public SpriteRenderer upperRenderer;
    public SpriteRenderer lowerRenderer;

    [Header("Dice Sides")]
    public Sprite[] sideSprites; // 4개 스프라이트 필요

    [Header("Animation Settings")]
    public float duration = 0.8f; // 한 사이클 애니메이션 지속 시간
    public float inflateAmount = 0.4f; // 부풀림 크기

    [Header("Roll Settings")]
    public int rollCount = 6; // 몇 번 굴릴지

    [Header("Movement Settings")]
    public Vector3 startPosition = Vector3.zero;
    public Vector3 endPosition = new Vector3(-3f, 3f, 0f); // 좌상단 예시
    // 속도 대신 위치 기반으로 보간 처리

    private int currentIndex = 0; // 현재 위쪽 면 인덱스

    private void Start()
    {
        if (sideSprites == null || sideSprites.Length < 4)
        {
            Debug.LogError("sideSprites 배열에 최소 4개의 스프라이트를 넣어주세요.");
            enabled = false;
            return;
        }

        transform.localPosition = startPosition;

        // 초기 스프라이트 설정
        upperRenderer.sprite = sideSprites[currentIndex];
        lowerRenderer.sprite = sideSprites[GetNextIndex(currentIndex)];

        StartCoroutine(RollLoop());
    }

    IEnumerator RollLoop()
    {
        for (int i = 0; i < rollCount; i++)
        {
            // 현재 진행률 (0~1) — 얼마나 굴렀는지 비율
            float progress = (float)i / (rollCount - 1);

            // start ~ end 위치를 보간해서 이번 애니메이션 시작 위치로 설정
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, progress);

            yield return RollCycle();

            // 면 교체
            currentIndex = GetNextIndex(currentIndex);
            upperRenderer.sprite = sideSprites[currentIndex];
            lowerRenderer.sprite = sideSprites[GetNextIndex(currentIndex)];
        }

        // 마지막 위치 정확하게 맞춤
        transform.localPosition = endPosition;

        Debug.Log("주사위 굴리기 완료!");
    }

    int GetNextIndex(int index)
    {
        return (index + 1) % sideSprites.Length;
    }

    IEnumerator RollCycle()
    {
        Vector3 moveVec = Vector3.right;

        upperRenderer.transform.localPosition = Vector3.zero;
        lowerRenderer.transform.localPosition = Vector3.zero;

        upperRenderer.transform.localScale = Vector3.one;
        lowerRenderer.transform.localScale = Vector3.zero;

        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            float ease = Mathf.SmoothStep(0f, 1f, t);

            float inflate = Mathf.Sin(ease * Mathf.PI) * inflateAmount;
            float totalScale = 1f + inflate;

            float scaleUpper = (1f - ease) * totalScale;
            float scaleLower = ease * totalScale;

            upperRenderer.transform.localScale = new Vector3(scaleUpper, 1f, 1f);
            lowerRenderer.transform.localScale = new Vector3(scaleLower, 1f, 1f);

            float upperWidth = upperRenderer.sprite.bounds.size.x * scaleUpper;
            float lowerWidth = lowerRenderer.sprite.bounds.size.x * scaleLower;

            float distance = (upperWidth / 2f) + (lowerWidth / 2f);

            Vector3 upperPos = moveVec * distance * 0.5f;
            Vector3 lowerPos = -moveVec * distance * 0.5f;

            Vector3 extraOffset = moveVec * 0.5f * t;

            upperRenderer.transform.localPosition = upperPos + extraOffset;
            lowerRenderer.transform.localPosition = lowerPos + extraOffset;

            time += Time.deltaTime;
            yield return null;
        }
    }
}
