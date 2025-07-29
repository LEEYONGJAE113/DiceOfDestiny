using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    void Start()
    {
        continueButton.onClick.AddListener(OnContinueClicked);
        newGameButton.onClick.AddListener(OnNewGameClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        exitButton.onClick.AddListener(() => Application.Quit());

        bool hasSave = PlayerPrefs.GetInt("SaveExists", 0) == 1;
        continueButton.interactable = hasSave;

        AudioManager.Instance.PlayBGM("MainBGM");
    }


    private void OnContinueClicked()
    {
        string lastScene = PlayerPrefs.GetString("LastScene", "GH_GameScene");
        SceneManager.LoadScene(lastScene);
    }

    private void OnNewGameClicked()
    {
        PlayerPrefs.SetString("LastScene", "GH_GameScene");
        PlayerPrefs.SetInt("SaveExists", 1);
        SceneManager.LoadScene("GH_GameScene");
    }
    private void OnSettingsClicked()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ToggleSettings(true);
        }
    }

    private void OnExitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
