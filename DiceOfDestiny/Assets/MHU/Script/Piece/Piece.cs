using UnityEngine;

[System.Serializable]
public struct Face
{
    public ClassData classData;
    // public ColorData tileColor;
    public TileColor tileColor;
}

[System.Serializable]
public class Piece : MonoBehaviour
{
    [SerializeField] private Face[] faces = new Face[6]; // 6개 면 데이터
    private readonly int topFaceIndex = 2; // 현재 윗면 인덱스 (고정: 2)

    // 전개도 데이터 (십자형: 0:바닥, 1:앞, 2:위, 3:뒤, 4:왼쪽, 5:오른쪽)
    private readonly int[] upTransition = new int[] { 1, 2, 3, 0, 4, 5 }; // 위로 이동
    private readonly int[] downTransition = new int[] { 3, 0, 1, 2, 4, 5 }; // 아래로 이동
    private readonly int[] leftTransition = new int[] { 4, 1, 5, 3, 2, 0 }; // 왼쪽으로 이동
    private readonly int[] rightTransition = new int[] { 5, 1, 4, 3, 0, 2 }; // 오른쪽으로 이동

    void Awake()
    {
        if (faces == null || faces.Length != 6)
        {
            faces = new Face[6];
            Debug.LogWarning("Faces array initialized.");
        }

        for (int i = 0; i < faces.Length; i++)
        {
            if (faces[i].classData == null /*|| faces[i].tileColor == null*/)
            {
                faces[i].classData = new ClassData();
                faces[i].tileColor = new TileColor();
                Debug.LogWarning($"Face {i} initialized with default values.");
            }
        }

    }

    public Face GetFace(int index)
    {
        if (index >= 0 && index < 6)
            return faces[index];
        Debug.LogError($"Invalid face index: {index}");
        return default;
    }

    public void SetFace(int index, ClassData classData, TileColor colorData)
    {
        if (index >= 0 && index < 6)
        {
            faces[index].classData = classData;
            faces[index].tileColor = colorData;
        }
        else
        {
            Debug.LogError($"Invalid face index for SetFace: {index}");
        }
    }

    public int GetTopFaceIndex()
    {
        return topFaceIndex;
    }

    public void UpdateTopFace(Vector2Int direction)
    {
        Face[] newFaces = new Face[6];

        // 이동 방향에 따라 faces 배열 재배치
        if (direction == Vector2Int.up)
        {
            for (int i = 0; i < 6; i++)
                newFaces[i] = faces[upTransition[i]];
        }
        else if (direction == Vector2Int.down)
        {
            for (int i = 0; i < 6; i++)
                newFaces[i] = faces[downTransition[i]];
        }
        else if (direction == Vector2Int.left)
        {
            for (int i = 0; i < 6; i++)
                newFaces[i] = faces[leftTransition[i]];
        }
        else if (direction == Vector2Int.right)
        {
            for (int i = 0; i < 6; i++)
                newFaces[i] = faces[rightTransition[i]];
        }
        else
        {
            Debug.LogWarning($"Invalid move direction: {direction}");
            return;
        }

        // faces 배열 업데이트
        faces = newFaces;

        // 디버깅: 회전 후 각 면의 상태 출력
        //for (int i = 0; i < 6; i++)
        //{
        //    Debug.Log($"Face {i}: ClassData={faces[i].classData}, ColorData={faces[i].colorData}");
        //}
    }
}