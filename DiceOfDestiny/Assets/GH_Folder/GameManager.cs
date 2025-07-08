using UnityEngine;

public class GameManager : Singletone<GameManager>
{
    public ActionPointManager actionPointManager { get; private set; }

    public HistoryManager historyManager { get; private set; }

    private void Awake()
    {
        actionPointManager = GetComponent<ActionPointManager>();
        historyManager = GetComponent<HistoryManager>();
    }
}