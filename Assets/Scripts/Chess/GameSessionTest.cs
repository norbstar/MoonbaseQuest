using UnityEngine;

namespace Chess
{
    public class GameSessionTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var scriptable = CreateTestGameSession();
            scriptable.SaveAsset("test");
        }

        private GameSessionScriptable CreateTestGameSession()
        {
            GameSessionScriptable scriptable = new GameSessionScriptable();

            GameSessionScriptable.Snapshot snapshot = new GameSessionScriptable.Snapshot();

            snapshot.lightSet.pieces.Add(new GameSessionScriptable.Piece
            {
                type = Pieces.PieceType.King,
                coord = new Coord
                {
                    x = 0,
                    y = 4
                }
            });

            snapshot.darkSet.pieces.Add(new GameSessionScriptable.Piece
            {
                type = Pieces.PieceType.King,
                coord = new Coord
                {
                    x = 7,
                    y = 5
                }
            });

            scriptable.snapshots.Add(snapshot);

            GameSessionScriptable.Move move = new GameSessionScriptable.Move();

            move.piece = new GameSessionScriptable.Piece
            {
                type = Pieces.PieceType.King,
                coord = new Coord
                {
                    x = 0,
                    y = 4
                }
            };

            move.to = new Coord
            {
                x = 1,
                y = 4
            };

            scriptable.moves.Add(move);

            return scriptable;
        }
    }
}