using System.Collections.Generic;

using UnityEngine;

using Chess.Pieces;

namespace Chess
{
    public class ChessBoardSetManager : MonoBehaviour
    {
        [Header("Lights")]
        [SerializeField] List<PieceManager> lightPieces;
        [SerializeField] CaptureZoneManager lightCaptureZone;

        [Header("Darks")]
        [SerializeField] List<PieceManager> darkPieces;
        [SerializeField] CaptureZoneManager darkCaptureZone;

        
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

        public bool TryReserveSlot(PieceManager piece, out Vector3 localPosition)
        {
            switch (piece.Set)
            {
                case Set.Light:
                    return lightCaptureZone.TryReserveSlot(piece, out localPosition);
                
                case Set.Dark:
                    return darkCaptureZone.TryReserveSlot(piece, out localPosition);
            }

            localPosition = default(Vector3);
            return false;
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

        public void Reset()
        {
            lightCaptureZone.Reset();
            darkCaptureZone.Reset();
        }
    }
}