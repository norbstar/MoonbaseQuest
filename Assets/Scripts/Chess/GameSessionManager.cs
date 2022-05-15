using UnityEngine;

namespace Chess
{
    public class GameSessionManager : MonoBehaviour
    {
        [SerializeField] GameSessionScriptable scriptable;

        void Awake()
        {
            scriptable = new GameSessionScriptable();
        }

        public void Save(string filename) => scriptable.SaveAsset(filename);

        public void Clear() => scriptable.Clear();

        public void SubmitSnapshot(GameSessionScriptable.Snapshot map) => scriptable.snapshots.Add(map);

        public void SubmitMove(GameSessionScriptable.Move move) => scriptable.moves.Add(move);
    }
}