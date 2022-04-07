using UnityEngine;

namespace Chess.Pieces
{
    public abstract class Piece : MonoBehaviour
    {
        [SerializeField] Type type;
        public Type Type { get { return type; } }

        [SerializeField] Theme theme;
        public Theme Theme { get { return theme; } }
        
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