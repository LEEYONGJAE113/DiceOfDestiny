using UnityEngine;
using UnityEngine.UI;

public class SettingUIController : MonoBehaviour
{
    [Header("Tab Buttons")]
    [SerializeField] private Button displayTabButton;
    [SerializeField] private Button audioTabButton;
    [SerializeField] private Button controlsTabButton;

    [Header("Content Panels")]
    [SerializeField] private GameObject displayPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject controlsPanel;

    [Header("Display Settings")]
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Dropdown fpsDropdown;
    [SerializeField] private Toggle vsyncToggle;


    private Resolution[] resolutions;

    private void Start()
    {
        displayTabButton.onClick.AddListener(() => ShowPanel(displayPanel));
        audioTabButton.onClick.AddListener(() => ShowPanel(audioPanel));
        controlsTabButton.onClick.AddListener(() => ShowPanel(controlsPanel));        

        InitResolutionOptions();
        fullscreenToggle.onValueChanged.AddListener(SetFullScreen);
        fpsDropdown.onValueChanged.AddListener(SetFPSLimit);
        vsyncToggle.onValueChanged.AddListener(SetVSync);

        int vsyncState = PlayerPrefs.GetInt("VSyncEnabled", 0);
        vsyncToggle.isOn = vsyncState == 1;
        QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;

        ShowPanel(displayPanel); // 디스플레이 탭 기본 활성화
        LoadFPSLimit(); // FPS 제한 로드
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ToggleSettings(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }



    private void ShowPanel(GameObject panelToShow)
    {
        displayPanel.SetActive(panelToShow == displayPanel);
        audioPanel.SetActive(panelToShow == audioPanel);
        controlsPanel.SetActive(panelToShow == controlsPanel);
    }

    private void InitResolutionOptions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();
        int currentIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            var res = resolutions[i];
            string label = res.width + "x" + res.height;
            options.Add(label);

            if (res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height)
                currentIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(ChangeResolution);
    }

    private void ChangeResolution(int index)
    {
        var res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    private void SetFullScreen(bool isFull)
    {
        Screen.fullScreen = isFull;
    }

    private void SetFPSLimit(int index)
    {
        QualitySettings.vSyncCount = 0; // 반드시 먼저 끄기

        switch (index)
        {
            case 0: Application.targetFrameRate = 30; break;
            case 1: Application.targetFrameRate = 60; break;
            case 2: Application.targetFrameRate = 144; break;
            case 3: Application.targetFrameRate = -1; break; // 무제한
        }

        PlayerPrefs.SetInt("FPSIndex", index);
    }
    private void LoadFPSLimit()
    {
        int index = PlayerPrefs.GetInt("FPSIndex", 1);
        fpsDropdown.value = index;
        fpsDropdown.RefreshShownValue();
        SetFPSLimit(index);
    }
    private void SetVSync(bool enabled)
    {
        QualitySettings.vSyncCount = enabled ? 1 : 0;
        PlayerPrefs.SetInt("VSyncEnabled", enabled ? 1 : 0);

        // VSync가 꺼졌을 경우에만 FPS 제한 적용
        if (!enabled)
        {
            SetFPSLimit(fpsDropdown.value);
        }

        // UI 상에서 FPS 드롭다운 인터랙션도 제어
        fpsDropdown.interactable = !enabled;
    }
}
