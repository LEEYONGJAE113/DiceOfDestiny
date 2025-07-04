using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    [Header("Stage Debug Settings")]
    [SerializeField] private Button reColorBoardButton;
    [SerializeField] private Button regenerateButton;
    [SerializeField] private Button nextStepButton;


    private void Start()
    {
        reColorBoardButton.onClick.AddListener(onClickReColorBoardButton);
    }

    public void onClickReColorBoardButton()
    {
        StageManger.Instance.StartStage();
    }
}
