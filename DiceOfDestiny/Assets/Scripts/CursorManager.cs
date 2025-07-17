using UnityEngine;
using System.Collections.Generic;

public class CursorManager : Singletone<CursorManager>
{
    [SerializeField] private GameObject cursorPrefab;
    private GameObject cursorInstance;
    private Canvas cursorCanvas;
    private RectTransform cursorRectTransform;
    private int defaultSortOrder = 32760;
    
    private static readonly HashSet<Canvas> activeCanvases = new HashSet<Canvas>();
    
    public static void RegisterCanvas(Canvas canvas)
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            activeCanvases.Add(canvas);
            UpdateCursorCanvasOrder();
        }
    }

    public static void UnregisterCanvas(Canvas canvas)
    {
        activeCanvases.Remove(canvas);
        UpdateCursorCanvasOrder();
    }

    private static void UpdateCursorCanvasOrder()
    {
        if (Instance != null && Instance.cursorCanvas != null)
        {
            int highestOrder = Instance.defaultSortOrder;
            foreach (var canvas in activeCanvases)
            {
                if (canvas != null && canvas != Instance.cursorCanvas)
                {
                    highestOrder = Mathf.Max(highestOrder, canvas.sortingOrder + 1);
                }
            }
            Instance.cursorCanvas.sortingOrder = highestOrder;
        }
    }

    private void Awake()
    {
        if (Instance != this) // 싱글톤 중복 방지
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        CreateCursorCanvas();
        CreateCursor();
    }

    private void CreateCursorCanvas()
    {
        if (cursorCanvas != null) return;

        var found = GameObject.Find("CursorCanvas");
        if (found != null)
        {
            cursorCanvas = found.GetComponent<Canvas>();
            return;
        }

        GameObject canvasObj = new GameObject("CursorCanvas");
        cursorCanvas = canvasObj.AddComponent<Canvas>();
        cursorCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        cursorCanvas.sortingOrder = defaultSortOrder;

        DontDestroyOnLoad(canvasObj);
    }

    private void CreateCursor()
    {
        if (cursorInstance != null) return; // 중복 생성 방지

        if (cursorPrefab == null)
        {
            Debug.LogError("[CursorUIManager] cursorPrefab이 할당되어 있지 않습니다. Project(Assets) 내 프리팹을 드래그하세요.");
            return;
        }
        if (cursorCanvas == null)
        {
            Debug.LogError("[CursorUIManager] cursorCanvas가 생성되어 있지 않습니다.");
            return;
        }

        cursorInstance = Instantiate(cursorPrefab, cursorCanvas.transform);
        cursorRectTransform = cursorInstance.GetComponent<RectTransform>();
        Cursor.visible = false;
    }

    private void Update()
    {
        if (cursorRectTransform == null)
            return;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            cursorRectTransform.parent as RectTransform,
            Input.mousePosition,
            cursorCanvas.worldCamera,
            out pos
        );
        cursorRectTransform.anchoredPosition = pos;
    }
}
