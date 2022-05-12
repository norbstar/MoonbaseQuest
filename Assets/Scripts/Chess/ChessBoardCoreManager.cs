using System.Reflection;

using UnityEngine;

namespace Chess
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(BoardManager))]
    [RequireComponent(typeof(MatrixManager))]
    [RequireComponent(typeof(MoveManager))]
    [RequireComponent(typeof(ClocksManager))]
    [RequireComponent(typeof(NewGameManager))]
    [RequireComponent(typeof(PawnPromotionManager))]
    public class ChessBoardCoreManager : BaseManager
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        public delegate void MatrixEvent(Cell[,] matrix);
        public static event MatrixEvent MatrixEventReceived;
        
        [Header("Camera")]
        [SerializeField] new Camera camera;

        [Header("Controllers")]
        [SerializeField] protected HybridHandController leftController;
        [SerializeField] protected HybridHandController rightController;

         [Header("Components")]
        [SerializeField] protected GameObject board;
        public GameObject Board { get { return board; } }
        [SerializeField] protected ChessBoardSetManager setManager;
        public ChessBoardSetManager SetManager { get { return setManager; } }
        [SerializeField] protected PieceTransformManager pieceTransformManager;
        public PieceTransformManager PieceTransformManager { get { return pieceTransformManager; } }
        [SerializeField] protected NotificationManager notificationManager;
        public NotificationManager NotificationManager { get { return notificationManager; } }
        [SerializeField] protected NewGameUIManager newGameUIManager;
        public NewGameUIManager NewGameUIManager { get { return newGameUIManager; } }

        [Header("Canvases")]
        [SerializeField] protected CoordReferenceCanvas coordReferenceCanvas;

        [Header("Materials")]
        [SerializeField] protected Material outOfScopeMaterial;
        public Material OutOfScopeMaterial { get { return outOfScopeMaterial; } }
        [SerializeField] protected Material selectedMaterial;
        
        [Header("Pieces")]
        [Range(15f, 35f)]
        [SerializeField] float pieceRotationSpeed = 25f;
        public float PieceRotationSpeed { get { return pieceRotationSpeed; } }

        [Range(0.1f, 25f)]
        [SerializeField] float pieceMoveSpeed = 5f;
        public float PieceMoveSpeed { get { return pieceMoveSpeed; } }

        [Header("Audio")]
        [SerializeField] protected AudioClip pieceDownClip;
        [SerializeField] protected AudioClip inCheckClip;
        [SerializeField] protected AudioClip checkmateClip;
        [SerializeField] protected AudioClip stalemateClip;

        [Header("Prefabs")]
        [SerializeField] protected GameObject previewPrefab;

        protected TrackingMainCameraManager cameraManager;
        protected AudioSource cameraAudioSource;
        protected MatrixManager matrixManager;
        public MatrixManager MatrixManager { get { return matrixManager; } }
        protected MoveManager moveManager;
        protected ClocksManager clocksManager;
        public ClocksManager ClocksManager { get { return clocksManager; } }
        protected BoardManager boardManager;
        public BoardManager BoardManager { get { return boardManager; } }
        protected NewGameManager newGameManager;
        public NewGameManager NewGameManager { get { return newGameManager; } }
        protected AudioSource audioSource;
        protected StageManager stageManager;
        public StageManager StageManager { get { return stageManager; } }
        protected PawnPromotionManager pawnPromotionManager;
        public PawnPromotionManager PawnPromotionManager { get { return pawnPromotionManager; } }
        protected Set activeSet;
        public Set ActiveSet { get { return activeSet; } }

        protected PlayerMode lightPlayer;
        protected PlayerMode darkPlayer;

        public virtual void Awake()
        {
            ResolveDependencies();
            stageManager = new StageManager();
        }

        private void ResolveDependencies()
        {
            cameraManager = camera.GetComponent<TrackingMainCameraManager>() as TrackingMainCameraManager;

            if (cameraManager != null)
            {
                cameraAudioSource = cameraManager.GetComponent<AudioSource>() as AudioSource;
            }

            matrixManager = GetComponent<MatrixManager>() as MatrixManager;
            moveManager = GetComponent<MoveManager>() as MoveManager;
            clocksManager = GetComponent<ClocksManager>() as ClocksManager;
            boardManager = GetComponent<BoardManager>() as BoardManager;
            newGameManager = GetComponent<NewGameManager>() as NewGameManager;
            pawnPromotionManager = GetComponent<PawnPromotionManager>() as PawnPromotionManager;
            audioSource = GetComponent<AudioSource>() as AudioSource;
        }

        protected void PostMatrixUpdate() => MatrixEventReceived?.Invoke(matrixManager.Matrix);
        
        public void AttachLayerToControllers(string layer)
        {
            leftController.AttachLayer(layer);
            rightController.AttachLayer(layer);
        }

        public void DetachLayerFromContollers(string layer)
        {
            leftController.DetachLayer(layer);
            rightController.DetachLayer(layer);
        }
    
        protected void ResetSet() => setManager.Reset();

        protected void PauseClocks() => clocksManager.PauseClocks();

        protected void PauseActiveClock() => clocksManager.PauseActiveClock();

        protected void ResetClocks() => clocksManager.ResetClocks();
    }
}