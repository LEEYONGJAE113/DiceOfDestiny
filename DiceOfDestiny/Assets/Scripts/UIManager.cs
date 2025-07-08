using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Modules")]
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject settingUI;
    [SerializeField] private GameObject dialogueUI;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetUIContext(scene.name);
    }

    private void SetUIContext(string sceneName)
    {
        bool isMain = sceneName.Contains("Main");
        bool isGame = sceneName.Contains("Game");

        mainUI?.SetActive(isMain);
        gameUI?.SetActive(isGame);
        settingUI?.SetActive(false);
        dialogueUI?.SetActive(false);
    }

    public void ToggleSettings(bool isOn)
    {
        settingUI?.SetActive(isOn);
    }

    public void ShowDialogue()
    {
        dialogueUI?.SetActive(true);
    }
}
