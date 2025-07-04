using UnityEngine;

public class GameManager : Singletone<GameManager>
{
    public ActionPointManager actionPointManager { get; private set; }

    private void Awake()
    {
        actionPointManager = GetComponent<ActionPointManager>();
    }
}
