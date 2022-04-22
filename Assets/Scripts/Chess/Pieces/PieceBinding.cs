using UnityEngine;

namespace Chess.Pieces
{
    public class PieceBinding
    {
        [Header("Config")]
        [SerializeField] Set set;
        public Set Set { get { return set; } }
        [SerializeField] PieceType type;
        public PieceType Type { get { return type; } }

        public Cell HomeCell
        {
            get
            {
                return homeCell;
            }
            
            set
            {
                ActiveCell = homeCell = value;
            }
        }

        public Cell ActiveCell
        {
            get
            {
                return activeCell;
            }
            
            set
            {
                activeCell = value;
            }
        }

        private Cell homeCell;
        private Cell activeCell;
    }
}