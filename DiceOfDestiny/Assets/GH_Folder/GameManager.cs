using UnityEngine;

public class GameManager : Singletone<GameManager>
{
    public ActionPointManager actionPointManager { get; private set; }

    public HistoryManager historyManager { get; private set; }

    public AP_UI_Test aP_UI_Test { get; private set; }

    private void Awake()
    {
        actionPointManager = GetComponent<ActionPointManager>();
        historyManager = GetComponent<HistoryManager>();
        
        aP_UI_Test = GetComponent<AP_UI_Test>();
    }
}