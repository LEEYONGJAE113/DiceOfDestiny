using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceCustomizeManager : MonoBehaviour
{
    [SerializeField] private GameObject cutomizePanel;
    [SerializeField] private GameObject carouselUIPanel;

    [SerializeField] private GameObject pieceCarouselUI;
    [SerializeField] private GameObject pieceNetCarouselUI;

    [SerializeField] private GameObject piecesContent;
    [SerializeField] private GameObject piecePreviewButtonPrefab;

    [SerializeField] private GameObject pieceNetPreviewButtonPrefab;
    [SerializeField] private GameObject pieceNetContent;

    [Header("ClassData")]
    [SerializeField] private ClassData[] classDatas;

    List<PiecePreviewButton> piecePreviewButtonList = new List<PiecePreviewButton>();

    public GameObject stickerDrawer;

    public GameObject stickerSourcePrefab;

    private void Start()
    {
        InitializePiecesCaruselUI();
        InitializePieceNetCaruselUI();

        InitializeStickerDrawer();
    }

    private void InitializeStickerDrawer()
    {
        foreach (var sticker in InventoryManager.Instance.classStickers)
        {
            GameObject stickerSource = Instantiate(stickerSourcePrefab, stickerDrawer.GetComponent<ScrollRect>().content.transform);
            stickerSource.GetComponent<StickerSource>().stickerSprite.sprite = sticker.Key.sprite;
        }
    }

    public void InitializePiecesCaruselUI()
    {
        for (int i = 0; i < InventoryManager.Instance.pieces.Count; i++)
        {
            Piece piece = InventoryManager.Instance.pieces[i];
            PiecePreviewButton button = Instantiate(piecePreviewButtonPrefab, piecesContent.transform).GetComponent<PiecePreviewButton>();
            button.InitializePiecePreviewButton(BoardManager.Instance.GetColor(piece.faces[2].color), piece.faces[2].classData.sprite, () => OnClickPiecePreviewButton(piece));
            piecePreviewButtonList.Add(button);
        }
    }
    public void InitializePieceNetCaruselUI()
    {
        for (int i = 0; i < InventoryManager.Instance.pieceNets.Count; i++)
        {
            PieceNet pieceNet = InventoryManager.Instance.pieceNets[i];
            PieceNetPreviewButton button = Instantiate(pieceNetPreviewButtonPrefab, pieceNetContent.transform).GetComponent<PieceNetPreviewButton>();
            button.InitializePieceNetPreviewButton(pieceNet, () => OnClickPieceNetPreviewButton(pieceNet));
        }
    }

    public void OnClickPiecePreviewButton(Piece piece)
    {
        ChangeToCustomizePanel();
    }

    public void OnClickPieceNetPreviewButton(PieceNet pieceNet)
    {
        ChangeToCustomizePanel();
    }

    void ChangeToCustomizePanel()
    {
        cutomizePanel.SetActive(true);
        carouselUIPanel.SetActive(false);
    }

    public void OnClickPieceCaruselUIButton()
    {
        if (pieceCarouselUI.activeSelf == false)
        {
            pieceNetCarouselUI.SetActive(false);
            pieceCarouselUI.SetActive(true);
        }
    }

    public void OnClickPieceNetCaruselUIButton()
    {
        if (pieceNetCarouselUI.activeSelf == false)
        {
            pieceCarouselUI.SetActive(false);
            pieceNetCarouselUI.SetActive(true);
        }
    }

    public void OnClickBackToSelectPanelButton()
    {
        cutomizePanel.SetActive(false);
        carouselUIPanel.SetActive(true);
    }
}
