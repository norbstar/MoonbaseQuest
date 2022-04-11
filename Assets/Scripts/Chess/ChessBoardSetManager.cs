using System.Collections.Generic;

using UnityEngine;

using Chess.Pieces;

namespace Chess
{
    public class ChessBoardSetManager : MonoBehaviour
    {
        [SerializeField] List<PieceManager> darkPieces;
        [SerializeField] List<PieceManager> lightPieces;
        
        public List<PieceManager> DarkPieces()
        {
            List<PieceManager> pieces = new List<PieceManager>();

            foreach (PieceManager piece in darkPieces)
            {
                pieces.Add(piece);
            }

            return pieces;
        }

        public List<PieceManager> LightPieces()
        {
            List<PieceManager> pieces = new List<PieceManager>();

            foreach (PieceManager piece in lightPieces)
            {
                pieces.Add(piece);
            }

            return pieces;
        }

        public List<PieceManager> AllPieces()
        {
            List<PieceManager> pieces = new List<PieceManager>();
            
            var darkPieces = DarkPieces() as List<PieceManager>;
            pieces.AddRange(darkPieces);
            
            var lightPieces = LightPieces() as List<PieceManager>;
            pieces.AddRange(lightPieces);

            return pieces;
        }
    }
}