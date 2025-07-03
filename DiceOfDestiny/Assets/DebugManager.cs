using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    [Header("Stage Debug Settings")]
    [SerializeField] private Button ReColorBoardButton;

    private void Start()
    {
        ReColorBoardButton.onClick.AddListener(onClickReColorBoardButton);
    }

    public void onClickReColorBoardButton()
    {
        BoardManager.Instance.SetBoard();
    }
}
