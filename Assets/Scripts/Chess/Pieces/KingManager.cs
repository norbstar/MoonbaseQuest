using System.Collections.Generic;

using UnityEngine;

namespace Chess.Pieces
{
    public class KingManager : PieceManager
    {
        [Header("Materials")]
        [SerializeField] Material inCheckMaterial;
        [SerializeField] Material inCheckMateMaterial;

        private bool inCheck;
        private bool inCheckMate;
        
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

        public bool InCheck
        {
            get
            {
                return inCheck;
            }

            set
            {
                inCheck = value;
                RefreshInCheck();
            }
        }

        public bool InCheckMate
        {
            get
            {
                return inCheckMate;
            }

            set
            {
                inCheckMate = value;
                RefreshInCheck();
            }
        }

        private void RefreshInCheck()
        {
            if (inCheckMate)
            {
                ApplyMaterial(inCheckMateMaterial);
            }
            else if (inCheck)
            {
                ApplyMaterial(inCheckMaterial);
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

            inCheck = inCheckMate = false;
            RefreshInCheck();
        }
    }
}