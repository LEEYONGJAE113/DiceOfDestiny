using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToastManager : Singletone<ToastManager>
{
    [SerializeField] private GameObject toastPrefab;   // Toast 프리팹
    [SerializeField] private Canvas uiCanvas;          // UI용 Canvas
    [SerializeField] private float toastDuration = 2f; // 토스트 표시 시간


    private void Awake()
    {
        if (uiCanvas == null)
        {
            uiCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        }
    }

    /// <summary>
    /// 화면 중앙 상단에 토스트 메시지를 표시합니다.
    /// </summary>
    public void ShowToast(string message, Vector3? worldPos)
    {
        var toastInstance = Instantiate(toastPrefab, uiCanvas.transform);
        var textComponent = toastInstance.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = message;

        if (worldPos.HasValue)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos.Value);

            RectTransform canvasRect = uiCanvas.GetComponent<RectTransform>();

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPos,
                uiCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : uiCanvas.worldCamera,
                out localPoint
            );

            RectTransform toastRect = toastInstance.GetComponent<RectTransform>();
            toastRect.anchoredPosition = localPoint;
        }

        StartCoroutine(HideAfterDelay(toastInstance));
    }

    private System.Collections.IEnumerator HideAfterDelay(GameObject toast)
    {
        yield return new WaitForSeconds(toastDuration);
        Destroy(toast);
    }
}
