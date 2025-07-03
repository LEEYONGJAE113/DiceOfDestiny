using UnityEngine;

[System.Serializable]
public struct Face
{
    public ClassData classData;
    public ColorData colorData;
}

[System.Serializable]
public class Piece : MonoBehaviour
{
    [SerializeField] private Face[] faces = new Face[6]; // 6개 면 데이터
    [SerializeField] private int topFaceIndex = 2; // 현재 윗면 인덱스 (기본: 2)

    // 전개도 데이터 (십자형: 0:바닥, 1:앞, 2:위, 3:뒤, 4:왼쪽, 5:오른쪽)
    private readonly int[] upTransition = new int[] { 1, 2, 3, 0, 4, 5 }; // 위로 이동: 0→1, 1→2, 2→3, 3→0
    private readonly int[] downTransition = new int[] { 3, 0, 1, 2, 4, 5 }; // 아래로 이동: 0→3, 1→0, 2→1, 3→2
    private readonly int[] leftTransition = new int[] { 4, 1, 5, 3, 2, 0 }; // 왼쪽으로 이동: 0→4, 2→5, 4→2, 5→0
    private readonly int[] rightTransition = new int[] { 5, 1, 4, 3, 0, 2 }; // 오른쪽으로 이동: 0→5, 2→4, 4→0, 5→2

    public Piece()
    {
        faces = new Face[6];
        topFaceIndex = 2;
    }

    public Face GetFace(int index)
    {
        if (index >= 0 && index < 6)
            return faces[index];
        return default;
    }

    public void SetFace(int index, ClassData classData, ColorData colorData)
    {
        if (index >= 0 && index < 6)
        {
            faces[index].classData = classData;
            faces[index].colorData = colorData;
        }
    }

    public int GetTopFaceIndex()
    {
        return topFaceIndex;
    }

    public void UpdateTopFace(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
            topFaceIndex = upTransition[topFaceIndex];
        else if (direction == Vector2Int.down)
            topFaceIndex = downTransition[topFaceIndex];
        else if (direction == Vector2Int.left)
            topFaceIndex = leftTransition[topFaceIndex];
        else if (direction == Vector2Int.right)
            topFaceIndex = rightTransition[topFaceIndex];
    }
}