using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singletone<UIManager>
{
    [Header("UI Prefabs")]
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject settingUIPrefab;
    [SerializeField] private GameObject dialogueUIPrefab;

    private GameObject currentUIRoot;
    private GameObject settingUI;
    private GameObject dialogueUI;
    private Canvas currentCanvas;

    public bool IsSettingUIOpen() => settingUI != null && settingUI.activeSelf;
    public bool IsUIReady { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        IsUIReady = false;

        // 항상 현재 씬에서 Canvas 찾기
        currentCanvas = FindFirstObjectByType<Canvas>();
        if (currentCanvas == null)
        {
            Debug.LogError("[UIManager] No Canvas found in scene. UI will not be visible.");
            return;
        }

        if (currentUIRoot != null)
        {
            Destroy(currentUIRoot);
            currentUIRoot = null;
        }
        if (settingUI != null)
        {
            Destroy(settingUI);
            settingUI = null;
        }
        if (dialogueUI != null)
        {
            Destroy(dialogueUI);
            dialogueUI = null;
        }

        var childrenToDelete = new System.Collections.Generic.List<GameObject>();
        foreach (Transform child in currentCanvas.transform)
        {
            string n = child.name;
            if (n.Contains("MainUI") || n.Contains("GameUI") ||
                n.Contains("SettingUI") || n.Contains("DialogueUI"))
            {
                childrenToDelete.Add(child.gameObject);
            }
        }
        foreach (var go in childrenToDelete)
            Destroy(go);

        switch (scene.name)
        {
            case "Main":
                currentUIRoot = Instantiate(mainUI, currentCanvas.transform, false);
                break;
            case "GameScene":
                currentUIRoot = Instantiate(gameUI, currentCanvas.transform, false);
                break;
            default:
                currentUIRoot = null;
                break;
        }

        settingUI = Instantiate(settingUIPrefab, currentCanvas.transform, false);
        settingUI.SetActive(false);

        if (dialogueUIPrefab != null)
        {
            dialogueUI = Instantiate(dialogueUIPrefab, currentCanvas.transform, false);
            dialogueUI.SetActive(false);
        }

        IsUIReady = true;
    }

    public void ToggleSettings(bool isOn)
    {
        
        if (settingUI == null)
        {
            Debug.LogWarning("[UIManager] SettingUI가 존재하지 않습니다.");
            return;
        }
        settingUI.SetActive(isOn);
    }

    public void ShowDialogue()
    {
        if (dialogueUI == null)
        {
            Debug.LogWarning("[UIManager] DialogueUI가 존재하지 않습니다.");
            return;
        }
        dialogueUI.SetActive(true);
    }
}
