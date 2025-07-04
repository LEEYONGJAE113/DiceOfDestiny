using System.Collections.Generic;
using System.Linq;
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

    private List<int> availableFpsList;

    private void Awake()
    {
        availableFpsList = GetAvailableRefreshRates();
    }

    private void Start()
    {
        displayTabButton.onClick.AddListener(() => ShowPanel(displayPanel));
        audioTabButton.onClick.AddListener(() => ShowPanel(audioPanel));
        controlsTabButton.onClick.AddListener(() => ShowPanel(controlsPanel));        

        InitResolutionOptions();
        InitFPSDropdown();
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

        // 고유 해상도 딕셔너리 (중복 제거)
        var uniqueResolutions = new Dictionary<string, Resolution>();

        foreach (var res in resolutions)
        {
            string key = res.width + "x" + res.height;
            if (!uniqueResolutions.ContainsKey(key))
            {
                uniqueResolutions.Add(key, res);
            }
        }

        // 정렬된 리스트 생성 (해상도 큰 순)
        var sortedResList = uniqueResolutions.Values
            .OrderByDescending(r => r.width)
            .ThenByDescending(r => r.height)
            .ToList();

        var options = new List<string>();
        int currentIndex = 0;

        for (int i = 0; i < sortedResList.Count; i++)
        {
            var res = sortedResList[i];
            string label = res.width + "x" + res.height;
            options.Add(label);

            if (res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height)
                currentIndex = i;
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener((index) =>
        {
            string[] wh = options[index].Split('x');
            int w = int.Parse(wh[0]);
            int h = int.Parse(wh[1]);
            Screen.SetResolution(w, h, Screen.fullScreen);
        });
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
        QualitySettings.vSyncCount = 0;

        int selectedFps = availableFpsList[index];
        Application.targetFrameRate = selectedFps;

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

    private List<int> GetAvailableRefreshRates()
    {
        HashSet<int> refreshRates = new HashSet<int>();

        foreach (var res in Screen.resolutions)
        {
            // 정수 FPS 추출 (예: 59.94 → 59)
            int hz = Mathf.RoundToInt(
                (float)res.refreshRateRatio.numerator / res.refreshRateRatio.denominator);

            refreshRates.Add(hz);
        }

        List<int> result = refreshRates.ToList();
        result.Sort();

        // 기본적으로 60FPS는 넣어두기
        if (!result.Contains(60))
            result.Insert(0, 60);

        return result;
    }

    private void InitFPSDropdown()
    {
        availableFpsList = GetAvailableRefreshRates();
        availableFpsList.Add(-1); // 무제한

        // 드롭다운 항목 텍스트 생성
        List<string> labels = availableFpsList.Select(fps =>
            fps == -1 ? "Unlimited" : $"{fps} FPS").ToList();

        fpsDropdown.ClearOptions();
        fpsDropdown.AddOptions(labels);

        // 기본값: 60FPS 인덱스 또는 0
        int defaultIndex = availableFpsList.IndexOf(60);
        if (defaultIndex == -1) defaultIndex = 0;

        int savedIndex = PlayerPrefs.GetInt("FPSIndex", defaultIndex);
        savedIndex = Mathf.Clamp(savedIndex, 0, availableFpsList.Count - 1);

        fpsDropdown.value = savedIndex;
        fpsDropdown.RefreshShownValue();

        SetFPSLimit(savedIndex);
    }

}
