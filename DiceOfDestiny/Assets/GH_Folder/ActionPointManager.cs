using UnityEngine;

public class ActionPointManager : MonoBehaviour
{
    // 행동력 정보
    public int currentAP { get; private set; } = 0;

    // 현재 행동력 주사위의 면 배열
    private int[] diceFaces = new int[] { 1, 1, 2, 2, 3, 3 };

    public void Init()
    {
        currentAP = 0;
    }

    public void AddAP(int _plusAP)
    {
        currentAP += _plusAP;

        // UI Refresh
    }
    public void RemoveAP(int _minusAP)
    {
        if (currentAP <= 0)
        {
            Debug.Log("행동력이 없습니다.");
            return;
        }
        currentAP -= _minusAP;
        Debug.Log("현재 행동력 : " + currentAP);

        // UI Refresh
    }
    public bool TryUseAP()
    {
        if (currentAP <= 0)
            return false;

        return true;
    }



    public void SetDiceFaces(int[] _diceNums)
    {
        for (int i = 0; i < diceFaces.Length; i++)
        {
            diceFaces[i] = _diceNums[i];
        }
    }
    public int RollingDice()
    {
        int randomNum = Random.Range(0, diceFaces.Length);

        AddAP(diceFaces[randomNum]);

        return currentAP;
    }
}