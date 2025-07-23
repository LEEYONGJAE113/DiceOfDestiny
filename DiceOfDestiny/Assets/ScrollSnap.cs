using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ScrollSnap : MonoBehaviour, IEndDragHandler, IBeginDragHandler
{
    [Header("필수 컴포넌트")]
    public ScrollRect scrollRect;

    [Header("설정")]
    public int itemCount = 6;            // 아이템 총 개수
    public float snapSpeed = 10f;        // 스냅 애니메이션 속도

    private bool isSnapping = false;
    private Coroutine snapCoroutine = null;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작 시 스냅 코루틴 중단
        if (isSnapping && snapCoroutine != null)
        {
            StopCoroutine(snapCoroutine);
            isSnapping = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isSnapping)
        {
            snapCoroutine = StartCoroutine(SnapToClosest());
        }
    }

    IEnumerator SnapToClosest()
    {
        isSnapping = true;

        // 현재 스크롤 위치 (0~1)
        float currentPos = scrollRect.horizontalNormalizedPosition;

        // 가장 가까운 아이템 인덱스 (0 ~ itemCount-1)
        int targetIndex = Mathf.RoundToInt(currentPos * (itemCount - 1));

        // 목표 위치 normalizedPosition
        float targetPos = (float)targetIndex / (itemCount - 1);

        // 부드럽게 이동
        while (Mathf.Abs(scrollRect.horizontalNormalizedPosition - targetPos) > 0.001f)
        {
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(
                scrollRect.horizontalNormalizedPosition, targetPos, Time.deltaTime * snapSpeed);

            yield return null;
        }

        scrollRect.horizontalNormalizedPosition = targetPos;
        isSnapping = false;

        Debug.Log($"Snap 완료! 현재 아이템 인덱스: {targetIndex}");
    }
}
