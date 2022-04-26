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
            InCheck,
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
            else if (state == State.InCheck)
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