using System.Reflection;

using UnityEngine;

namespace Chess
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(BoardManager))]
    [RequireComponent(typeof(MatrixManager))]
    [RequireComponent(typeof(MoveManager))]
    [RequireComponent(typeof(TimingsManager))]
    public class ChessBoardCoreManager : BaseManager
    {
        private static string className = MethodBase.GetCurrentMethod().DeclaringType.Name;

        [Header("Camera")]
        [SerializeField] new Camera camera;

        [Header("Controllers")]
        [SerializeField] HybridHandController leftController;
        [SerializeField] HybridHandController rightController;
        
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}