using System.Collections.Generic;

using UnityEngine;

using Chess.Pieces;

namespace Chess
{
    public class ChessBoardSetManager : MonoBehaviour
    {
        [SerializeField] List<Piece> darkPieces;
        [SerializeField] List<Piece> lightPieces;
        
        public List<Piece> DarkPieces()
        {
            List<Piece> pieces = new List<Piece>();

            foreach (Piece piece in darkPieces)
            {
                pieces.Add(piece);
            }

            return pieces;
        }

        public List<Piece> LightPieces()
        {
            List<Piece> pieces = new List<Piece>();

            foreach (Piece piece in lightPieces)
            {
                pieces.Add(piece);
            }

            return pieces;
        }

        public List<Piece> AllPieces()
        {
            List<Piece> pieces = new List<Piece>();
            
            var darkPieces = DarkPieces() as List<Piece>;
            pieces.AddRange(darkPieces);
            
            var lightPieces = LightPieces() as List<Piece>;
            pieces.AddRange(lightPieces);

            return pieces;
        }
    }
}