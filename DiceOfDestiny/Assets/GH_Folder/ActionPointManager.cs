using UnityEngine;

public enum TestGameState
{
    Dice,
    Action,
    TurnOff
}

public class ActionPointManager : MonoBehaviour
{
    // 현재 행동력 주사위의 면 배열
    private int[] diceFaces = new int[] { 1, 1, 2, 2, 3, 3 };

    // 플레이어 상태
    public TestGameState testGameState { get; private set; }
    // 턴 정보
    public int currentTurnNum { get; private set; } = 1;
    // 주사위 정보
    public int currentDiceNum { get; private set; } = 0;
    // 행동력 정보
    public int currentAP { get; private set; } = 0;

    void Start()
    {
        testGameState = TestGameState.Dice;

        Init();
    }

    void Update()
    {
        if (testGameState == TestGameState.Dice)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RollingDice();

                testGameState = TestGameState.Action;
                GameManager.Instance.actionPointUI.Refresh();
            }
        }
        if (testGameState == TestGameState.Action)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                TurnOff();
                GameManager.Instance.actionPointUI.Refresh();
            }
        }
        if (testGameState == TestGameState.TurnOff)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                TurnOff();
                GameManager.Instance.actionPointUI.Refresh();
            }
        }
    }

    public void Init()
    {
        currentAP = 0;
        GameManager.Instance.actionPointUI.Refresh();
    }

    public void AddAP(int _plusAP)
    {
        currentAP += _plusAP;

        // UI Refresh
        GameManager.Instance.actionPointUI.Refresh();
    }
    public void RemoveAP(int _minusAP)
    {
        if (currentAP <= 0)
        {
            Debug.Log("행동력이 없습니다.");
            ToastManager.Instance.ShowToast("행동력이 없습니다.", transform);
            return;
        }
        currentAP -= _minusAP;
        Debug.Log("현재 행동력 : " + currentAP);

        // UI Refresh
        GameManager.Instance.actionPointUI.Refresh();
    }
    public bool TryUseAP()
    {
        if (currentAP <= 0)
            return false;

        return true;
    }

    public void SetDiceFaces(int[] _diceNums)
    {
        for (int i = 0; i < diceFaces.Length; i++)
        {
            diceFaces[i] = _diceNums[i];
        }
    }

    public void RollingDice()
    {
        int randomNum = Random.Range(0, diceFaces.Length);

        currentDiceNum = diceFaces[randomNum];

        AddAP(currentDiceNum);
    }

    public void PieceAction()
    {
        RemoveAP(1);

        if (!TryUseAP())
        {
            testGameState = TestGameState.TurnOff;
            GameManager.Instance.actionPointUI.Refresh();
        }
    }

    public void TurnOff()
    {
        if (testGameState == TestGameState.Dice)
        {
            Debug.Log("먼저 주사위를 굴리세요.");
            ToastManager.Instance.ShowToast("먼저 주사위를 굴리세요.", transform);
            return;
        }

        PieceManager.Instance.DecreaseDebuffAllPieces();

        currentTurnNum++;

        testGameState = TestGameState.Dice;

        Init();
    }
}