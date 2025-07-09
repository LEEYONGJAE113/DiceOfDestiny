using UnityEngine;
using TMPro;

public class AP_UI_Test : MonoBehaviour
{
    public TextMeshProUGUI currentState;
    public TextMeshProUGUI currentTurn;
    public TextMeshProUGUI Dice;
    public TextMeshProUGUI AP;

    public void Refresh()
    {
        currentState.text = "State : " + GameManager.Instance.actionPointManager.testGameState.ToString();
        currentTurn.text = "Turn : " + GameManager.Instance.actionPointManager.currentTurnNum;

        Dice.text = "Dice : " + GameManager.Instance.actionPointManager.currentDiceNum.ToString();
        AP.text = "AP : " + GameManager.Instance.actionPointManager.currentAP.ToString();
    }
}
