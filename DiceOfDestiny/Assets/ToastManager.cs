using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToastManager : Singletone<ToastManager>
{
    [SerializeField] private GameObject toastPrefab;
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private float toastDuration = 2f; 
    [SerializeField] private float worldYOffset = 0f; 
    [SerializeField] private float pixelYOffset = 20f;

    private readonly List<RectTransform> activeToasts = new List<RectTransform>();

    private void Awake()
    {
        if (uiCanvas == null)
        {
            uiCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        }

        // Screen-Space–Camera 모드라면 worldCamera 할당
        if (uiCanvas.renderMode != RenderMode.ScreenSpaceOverlay && uiCanvas.worldCamera == null)
            uiCanvas.worldCamera = Camera.main;
    }

    /// <summary>
    /// 기물 위에 토스트를 표시합니다.
    /// </summary>
    /// <param name="message">토스트에 표시할 텍스트</param>
    /// <param name="targetPiece">토스트를 띄울 대상 기물의 Transform</param>
    /// <param name="delay">딜레이(초). 0이면 즉시 표시</param>

    public void ShowToast(string message, Transform targetPiece, float delay = 0f)
    {
        StartCoroutine(ShowToastRoutine(message, targetPiece, delay));
    }

    private System.Collections.IEnumerator ShowToastRoutine(string message, Transform targetPiece, float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        GameObject toastInstance = Instantiate(toastPrefab, uiCanvas.transform);
        RectTransform toastRect = toastInstance.GetComponent<RectTransform>();
        toastInstance.SetActive(true);
        toastRect.pivot = new Vector2(0.5f, 0f);

        activeToasts.Add(toastRect);
        int index = activeToasts.Count - 1;        

        Vector3 worldPos = targetPiece.position + Vector3.up * worldYOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        RectTransform canvasRect = uiCanvas.GetComponent<RectTransform>();

        Camera cam = uiCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : uiCanvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos,cam, out Vector2 localPoint
        );

        float uiYOff = ((pixelYOffset + 20) * index) / uiCanvas.scaleFactor;
        toastRect.anchoredPosition = new Vector2(localPoint.x, localPoint.y + uiYOff);

        var textComponent = toastInstance.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = message;

        yield return new WaitForSeconds(toastDuration);
        activeToasts.Remove(toastRect);
        Destroy(toastInstance);
    }
}
