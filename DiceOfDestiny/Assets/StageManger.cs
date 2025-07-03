using UnityEngine;

public class StageManger : MonoBehaviour
{
    public static StageManger Instance { get; private set; }
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

    // 스테이지 상세
    int[] colcorWieight = new int[6]; 

}
