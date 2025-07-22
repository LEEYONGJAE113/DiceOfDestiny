using UnityEngine;
using UnityEngine.UI;

public class UIBackpack : MonoBehaviour
{
    [Header("Backpack Button")]
    [SerializeField] private Button BackpackOpenCloseButton;

    [SerializeField] private Button[] PieceAppearButton;

    [Header("Backpack UI")]
    [SerializeField] private GameObject UIPieceGroup;

    [SerializeField] private Transform pieceParent;

    private void Start()
    {
        BackpackOpenCloseButton.onClick.AddListener(onClickBackpackOpenCloseButton);
    }


    public void onClickBackpackOpenCloseButton()
    {
        UIPieceGroup.SetActive(!UIPieceGroup.activeSelf);
    }

    public void onClickPieceAppearButton(int index)
    {
        Debug.Log("기물 생성! index : " + index);
        Instantiate(PieceManager.Instance.piecePrefabs[index], new Vector2(index, 0), Quaternion.identity, pieceParent);
    }
}
