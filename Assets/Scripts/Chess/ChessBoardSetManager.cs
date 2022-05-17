using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

using Chess.Pieces;

namespace Chess
{
    public class ChessBoardSetManager : MonoBehaviour
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        [Header("Components")]
        [SerializeField] ChessBoardManager chessBoardManager;
        [SerializeField] GameObject lightSet;
        [SerializeField] GameObject darkSet;

        [Header("Lights")]
        [SerializeField] List<PieceManager> lightPieces;
        [SerializeField] CaptureZoneManager lightCaptureZone;

        [Header("Darks")]
        [SerializeField] List<PieceManager> darkPieces;
        [SerializeField] CaptureZoneManager darkCaptureZone;

        [Header("Setup")]
        [SerializeField] BoardSetupScriptable boardSetup;

        public List<PieceManager> EnabledPieces { get { return AllPieces().Where(p => p.isActiveAndEnabled).ToList(); } }

        public List<PieceManager> ActiveEnabledPieces { get { return AllPieces().Where(p => p.isActiveAndEnabled && (p.ActiveCell != null)).ToList(); } }

        public List<PieceManager> EnabledLightPieces { get { return LightPieces().Where(p => p.isActiveAndEnabled).ToList(); } }

        public List<PieceManager> ActiveEnabledLightPieces { get { return LightPieces().Where(p => p.isActiveAndEnabled && (p.ActiveCell != null)).ToList(); } }

        public List<PieceManager> EnabledDarkPieces { get { return DarkPieces().Where(p => p.isActiveAndEnabled).ToList(); } }

        public List<PieceManager> ActiveEnabledDarkPieces { get { return DarkPieces().Where(p => p.isActiveAndEnabled && (p.ActiveCell != null)).ToList(); } }
        
        // Start is called before the first frame update
        void Start()
        {
            if (boardSetup == null) return;

            AddPieces(Set.Light, boardSetup.LightPieces);
            AddPieces(Set.Dark, boardSetup.DarkPieces);
        }

        private void AddPieces(Set set, List<Chess.BoardSetupScriptable.Piece> pieces)
        {
            foreach (Chess.BoardSetupScriptable.Piece piece in pieces)
            {
                AddPiece(set, piece.manager, piece.coord);
            }
        }

        public PieceManager AddPiece(Set set, PieceManager piece, Coord coord, bool isAddInPiece = false)
        {
            if (TryGets.TryGetCoordToPosition(coord, transform.localPosition.y, out Vector3 localPosition))
            {
                var instance = GameObject.Instantiate(piece.gameObject);
                instance.transform.parent = (set == Set.Light) ? lightSet.transform : darkSet.transform;
                instance.transform.localPosition = localPosition;
                instance.transform.localRotation = (set == Set.Light) ? Quaternion.identity : Quaternion.Euler(0f, 180f, 0f);
                
                var manager = instance.GetComponent<PieceManager>() as PieceManager;
                manager.ChessBoardManager = chessBoardManager;
                manager.IsAddInPiece = isAddInPiece;

                List<PieceManager> list = (set == Set.Light) ? lightPieces : darkPieces;
                list.Add(manager);

                return manager;
            }

            return null;
        }

        public void RemovePiece(PieceManager piece)
        {
            List<PieceManager> list = (piece.Set == Set.Light) ? lightPieces : darkPieces;
            list.Remove(piece);

            GameObject.Destroy(piece.gameObject);
        }

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