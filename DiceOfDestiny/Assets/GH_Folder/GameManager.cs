using UnityEngine;

public class GameManager : Singletone<GameManager>
{
    public ActionPointManager actionPointManager { get; private set; }

    public HistoryManager historyManager { get; private set; }

    public AP_UI_Test aP_UI_Test { get; private set; }

    private void Start()
    {
        var canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("[GameManager] Canvas 오브젝트를 찾을 수 없습니다.");
            return;
        }
        actionPointManager = canvas.GetComponentInChildren<ActionPointManager>();
        aP_UI_Test = canvas.GetComponentInChildren<AP_UI_Test>();

        if (actionPointManager == null) Debug.LogError("[GameManager] ActionPointManager를 찾을 수 없습니다.");
        if (aP_UI_Test == null) Debug.LogError("[GameManager] AP_UI_Test를 찾을 수 없습니다.");

        historyManager = GetComponent<HistoryManager>();
        if (historyManager == null) Debug.LogWarning("[GameManager] HistoryManager가 붙어있지 않습니다.");
    }

}