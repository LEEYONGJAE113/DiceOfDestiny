using NUnit.Framework;
using UnityEngine;

public class StageManger : MonoBehaviour
{
    public static StageManger Instance { get; private set; }

    [Header("Stage Settings")]
    [SerializeField] private int stageIndex = 0;
    [SerializeField] private StageDifficultyProfile[] stageProfiles;

    StageDifficultyProfile currentProfile;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentProfile = stageProfiles[stageIndex];
    }

    public void StartStage()
    {
        ObstacleManager.Instance.RemoveAllObstacle();
        BoardManager.Instance.SetBoard(currentProfile);
    }

    public void NextStage()
    {
        ++stageIndex;
        currentProfile = stageProfiles[stageIndex];

        ObstacleManager.Instance.RemoveAllObstacle();
        BoardManager.Instance.SetBoard(currentProfile);
    }
}
