using System.Collections.Generic;
using UnityEngine;

public class HistoryManager : MonoBehaviour
{
    //스테이지 목표 진행률
    //

    //퍼즐 말들의 기록
    private List<PieceHistory> pieceHistories;

    public void Save(int _x, int _y, int _prevJob)
    {
        // 저장된 데이터가 5개이상일 경우 가장 오래된 데이터 제거
        if (pieceHistories.Count >= 5)
            pieceHistories.RemoveAt(0);
        pieceHistories.Add(new PieceHistory(_x, _y, _prevJob));
    }

    public void Load(int _wantTurn)
    {
        /*
        int wantNum = Count - 1 - _wantTurn;
        현재 위치와 말들에게 = pieceHistories[wantNum]
        */
    }
}
