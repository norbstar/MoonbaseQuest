using System.Collections.Generic;

using UnityEngine;

namespace Chess.Pieces
{
    public class KingManager : PieceManager
    {
        [Header("Materials")]
        [SerializeField] Material inCheckMaterial;
        [SerializeField] Material checkmateMaterial;
        [SerializeField] Material stalemateMaterial;

        public enum State
        {
            Nominal,
            Check,
            Checkmate,
            Stalemate
        }

        private State state;

        protected override List<CoordBundle> GenerateCoordBundles(Cell[,] matrix, int vector)
        {
            List<CoordBundle> bundles = new List<CoordBundle>();

            TryOneTimeVector(-1, 0, bundles);
            TryOneTimeVector(-1, 1, bundles);
            TryOneTimeVector(1, 0, bundles);
            TryOneTimeVector(1, 1, bundles);
            TryOneTimeVector(0, -1, bundles);
            TryOneTimeVector(-1, -1, bundles);
            TryOneTimeVector(0, 1, bundles);
            TryOneTimeVector(1, -1, bundles);

            // In order to castle, the King needs to not have hsitory and have line of sight 2 places to it's left or right.
            // It also can not move through to a position that would place it in check and the rook to it's left/right
            // also can not have history. If these conditions are met, then the Rook can move to the oppositing side
            // of the King to complete the castling.

            // List<PieceManager> rooks = new List<PieceManager>();

            // if (TryGets.TryGetSetPiecesByType(chessBoardManager, Set, PieceType.Rook, out List<PieceManager> pieces))
            // {
            //     foreach(PieceManager piece in pieces)
            //     {
            //         if (!piece.HasHistory)
            //         {
            //             rooks.Add(piece);
            //         }
            //     }
            // }

            // if (!hasHistory)
            // {
            //     TryOneTimeVector(-2, 0, bundles);
            //     TryOneTimeVector(2, 0, bundles);
            // }

            return bundles;
        }

        public State KingState
        {
            get
            {
                return state;
            }

            set
            {
                state = value;
                RefreshInCheck();
            }
        }

        private void RefreshInCheck()
        {
            if (state == State.Checkmate)
            {
                ApplyMaterial(checkmateMaterial);
            }
            else if (state == State.Check)
            {
                ApplyMaterial(inCheckMaterial);
            }
            else if (state == State.Stalemate)
            {
                ApplyMaterial(stalemateMaterial);
            }
            else
            {
                ApplyMaterial(defaultMaterial);
            }
        }

        public override void UseDefaultMaterial()
        {
            base.UseDefaultMaterial();

            RefreshInCheck();
        }

        public override void Reset()
        {
            base.Reset();

            state = State.Nominal;
            RefreshInCheck();
        }
    }
}