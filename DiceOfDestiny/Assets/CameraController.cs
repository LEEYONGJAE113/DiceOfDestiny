using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera mainCamera;

    [Header("Zoom Settings")]
    [SerializeField] private float[] zoomLevels = { 7f, 5f, 3f };
    private int currentZoomLevel = 0;

    [Header("Drag Settings")]
    [SerializeField] private float dragSpeed = 1.0f;
    // 드래그 이동 활성 여부
    private bool canDrag = false;

    private Vector3 lastMousePosition;

    private Vector2 minBounds; // 최초 화면 좌하단 월드 좌표
    private Vector2 maxBounds; // 최초 화면 우상단 월드 좌표

    void Start()
    {

        mainCamera = Camera.main;

        // 초기 카메라 크기 세팅
        mainCamera.orthographicSize = zoomLevels[currentZoomLevel];
        SetInitialBounds();
    }
    void Update()
    {
        HandleZoom();
        HandleDrag();
    }

    void SetInitialBounds()
    {
        Vector3 camPos = mainCamera.transform.position;
        float camHeight = mainCamera.orthographicSize * 2f;
        float camWidth = camHeight * mainCamera.aspect;

        minBounds = new Vector2(camPos.x - camWidth / 2f, camPos.y - camHeight / 2f);
        maxBounds = new Vector2(camPos.x + camWidth / 2f, camPos.y + camHeight / 2f);
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            Vector3 mouseWorldPosBefore = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            if (scroll > 0f) // 위로 올릴 경우 확대
            {
                if (currentZoomLevel < zoomLevels.Length - 1)
                {
                    currentZoomLevel++;
                    mainCamera.orthographicSize = zoomLevels[currentZoomLevel];
                }
            }
            else if (scroll < 0f) // 아래로 내릴 경우 축소
            {
                if (currentZoomLevel > 0)
                {
                    currentZoomLevel--;
                    mainCamera.orthographicSize = zoomLevels[currentZoomLevel];
                }
            }

            if (currentZoomLevel == 0) // 가장 큰 줌일 경우
            {
                mainCamera.transform.position = new Vector3(0, 0, -10);
            }
            else // 아닐 경우
            {
                Vector3 mouseWorldPosAfter = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                Vector3 diff = mouseWorldPosBefore - mouseWorldPosAfter;
                mainCamera.transform.position += diff;

                ClampCameraPosition();
            }

            canDrag = currentZoomLevel > 0; // 가장 큰 줌 레벨에서는 드래그 비활성화
        }
    }

    void HandleDrag()
    {
        if (!canDrag) return;

        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            Vector3 move = new Vector3(-delta.x, -delta.y, 0f) * (mainCamera.orthographicSize / 1000f) * dragSpeed;
            mainCamera.transform.position += move;

            lastMousePosition = Input.mousePosition;

            ClampCameraPosition();
        }
    }
    void ClampCameraPosition()
    {
        Vector3 pos = mainCamera.transform.position;

        float camHeight = mainCamera.orthographicSize * 2f;
        float camWidth = camHeight * mainCamera.aspect;

        float paddingRatio = 0.1f; // 전체 width 대비 10% 여백
        float zoomFactor = zoomLevels[0] / mainCamera.orthographicSize;
        float paddingX = camWidth * paddingRatio * zoomFactor;

        float minX = minBounds.x + camWidth / 2f + paddingX;
        float maxX = maxBounds.x - camWidth / 2f - paddingX;

        float minY = minBounds.y + camHeight / 2f;
        float maxY = maxBounds.y - camHeight / 2f;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        mainCamera.transform.position = pos;
    }
}
