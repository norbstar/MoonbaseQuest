using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

using Chess.Pieces;

namespace Chess
{
    [RequireComponent(typeof(ChessBoardManager))]
    public class MoveManager : MonoBehaviour
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        [Header("Config")]
        [SerializeField] MoveStyle moveStyle;
        public MoveStyle MoveStyle { get { return moveStyle; } }

        [SerializeField] MoveType moveType;
        public MoveType MoveType { get { return moveType; } }
        
        private ChessBoardManager chessBoardManager;

        void Awake()
        {
            ResolveDependencies();
            availableMoves = new Dictionary<PieceManager, List<Cell>>();
        }

        private void ResolveDependencies() => chessBoardManager = GetComponent<ChessBoardManager>() as ChessBoardManager;

        public void EvaluateOpeningMove() => EvaluateMove();
        public Dictionary<PieceManager, List<Cell>> AvailableMoves { get { return availableMoves; } }

        private Dictionary<PieceManager, List<Cell>> availableMoves;

        public void EvaluateMove()
        {
            Set activeSet = chessBoardManager.ActiveSet;

            chessBoardManager.StageManager.LiveStage = StageManager.Stage.Evaluating;
            availableMoves.Clear();
            chessBoardManager.InFocusPiece = null;
            
            chessBoardManager.EnableInteractions(Set.Light, false);
            chessBoardManager.EnableInteractions(Set.Dark, false);

            bool inCheck = IsKingInCheck(activeSet, chessBoardManager.MatrixManager.Matrix);
            bool hasMoves = CalculateMoves(out availableMoves);

            chessBoardManager.TimingsManager.ResumeActiveClock();

            if (hasMoves)
            {
                if (inCheck)
                {
                    chessBoardManager.OnCheck(hasMoves);
                }

                if (chessBoardManager.ShouldAutomate())
                {
                    AutomateMove();
                }
            }
            else
            {
                if (inCheck)
                {
                    chessBoardManager.OnCheckmate();
                }
                else
                {
                    chessBoardManager.OnStalemate();
                }
            }
        }

        private void AutomateMove() => PlayRandomMove();

        private void PlayRandomMove()
        {
            if (availableMoves.Count == 0) return;

            int idx = Random.Range(0, availableMoves.Count);
            KeyValuePair<PieceManager, List<Cell>> availableCells = availableMoves.ElementAt(idx);
            int cellIdx = Random.Range(0, availableCells.Value.Count);
            chessBoardManager.InFocusPiece = availableCells.Key;
            Cell cell = availableCells.Value[cellIdx];
            chessBoardManager.CommitToMove(cell);
        }

        public bool WouldKingBeInCheck(PieceManager piece, Cell targetCell)
        {
            Set activeSet = chessBoardManager.ActiveSet;
            Cell[,] projectedMatrix = chessBoardManager.MatrixManager.ProjectMatrix(piece.ActiveCell, targetCell);
            return IsKingInCheck(activeSet, projectedMatrix);
        }

        public bool IsKingInCheck(Set set, Cell[,] matrix)
        {
            Set activeSet = chessBoardManager.ActiveSet;

            if (TryGets.TryResolveKingCell(matrix, set, out Cell kingCell))
            {
                if (TryGets.TryGetSetPieces(chessBoardManager, (activeSet == Set.Light) ? Set.Dark : Set.Light, out List<PieceManager> opposingPieces))
                {
                    if (TryGets.TryGetCoordReference(kingCell.coord, out string kingReference))
                    {
                        foreach (PieceManager opposingPiece in opposingPieces)
                        {
                            TryGets.TryGetCoordReference(opposingPiece.ActiveCell.coord, out string reference);

                            if (opposingPiece.CanMoveTo(matrix, kingCell))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool CalculateMoves(out Dictionary<PieceManager, List<Cell>> availableMoves)
        {
            bool hasAnyMoves = false;
            Set activeSet = chessBoardManager.ActiveSet;
            ChessBoardSetManager setManager = chessBoardManager.SetManager;
            availableMoves = new Dictionary<PieceManager, List<Cell>>();

            List<PieceManager> activeEnabledPieces = (activeSet == Set.Light) ? setManager.ActiveEnabledLightPieces : setManager.ActiveEnabledDarkPieces;

            foreach (PieceManager piece in activeEnabledPieces)
            {
                List<Cell> potentialMoves = piece.CalculateMoves(chessBoardManager.MatrixManager.Matrix, (activeSet == Set.Light) ? 1 : -1);
                bool hasMoves = potentialMoves.Count > 0;

                List<Cell> legalMoves = new List<Cell>();

                foreach (Cell move in potentialMoves)
                {
                    if (!WouldKingBeInCheck(piece, move))
                    {
                        legalMoves.Add(move);
                    }
                }

                hasMoves = legalMoves.Count > 0;

                piece.EnableInteractions(hasMoves);

                if (hasMoves)
                {    
                    availableMoves.Add(piece, legalMoves);
                    hasAnyMoves = true;
                }
                else
                {
                    piece.ApplyMaterial(chessBoardManager.OutOfScopeMaterial);
                }
            }

            chessBoardManager.StageManager.LiveStage = StageManager.Stage.PendingSelect;
            
            chessBoardManager.AdjustChessPieceInteractableLayer(Set.Light, (activeSet == Set.Light));
            chessBoardManager.AdjustChessPieceInteractableLayer(Set.Dark, (activeSet == Set.Dark));

            return hasAnyMoves;
        }
    }
}