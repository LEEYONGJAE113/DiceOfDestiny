using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private PauseMenuController pauseMenuController;
    private void Awake()
    {
        pauseMenuController = GetComponentInChildren<PauseMenuController>(true);
    }

    private void Update()
    {
        if (pauseMenuController == null)
        {
            pauseMenuController = FindFirstObjectByType<PauseMenuController>();
            if (pauseMenuController == null) return; // 여전히 없으면 무시
        }

        // [중요] SettingUI 열려 있으면 ESC는 설정창만 닫고 끝
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UIManager.Instance.IsSettingUIOpen())
            {
                UIManager.Instance.ToggleSettings(false);
                return;
            }

            // PauseMenu만 온/오프
            pauseMenuController.TogglePause();
        }
    }
}
