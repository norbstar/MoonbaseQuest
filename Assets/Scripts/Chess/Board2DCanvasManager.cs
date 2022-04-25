using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Chess
{
    public class Board2DCanvasManager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] List<GameObject> cells;

        [Header("Light Sprites")]
        [SerializeField] Sprite lightPawn;
        [SerializeField] Sprite lightRook;
        [SerializeField] Sprite lightKnight;
        [SerializeField] Sprite lightBishop;
        [SerializeField] Sprite lightKing;
        [SerializeField] Sprite lightQueen;

        [Header("Dark Sprites")]
        [SerializeField] Sprite darkPawn;
        [SerializeField] Sprite darkRook;
        [SerializeField] Sprite darkKnight;
        [SerializeField] Sprite darkBishop;
        [SerializeField] Sprite darkKing;
        [SerializeField] Sprite darkQueen;

        private int maxRowIdx = ChessBoardManager.MatrixRows - 1;
        private int maxColumnIdx = ChessBoardManager.MatrixColumns - 1;

        void OnEnable()
        {
            ChessBoardManager.MatrixEventReceived += OnMatrixEvent;
        }

        void OnDisable()
        {
            ChessBoardManager.MatrixEventReceived -= OnMatrixEvent;
        }

        private void OnMatrixEvent(Cell[,] matrix)
        {
            int cellIdx = 0;

            for (int y = 0 ; y <= maxRowIdx ; y++)
            {
                for (int x = 0 ; x <= maxColumnIdx ; x++)
                {
                    GameObject cell = cells[cellIdx];
                    var  image = cell.GetComponent<Image>() as Image;

                    Cell thisCell = matrix[x, y];

                    if (thisCell.IsOccupied)
                    {
                        switch (thisCell.wrapper.manager.Type)
                        {
                            case Pieces.PieceType.Pawn:
                                image.sprite = (thisCell.wrapper.manager.Set == Set.Light) ? lightPawn : darkPawn;
                                break;

                            case Pieces.PieceType.Rook:
                                image.sprite = (thisCell.wrapper.manager.Set == Set.Light) ? lightRook : darkRook;
                                break;

                            case Pieces.PieceType.Knight:
                                image.sprite = (thisCell.wrapper.manager.Set == Set.Light) ? lightKnight : darkKnight;
                                break;

                            case Pieces.PieceType.Bishop:
                                image.sprite = (thisCell.wrapper.manager.Set == Set.Light) ? lightBishop : darkBishop;
                                break;

                            case Pieces.PieceType.King:
                                image.sprite = (thisCell.wrapper.manager.Set == Set.Light) ? lightKing : darkKing;
                                break;

                            case Pieces.PieceType.Queen:
                                image.sprite = (thisCell.wrapper.manager.Set == Set.Light) ? lightQueen : darkQueen;
                                break;
                        }

                        image.color = Color.white;
                    }
                    else
                    {
                        image.sprite = null;
                        image.color = new Color(1f, 0f, 0f, 0f);
                    }

                    ++cellIdx;
                }
            }
        }
    }
}