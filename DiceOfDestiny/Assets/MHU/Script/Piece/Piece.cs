using UnityEngine;

[CreateAssetMenu(fileName = "Piece", menuName = "Piece/Piece", order = 10)]
public class Piece : ScriptableObject
{
    [SerializeField] public Face[] faces = new Face[6]; // 6개 면 데이터
    private Buff buff = new Buff();
    private Debuff debuff = new Debuff();

    private class Buff
    {

    }

    private class Debuff
    {
        public bool stun;
    }

    public bool GetStun()
    {
        return debuff.stun;
    }
    public void SetStun(bool value)
    {
        debuff.stun = value;
    }
}

[System.Serializable]
public struct Face
{
    public ClassData classData; // 클래스 데이터
    public TileColor color;
}