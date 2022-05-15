using System.Reflection;

using UnityEngine;

using Chess.Pieces;

namespace Chess
{
    [RequireComponent(typeof(ChessBoardManager))]
    public class PawnPromotionManager : MonoBehaviour
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        [Header("Components")]
        [SerializeField] PawnPromotionUIManager uiManager;

        [Header("Audio")]
        [SerializeField] protected AudioClip promotionClip;

        private ChessBoardManager chessBoardManager;
        private PieceManager promotionPiece;
        private PieceManager pickedPiece;

        void Awake() => ResolveDependencies();

        private void ResolveDependencies() => chessBoardManager = GetComponent<ChessBoardManager>() as ChessBoardManager;

        void OnEnable() => PawnPromotionUIManager.EventReceived += OnEvent;

        void OnDisable() => PawnPromotionUIManager.EventReceived -= OnEvent;

        public bool ShouldPawnBePromoted(Set set, PieceManager piece)
        {
            if (piece.Type == PieceType.Pawn)
            {
                int vector = (set == Set.Light) ? 1 : -1;
                int lastRank = (vector == 1) ? MatrixManager.MaxRowIdx : 0;

                if (piece.ActiveCell.coord.y == lastRank)
                {
                    return true;
                }
            }

            return false;
        }

        public PieceManager ResolvePieceBySet(Set set, PieceType type) => uiManager.ResolvePieceBySet(set, type);

        public void Show(Set set) => uiManager.Show(set);

        public void PreparePawnForPromotion(Set set, PieceManager piece)
        {
            chessBoardManager.Stage = StageManager.Stage.Promoting;
            AudioSource.PlayClipAtPoint(promotionClip, transform.position, 1.0f);

            promotionPiece = piece;
            piece.gameObject.SetActive(false);
            chessBoardManager.PieceTransformManager.ShowAndRaisePiece(set, piece.transform.position);
        }

        public PieceType PickRandomType() => uiManager.PickRandomType();

        public void PromotePiece(Set set)
        {
            Cell cell = promotionPiece.ActiveCell;
            ChessBoardSetManager setManager = chessBoardManager.SetManager;

            if (promotionPiece.IsAddInPiece)
            {
                setManager.RemovePiece(promotionPiece);
            }
            else
            {
                promotionPiece.gameObject.SetActive(true);

                if (setManager.TryReserveSlot(cell.wrapper.manager, out Vector3 localPosition))
                {
                    cell.wrapper.manager.transform.localPosition = localPosition;
                    cell.wrapper.manager.EnableInteractions(false);
                    cell.wrapper.manager.ActiveCell = null;
                    cell.wrapper.manager.ShowMesh();
                    cell.wrapper.manager = null;
                }
            }

            PieceManager promotedPiece = setManager.AddPiece(set, pickedPiece, cell.coord, true);
            cell.wrapper.manager = promotedPiece;

            promotedPiece.ActiveCell = cell;
            promotedPiece.MoveEventReceived += chessBoardManager.OnMoveEvent;
            promotedPiece.EventReceived += chessBoardManager.OnPieceEvent;

            chessBoardManager.MatrixManager.Matrix[cell.coord.x, cell.coord.y].wrapper.manager = promotedPiece;
            promotionPiece = null;

            chessBoardManager.CompleteTurn();
        }

        public void SetPickedPiece(Set set, PieceManager piece)
        {
            PieceTransformManager manager = chessBoardManager.PieceTransformManager;

            pickedPiece = piece;

            manager.SetPiece(set, piece.Type);
            manager.LowerAndHidePiece();
        }

        public void OnEvent(Set set, PieceManager piece)
        {
            uiManager.Hide();
            SetPickedPiece(set, piece);
        }
    }
}