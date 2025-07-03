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

    // �������� ��
    int[] colcorWieight = new int[6]; 

}
