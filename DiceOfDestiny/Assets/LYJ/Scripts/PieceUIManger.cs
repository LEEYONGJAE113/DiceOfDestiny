using System.Collections.Generic;
using UnityEngine;

public class PieceUIManager : Singletone<PieceUIManager>
{
    [Header("상태별 UI")]
    [SerializeField, Tooltip("선택 가능")] private GameObject SelectableUI;
    [SerializeField, Tooltip("현재 선택 중")] private GameObject SelectingUI;
    [SerializeField, Tooltip("선택 불가")] private GameObject CantSelectUI;

    private Dictionary<States, GameObject> uiDic;

    void Awake()
    {
        InitDic();
    }

    private void InitDic()
    {
        uiDic = new Dictionary<States, GameObject>
        {
            {States.Selectable, SelectableUI},
            {States.Selecting, SelectingUI},
            {States.CantSelect, CantSelectUI}
        };
    }

    public void CreatePieceUI(States state, GameObject parent)
    {
        if (uiDic.TryGetValue(state, out GameObject newUIObj))
        {
            Instantiate(newUIObj, parent.transform.position, Quaternion.identity, parent.transform);
        }
    }

    public void ErasePieceUI(GameObject parent)
    {
        RectTransform[] allUIs = parent.GetComponentsInChildren<RectTransform>();

        foreach (RectTransform targets in allUIs)
        {
            if (targets == null) { return; }
            if (targets.CompareTag("PieceUI")) // 추후 태그 수정 필요
            {
                Destroy(targets.gameObject);
            }
        }
    }

}
