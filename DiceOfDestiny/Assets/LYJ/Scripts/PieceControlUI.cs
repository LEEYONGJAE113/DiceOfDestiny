using UnityEngine;
using UnityEngine.UI;

public class PieceControlUI : MonoBehaviour
{
    private enum directions { Up, Down, Left, Right }
    [SerializeField] private directions dir;
    [SerializeField] private GameObject faceColor;
    [SerializeField] private GameObject faceClass;
    private Image buttonImage;

    void Awake()
    {
        buttonImage = GetComponentInChildren<Image>();
        buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 0f);

        faceClass.SetActive(false);
        faceColor.SetActive(false);
    }

    public void OnButtonEnter()
    {
        buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 1f);

        faceClass.SetActive(true);
        faceColor.SetActive(true);

        Image faceClassImage = faceClass.GetComponent<Image>();
        Image faceColorImage = faceColor.GetComponent<Image>();

        int faceIndex;

        switch (dir)
        {
            case directions.Up:
                faceIndex = 3;
                break;
            case directions.Down:
                faceIndex = 1;
                break;
            case directions.Left:
                faceIndex = 5;
                break;
            case directions.Right:
                faceIndex = 4;
                break;
            default:
                Debug.Log("<color=#00aeff>이미지 인덱스가 뭔가 오류남</color>");
                faceIndex = 2;
                break;
        }
        Face gettedFace = GetComponentInParent<PieceController>().GetFace(faceIndex);
        faceClassImage.sprite = gettedFace.classData.sprite;
        Color gettedColor = BoardManager.Instance.tileColors[(int)(gettedFace.color)];
        faceColorImage.color = gettedColor;
    }

    public void OnButtonExit()
    {
        buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 0f);
        
        faceClass.SetActive(false);
        faceColor.SetActive(false);
    }

    public void OnClick()
    {
        switch (dir)
        {
            case directions.Up:
                Debug.Log("위버튼눌림");
                // 매니저에서 지우기 요청
                // 피스 컨트롤러에서 위로가는거
                // 피스매니저에서 이동함 플래그 세우기
                // 매니저에서 그리기 요청
                break;
            case directions.Down:
                Debug.Log("밑버튼눌림");
                // 매니저에서 지우기 요청
                // 피스 컨트롤러에서 밑으로가는거
                // 피스매니저에서 이동함 플래그 세우기
                // 매니저에서 그리기 요청
                break;
            case directions.Left:
                Debug.Log("왼버튼눌림");
                // 매니저에서 지우기 요청
                // 피스 컨트롤러에서 왼쪽으로가는거
                // 피스매니저에서 이동함 플래그 세우기
                // 매니저에서 그리기 요청
                break;
            case directions.Right:
                Debug.Log("오른버튼눌림");
                // 매니저에서 지우기 요청
                // 피스 컨트롤러에서 오른쪽으로가는거
                // 피스매니저에서 이동함 플래그 세우기
                // 매니저에서 그리기 요청
                break;
            default:
                Debug.Log("<color=#00aeff>UI에 방향 할당되지 않음</color>");
                break;
        }
    }
}
