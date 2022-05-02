using UnityEngine;

namespace Chess
{
    [RequireComponent(typeof(Animator))]
    public class PieceTransformAnimationManager : MonoBehaviour
    {
        [SerializeField] PieceTransformManager pieceTransformManager;

        private Animator animator;

        void Awake()
        {
            ResolveDependencies();
        }

        private void ResolveDependencies()
        {
            animator = GetComponent<Animator>() as Animator;
        }
        
        public void Invoke(PieceTransformManager.Action action)
        {
            switch (action)
            {
                case PieceTransformManager.Action.Raise:
                    animator.SetInteger("state", 1);
                    break;

                case PieceTransformManager.Action.Lower:
                    animator.SetInteger("state", -1);
                    break;
            }
        }

        public void OnRaiseComplete() => pieceTransformManager.OnRaiseComplete();

        public void OnLowerComplete() => pieceTransformManager.OnLowerComplete();
    }
}