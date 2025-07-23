using UnityEngine;
using UnityEngine.UI;

public class PainterActiveSkillUI : MonoBehaviour
{
    [Header("팔레트 색 버튼 6개")]
    [SerializeField] private Button redButton;
    [SerializeField] private Button greenButton;
    [SerializeField] private Button blueButton;
    [SerializeField] private Button yellowButton;
    [SerializeField] private Button purpleButton;
    [SerializeField] private Button grayButton;

    [Header("팔레트 이미지")]
    [SerializeField] private Image paletteImage;

    [Header("팔레트 0,0 기준 위치")]
    [SerializeField] private Vector2 paletteOffset = new Vector2(-424f, -460f);
    [Header("한 칸당 움직일 거리")]
    [SerializeField] private float paletteDistance = 77f;

    private TileColor selectedColor = TileColor.None;

    // 선택된 색상을 외부에서 가져갈 수 있는 getter
    public TileColor SelectedColor
    {
        get { return selectedColor; }
    }

    void Start()
    {
        AssignButtonColors();

        // 버튼에 리스너 추가
        if (redButton != null) redButton.onClick.AddListener(OnRedButtonClicked);
        if (greenButton != null) greenButton.onClick.AddListener(OnGreenButtonClicked);
        if (blueButton != null) blueButton.onClick.AddListener(OnBlueButtonClicked);
        if (yellowButton != null) yellowButton.onClick.AddListener(OnYellowButtonClicked);
        if (purpleButton != null) purpleButton.onClick.AddListener(OnPurpleButtonClicked);
        if (grayButton != null) grayButton.onClick.AddListener(OnGrayButtonClicked);
    }

    private void AssignButtonColors()
    {
        if (redButton != null && BoardManager.Instance.tileColors.Length > 0)
            redButton.GetComponent<Image>().color = BoardManager.Instance.tileColors[0]; // 빨강
        if (greenButton != null && BoardManager.Instance.tileColors.Length > 1)
            greenButton.GetComponent<Image>().color = BoardManager.Instance.tileColors[1]; // 초록
        if (blueButton != null && BoardManager.Instance.tileColors.Length > 2)
            blueButton.GetComponent<Image>().color = BoardManager.Instance.tileColors[2]; // 파랑
        if (yellowButton != null && BoardManager.Instance.tileColors.Length > 3)
            yellowButton.GetComponent<Image>().color = BoardManager.Instance.tileColors[3]; // 노랑
        if (purpleButton != null && BoardManager.Instance.tileColors.Length > 4)
            purpleButton.GetComponent<Image>().color = BoardManager.Instance.tileColors[4]; // 보라
        if (grayButton != null && BoardManager.Instance.tileColors.Length > 5)
            grayButton.GetComponent<Image>().color = BoardManager.Instance.tileColors[5]; // 회색
    }

    public void ShowPalette()
    {
        paletteImage.gameObject.SetActive(true); // UI 활성화

        // 마지막으로 클릭한 타일의 위치 가져오기
        Vector2Int selectPos = BoardSelectManager.Instance.lastClickedPosition;

        // 타일 위치를 기준으로 UI 위치 계산 (각 칸당 80씩 증가)
        float posX = selectPos.x * paletteDistance + paletteOffset.x;
        float posY = selectPos.y * paletteDistance + paletteOffset.y;

        // 팔레트 이미지의 RectTransform 가져오기
        RectTransform paletteRect = paletteImage.GetComponent<RectTransform>();

        // UI 위치 설정 (anchoredPosition 사용)
        paletteRect.anchoredPosition = new Vector2(posX, posY);
    }

    // 각 버튼 클릭 시 호출되는 public 함수
    public void OnRedButtonClicked()
    {
        selectedColor = TileColor.Red;
        paletteImage.gameObject.SetActive(false); // UI 비활성화
    }

    public void OnGreenButtonClicked()
    {
        selectedColor = TileColor.Green;
        paletteImage.gameObject.SetActive(false); // UI 비활성화
    }

    public void OnBlueButtonClicked()
    {
        selectedColor = TileColor.Blue;
        paletteImage.gameObject.SetActive(false); // UI 비활성화
    }

    public void OnYellowButtonClicked()
    {
        selectedColor = TileColor.Yellow;
        paletteImage.gameObject.SetActive(false); // UI 비활성화
    }

    public void OnPurpleButtonClicked()
    {
        selectedColor = TileColor.Purple;
        paletteImage.gameObject.SetActive(false); // UI 비활성화
    }

    public void OnGrayButtonClicked()
    {
        selectedColor = TileColor.Gray;
        paletteImage.gameObject.SetActive(false); // UI 비활성화
    }

    // UI가 비활성화될 때 선택된 색상 초기화
    public void OnDisable()
    {
        selectedColor = TileColor.None;
    }
}
