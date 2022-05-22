using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Chess.Pieces;

namespace Chess
{
    public class TryGets
    {
        public static bool TryGetCoord(Vector3 localPosition, out Coord coord)
        {
            var normX = ChessMath.Normalize(localPosition.x, -0.35f, 0.35f);
            int x = (int) Mathf.Round(MatrixManager.MaxColumnIdx * (float) normX);

            var normZ = ChessMath.Normalize(localPosition.z, -0.35f, 0.35f);
            int z = (int) Mathf.Round(MatrixManager.MaxRowIdx * (float) normZ);

            if ((x >= 0 && x <= MatrixManager.MaxColumnIdx) && (z >= 0 && z <= MatrixManager.MaxRowIdx))
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

        public static bool TryGetCoordToPosition(Coord coord, float height, out Vector3 localPosition)
        {
            if ((coord.x >= 0 && coord.x <= MatrixManager.MaxColumnIdx) && (coord.y >= 0 && coord.y <= MatrixManager.MaxRowIdx))
            {
                float x = ChessMath.RoundFloat(-0.35f + (coord.x * 0.1f));
                float y = ChessMath.RoundFloat(-0.35f + (coord.y * 0.1f));

                localPosition = new Vector3(x, height, y);
                return true;
            }

            localPosition = default(Vector3);
            return false;
        }

        public static bool TryGetCoordReference(Coord coord, out string reference)
        {
            int maxRowIdx = MatrixManager.MatrixRows - 1;
            int maxColumnIdx = MatrixManager.MatrixColumns - 1;

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

        public static bool TryGetCell(Cell[,] matrix, Vector3 localPosition, out Cell cell)
        {
            for (int y = 0 ; y <= MatrixManager.MaxRowIdx ; y++)
            {
                for (int x = 0 ; x <= MatrixManager.MaxColumnIdx ; x++)
                {
                    cell = matrix[x, y];
                    Vector3 cellPosition = ChessMath.RoundVector3(cell.localPosition);
                    Vector3 queryPosition = ChessMath.RoundVector3(localPosition);

                    if ((cellPosition.x == queryPosition.x) && (cellPosition.z == queryPosition.z))
                    {
                        return true;
                    }
                }
            }

            cell = default(Cell);
            return false;
        }

        public static bool TryGetPiecesAlongVector(Cell[,] projectedMatrix, Cell origin, Vector2 vector, out List<PieceManager> pieceManagers)
        {
            pieceManagers = new List<PieceManager>();

            int x = origin.coord.x;
            int y = origin.coord.y;

            do
            {
                x += (int) vector.x;
                y += (int) vector.y;

                if ((x >= 0 && x <= MatrixManager.MaxColumnIdx) && (y >= 0 && y <= MatrixManager.MaxRowIdx))
                {
                    Cell cell = projectedMatrix[x, y];

                    if (cell.wrapper.manager != null)
                    {
                        pieceManagers.Add(cell.wrapper.manager);
                    }
                }
            } while ((x >= 0 && x <= MatrixManager.MaxColumnIdx) && (y >= 0 && y <= MatrixManager.MaxRowIdx));

            return false;
        }
    
        public static bool TryGetPieceToCell(Cell[,] matrix, PieceManager piece, out Cell cell)
        {
            var localPosition = ChessMath.RoundPosition(piece.transform.localPosition);
            
            if (TryGets.TryGetCoord(localPosition, out Coord coord))
            {
                cell = matrix[coord.x, coord.y];
                return true;
            }

            cell = default(Cell);
            return false;
        }
    
        public static bool TryGetSetPieces(ChessBoardManager manager, Set set, out List<PieceManager> pieces)
        {
            pieces = (set == Set.Light) ? manager.SetManager.ActiveEnabledLightPieces : manager.SetManager.ActiveEnabledDarkPieces;
            return true;
        }

        public static bool TryGetSingleSetPieceByType(ChessBoardManager manager, Set set, PieceType type, out PieceManager pieceManager)
        {
            if (TryGets.TryGetSetPieces(manager, set, out List<PieceManager> activeEnabledPieces))
            {
                List<Cell> matchingCells = activeEnabledPieces.Where(p => p.Type == type).Select(p => p.ActiveCell).ToList();

                if (matchingCells.Count > 0)
                {
                    pieceManager = matchingCells.First().wrapper.manager;                    
                    return (pieceManager != null);
                }
            }
            
            pieceManager = default(PieceManager);
            return false;
        }

        public static bool TryGetSetPiecesByType(ChessBoardManager manager, Set set, PieceType type, out List<PieceManager> pieces)
        {
            if (TryGets.TryGetSetPieces(manager, set, out List<PieceManager> activeEnabledPieces))
            {
                pieces = activeEnabledPieces.Where(p => p.Type == type).ToList();
            }
            else
            {
                pieces = new List<PieceManager>();
            }

            return (pieces.Count > 0);
        }

        public static bool TryGetPiecesByType(ChessBoardManager manager, PieceType type, out List<PieceManager> pieces)
        {
            pieces = manager.SetManager.ActiveEnabledPieces.Where(p => p.Type == type).ToList();
            return (pieces.Count > 0);
        }

        public static bool TryResolveKingCell(Cell[,] matrix, Set set, out Cell cell)
        {
            for (int y = 0 ; y <= MatrixManager.MaxRowIdx ; y++)
            {
                for (int x = 0 ; x <= MatrixManager.MaxColumnIdx ; x++)
                {
                    Cell thisCell = matrix[x, y];
                    if (!thisCell.IsOccupied) continue;
                    PieceManager piece = thisCell.wrapper.manager;
                    
                    if (piece.Set == set && piece.Type == PieceType.King)
                    {
                        cell = thisCell;
                        return true;
                    }
                }
            }

            cell = default(Cell);
            return false;
        }
    }
}