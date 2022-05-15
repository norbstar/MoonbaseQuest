using System.Collections;

using UnityEngine;

namespace Chess
{
    [RequireComponent(typeof(ChessBoardManager))]
    [RequireComponent(typeof(AudioSource))]
    public class BoardManager : MonoBehaviour
    {
        public enum MoveType
        {
            Lower,
            Raise,
            Reset
        }


        [Header("Audio")]
        [SerializeField] AudioClip adjustTableHeightClip;

        [Header("Config")]
        [SerializeField] float adjustTableSpeed = 0.5f;

        private ChessBoardManager chessBoardManager;
        private AudioSource audioSource;
        private Coroutine coroutine;
        private float defaultTableYOffset = 0f;
        private float lowerYTableBounds = -0.25f;
        private float upperYTableBounds = 0.25f;

        void Awake() => ResolveDependencies();

        private void ResolveDependencies()
        {
            chessBoardManager = GetComponent<ChessBoardManager>() as ChessBoardManager;
            audioSource = GetComponent<AudioSource>() as AudioSource;
        }

        public void Move(MoveType moveType) => coroutine = StartCoroutine(MoveTableCoroutine(moveType, adjustTableSpeed));

        private IEnumerator MoveTableCoroutine(MoveType moveType, float movementSpeed)
        {
            GameObject board = chessBoardManager.gameObject;
            float yPosition = 0;

            switch (moveType)
            {
                case MoveType.Lower:
                    yPosition = lowerYTableBounds;
                    break;

                case MoveType.Raise:
                    yPosition = upperYTableBounds;
                    break;

                case MoveType.Reset:
                    yPosition = defaultTableYOffset;
                    break;
            }


            Vector3 targetPosition = new Vector3(board.transform.localPosition.x, yPosition, board.transform.localPosition.z);
            
            audioSource.clip = adjustTableHeightClip;
            audioSource.Play();

            while (board.transform.localPosition != targetPosition)
            {
                board.transform.localPosition = Vector3.MoveTowards(board.transform.localPosition, targetPosition, movementSpeed * Time.deltaTime);
                yield return null;
            }

            audioSource.Stop();
        }
    
        public void LockTable()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }

            audioSource.Stop();
        }
    }
}