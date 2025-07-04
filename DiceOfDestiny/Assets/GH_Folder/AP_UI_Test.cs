using UnityEngine;
using TMPro;

public enum TestGameState
{
    Dice,
    Action,
    TurnOff
}

public class AP_UI_Test : MonoBehaviour
{
    public GameObject player;

    public TextMeshProUGUI currentState;
    public TextMeshProUGUI currentTurn;
    public TextMeshProUGUI Dice;
    public TextMeshProUGUI AP;

    public TestGameState testGameState;

    public int currentTurnNum = 1;

    void Start()
    {
        testGameState = TestGameState.Dice;

        GameManager.Instance.actionPointManager.Init();
    }

    void Update()
    {
        switch (testGameState)
        {
            case TestGameState.Dice:
                currentState.text = "State : Dice";
                if (Input.GetKeyDown(KeyCode.D))
                {
                    Dice.text = "Dice : " + GameManager.Instance.actionPointManager.RollingDice().ToString();
                    AP.text = "AP : " + GameManager.Instance.actionPointManager.currentAP.ToString();
                    testGameState = TestGameState.Action;
                }
                break;
            case TestGameState.Action:
                currentState.text = "State : Action";
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    PlayerMove();
                    GameManager.Instance.actionPointManager.RemoveAP(1);
                    AP.text = "AP : " + GameManager.Instance.actionPointManager.currentAP.ToString();
                    
                    if (!GameManager.Instance.actionPointManager.TryUseAP())
                    {
                        testGameState = TestGameState.TurnOff;
                        break;
                    }
                }
                break;
            case TestGameState.TurnOff:
                currentState.text = "State : TurnOff";
                if (Input.GetKeyDown(KeyCode.T))
                {
                    currentTurnNum++;
                    currentTurn.text = "Turn : " + currentTurnNum;
                    testGameState = TestGameState.Dice;
                }
                break;
            default:
                Debug.Log("상태 오류");
                break;
        }
    }

    private void PlayerMove()
    {
        player.transform.position += new Vector3(0, 0.5f, 0);
    }
}
