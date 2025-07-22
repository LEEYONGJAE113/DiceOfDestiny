using UnityEngine;
using UnityEngine.UI;

public class PainterActiveSkillUI : MonoBehaviour
{
    [SerializeField] private Button redButton;
    [SerializeField] private Button greenButton;
    [SerializeField] private Button blueButton;
    [SerializeField] private Button yellowButton;
    [SerializeField] private Button purpleButton;
    [SerializeField] private Button grayButton;
    [SerializeField] private Image Pallete;

    private TileColor selectedColor = TileColor.None;

    // 선택된 색상을 외부에서 가져갈 수 있는 getter
    public TileColor SelectedColor
    {
        get { return selectedColor; }
    }

    void Start()
    {
        // 버튼에 리스너 추가
        if (redButton != null) redButton.onClick.AddListener(OnRedButtonClicked);
        if (greenButton != null) greenButton.onClick.AddListener(OnGreenButtonClicked);
        if (blueButton != null) blueButton.onClick.AddListener(OnBlueButtonClicked);
        if (yellowButton != null) yellowButton.onClick.AddListener(OnYellowButtonClicked);
        if (purpleButton != null) purpleButton.onClick.AddListener(OnPurpleButtonClicked);
        if (grayButton != null) grayButton.onClick.AddListener(OnGrayButtonClicked);
    }

    public void ShowPalette()
    {
        Pallete.gameObject.SetActive(true); // UI 활성화
    }

    // 각 버튼 클릭 시 호출되는 public 함수
    public void OnRedButtonClicked()
    {
        selectedColor = TileColor.Red;
        Pallete.gameObject.SetActive(false); // UI 비활성화
    }

    public void OnGreenButtonClicked()
    {
        selectedColor = TileColor.Green;
        Pallete.gameObject.SetActive(false); // UI 비활성화
    }

    public void OnBlueButtonClicked()
    {
        selectedColor = TileColor.Blue;
        Pallete.gameObject.SetActive(false); // UI 비활성화
    }

    public void OnYellowButtonClicked()
    {
        selectedColor = TileColor.Yellow;
        Pallete.gameObject.SetActive(false); // UI 비활성화
    }

    public void OnPurpleButtonClicked()
    {
        selectedColor = TileColor.Purple;
        Pallete.gameObject.SetActive(false); // UI 비활성화
    }

    public void OnGrayButtonClicked()
    {
        selectedColor = TileColor.Gray;
        Pallete.gameObject.SetActive(false); // UI 비활성화
    }

    // UI가 비활성화될 때 선택된 색상 초기화
    public void OnDisable()
    {
        selectedColor = TileColor.None;
    }
}
