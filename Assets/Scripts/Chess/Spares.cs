using UnityEngine;

namespace Chess
{
    public class Spares : MonoBehaviour
    {
#if false
        foreach (PieceManager piece in ActivePieces)
        {
            if (piece.isActiveAndEnabled)
            {
                piece.TestKingTrajectories();
            }
        }

        public bool TryGetSetPieceCellsByType(Set set, PieceType type, out List<Cell> cells)
        {
            List<PieceManager> activePieces = (set == Set.Light) ? ActiveLightPieces : ActiveDarkPieces;
            List<Cell> matchingCells = activePieces.Where(p => p.Type == type).Select(p => p.ActiveCell).ToList();

            cells = matchingCells;
            return (matchingCells.Count > 0);
        }

        public bool TryGetPieceCellsByType(PieceType type, out List<Cell> cells)
        {
            List<Cell> matchingCells = ActivePieces.Where(p => p.Type == type).Select(p => p.ActiveCell).ToList();

            cells = matchingCells;
            return (matchingCells.Count > 0);
        }

        public Cell ResolveKingCell(Set set)
        {
            bool isKing = (type == PieceType.King);

            if (isKing)
            {
                return ActiveCell;
            }

            if (chessBoardManager.TryGetSingleSetPieceByType(set, PieceType.King, out Cell cell))
            {
                return cell;
            }

            return null;
        }

        private bool WouldMovePlaceKingInCheck(Cell kingCell, Cell targetCell)
        {
            if (kingCell == null) return false;

            Debug.Log($"WouldMovePlaceKingInCheck King : {kingCell.piece.name} Piece : {name} Target Cell : [{targetCell.coord.x}, {targetCell.coord.y}]");

            if (TryGetRealtiveKingVector(kingCell, out VectorPackage package))
            {
                Debug.Log($"WouldMovePlaceKingInCheck Vector : [{package.Vector.x}, {package.Vector.y}] Type : {package.Type}");

                Cell[,] projectedMatrix = ProjectMatrix(ActiveCell, targetCell);

                if (chessBoardManager.TryGetPiecesAlongVector(projectedMatrix, kingCell, package.Vector, out List<PieceManager> pieces))
                {
                    foreach (PieceManager piece in pieces)
                    {
                        Debug.Log($"WouldMovePlaceKingInCheck Piece : {piece.name}");
                    }
                }
            }

            return false;
        }

        private bool WouldMovingKingPlaceKingInCheck(Cell kingCell, Cell targetCell)
        {
            if (kingCell == null) return false;

            Debug.Log($"WouldMovingKingPlaceKingInCheck King : {kingCell.piece.name} Piece : {name} Target Cell : [{targetCell.coord.x}, {targetCell.coord.y}]");

            return false;
        }

        public bool CanMoveTo(Cell projectedCell, Cell targetCell)
        {
            Cell[,] projectedMatrix = ProjectMatrix(ActiveCell, projectedCell);
            return CanMoveTo(projectedMatrix, targetCell);
        }

        Cell kingCell = ResolveKingCell(set);
            
        foreach (Cell potentialMove in potentialMoves)
        {
            Debug.Log($"CalculateMoves Piece : {name} Move : [{potentialMove.coord.x}, {potentialMove.coord.y}]");

            if ((type == PieceType.King) && (!WouldMovingKingPlaceKingInCheck(kingCell, potentialMove)))
            {
                // Moving the King to the potential cell would not place it in check by a piece of the opposing set.
                moves.Add(potentialMove);
            }
            else if (!WouldMovePlaceKingInCheck(kingCell, potentialMove))
            {
                // Moving the piece relative to the King would not expose the King to check by a piece of the opposing set.
                moves.Add(potentialMove);
            }
        }
#endif
    }
}