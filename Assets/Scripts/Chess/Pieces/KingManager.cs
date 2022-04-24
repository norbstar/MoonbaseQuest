using System.Collections.Generic;

using UnityEngine;

namespace Chess.Pieces
{
    public class KingManager : PieceManager
    {
        [Header("Materials")]
        [SerializeField] Material inCheckMaterial;

        private bool inCheck;
        
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

                if (inCheck)
                {
                    ApplyMaterial(inCheckMaterial);
                }
            }
        }

        public override void UseDefaultMaterial()
        {
            if (!inCheck)
            {
                base.UseDefaultMaterial();
            }
        }

        public override void Reset()
        {
            base.Reset();
            inCheck = false;
        }
    }
}