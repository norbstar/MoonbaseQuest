using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using Chess.Pieces;

namespace Chess
{
    public class TryGets
    {
        public static bool TryGetSetPiecesByType(ChessBoardSetManager manager, Set set, PieceType type, out List<Cell> cells)
        {
            List<PieceManager> activePieces;

            if (set == Set.Light)
            {
                activePieces = manager.LightPieces().Where(p => p.isActiveAndEnabled).ToList();
            }
            else
            {
                activePieces = manager.DarkPieces().Where(p => p.isActiveAndEnabled).ToList();
            }

            List<Cell> matchingCells = activePieces.Where(p => p.Type == type).Select(p => p.ActiveCell).ToList();

            cells = matchingCells;
            return (matchingCells.Count > 0);
        }

        public static bool TryGetPiecesByType(ChessBoardSetManager manager, PieceType type, out List<Cell> cells)
        {
            List<PieceManager> activePieces = manager.AllPieces().Where(p => p.isActiveAndEnabled).ToList();
            List<Cell> matchingCells = activePieces.Where(p => p.Type == type).Select(p => p.ActiveCell).ToList();

            cells = matchingCells;
            return (matchingCells.Count > 0);
        }

        public static bool TryGetPieceToCell(Cell[,] matrix, PieceManager piece, out Cell cell)
        {
            var localPosition = ChessMath.RoundPosition(piece.transform.localPosition);
            
            if (TryGetCoord(localPosition, out Coord coord))
            {
                cell = matrix[coord.x, coord.y];
                return true;
            }

            cell = default(Cell);
            return false;
        }

        public static bool TryGetCoord(Vector3 localPosition, out Coord coord)
        {
            int maxRowIdx = ChessBoardManager.MatrixRows - 1;
            int maxColumnIdx = ChessBoardManager.MatrixColumns - 1;

            var normX = ChessMath.Normalize(localPosition.x, -0.35f, 0.35f);
            int x = (int) Mathf.Round(maxColumnIdx * (float) normX);

            var normZ = ChessMath.Normalize(localPosition.z, -0.35f, 0.35f);
            int z = (int) Mathf.Round(maxRowIdx * (float) normZ);

            if ((x >= 0 && x <= maxColumnIdx) && (z >= 0 && z <= maxRowIdx))
            {
                coord = new Coord
                {
                    x = x,
                    y = z
                };

                return true;
            }

            coord = default(Coord);
            return false;
        }

        public static bool TryGetCoordToPosition(ChessBoardSetManager manager, Coord coord, out Vector3 localPosition)
        {
            int maxRowIdx = ChessBoardManager.MatrixRows - 1;
            int maxColumnIdx = ChessBoardManager.MatrixColumns - 1;
            Vector3 surface = manager.transform.localPosition;

            if ((coord.x >= 0 && coord.x <= maxColumnIdx) && (coord.y >= 0 && coord.y <= maxRowIdx))
            {
                float x = ChessMath.RoundFloat(-0.35f + (coord.x * 0.1f));
                float y = ChessMath.RoundFloat(-0.35f + (coord.y * 0.1f));

                localPosition = new Vector3(x, surface.y, y);
                return true;
            }

            localPosition = default(Vector3);
            return false;
        }

        public static bool TryGetCoordReference(Coord coord, out string reference)
        {
            int maxRowIdx = ChessBoardManager.MatrixRows - 1;
            int maxColumnIdx = ChessBoardManager.MatrixColumns - 1;

            if ((coord.x >= 0 && coord.x <= maxColumnIdx) && (coord.y >= 0 && coord.y <= maxRowIdx))
            {
                char letter = Convert.ToChar((int) 'a' + coord.x);
                char digit = Convert.ToChar((int) '1' + coord.y);
                reference = $"{letter} : {digit}";
                return true;
            }

            reference = default(string);
            return false;
        }
    }
}