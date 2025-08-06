using UnityEngine;

[CreateAssetMenu(fileName = "Piece", menuName = "Piece/Piece", order = 10)]
public class Piece : ScriptableObject
{
    [SerializeField] public bool isAvailable; // 사용 가능 여부
    [SerializeField] public Face[] faces = new Face[6]; // 6개 면 데이터
    //   4
    // 1 2 3 0
    //   5
    public Buff buff = new Buff();
    public Debuff debuff = new Debuff();

    public class Buff
    {

    }

    public class Debuff
    {
        public bool IsStun { get; private set; } = false;
        public int stunTurn = 0;
        public void SetStun(bool _stun, int _turn)
        {
            IsStun = _stun;
            stunTurn = _turn;
        }
        public void DecreaseStunTurn()
        {
            stunTurn--;
            Debug.Log("남은 기절 턴 : " + stunTurn);
            if (stunTurn <= 0)
            {
                Debug.Log("스턴이 풀렸습니다.");
                IsStun = false;
            }
        }
    }
}

[System.Serializable]
public struct Face
{
    public ClassData classData; // 클래스 데이터
    public TileColor color;
}