using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AP_UI_Test : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentState;
    [SerializeField] private TextMeshProUGUI currentTurn;
    [SerializeField] private TextMeshProUGUI Dice;
    [SerializeField] private TextMeshProUGUI AP;
    [SerializeField] private Button EndTurnButton;

    private void Start()
    {
        EndTurnButton.onClick.AddListener(onClickEndTurnButton);
    }
    public void onClickEndTurnButton()
    {
        ObstacleManager.Instance.UpdateObstacleStep();

        GameManager.Instance.actionPointManager.TurnOff();
    }
    public void Refresh()
    {
        currentState.text = "State : " + GameManager.Instance.actionPointManager.testGameState.ToString();
        currentTurn.text = "Turn : " + GameManager.Instance.actionPointManager.currentTurnNum;

        Dice.text = "Dice : " + GameManager.Instance.actionPointManager.currentDiceNum.ToString();
        AP.text = "AP : " + GameManager.Instance.actionPointManager.currentAP.ToString();
    }
}
